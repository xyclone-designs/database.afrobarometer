using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace Database.Afrobarometer
{
	public static partial class Utils
	{
		public static partial class Inputs
		{
			public static class Question
			{
				[StringSyntax("Regex")]
				public static readonly char[] QuestionText_Trim = [',', '.', '"'];
				public static readonly string QuestionText_Regex = "^[qQ]?[0-9][0-9]?[0-9]?[a-zA-Z]?[0-9]?.?";

				[StringSyntax("Regex")]
				public static readonly string ValueLabels_Regex_One = "[0-9]*=[A-Za-z]+[,\\s]+=[A-Za-z]"; // 661=DIOURBEL, =FATICK => 661=DIOURBEL, 662=FATICK (Add one to previous)
				[StringSyntax("Regex")]
				public static readonly string ValueLabels_Regex_Two = "[^,\\s0-9]\\s*[0-9]*="; // know8=Refused => know, 8=Refused (Add comma)
				[StringSyntax("Regex")]
				public static readonly string ValueLabels_Regex_Three = "[,]\\s*[0-9]+\\s+[0-9]+\\s?="; // 661=migrated, 44  18=Travel (Remove lone number)
				public static readonly string ValueLabels_Regex_Four = "[,]\\s*[0-9]+\\s*[0-9]+\\s+[0-9]+\\s?="; // 661=migrated, 6737 44  18=Travel (Remove dual lone number)
				public static readonly string[][] ValueLabels_Replacements = // stubborn
				[
					["518 =Sonrhai, =Makua", "518 =Sonrhai, 540=Makua"],
					["9= Turkey, =Ghana", "9= Turkey, 10=Ghana"],
				];

				public static readonly string[] ValueLabels_NotApplicables =
				[
					"1=very happy, 2= ++, 3= +, 4= =, 5=  -, 6= --,7=not at all happy,  9=don’t know, -1=Missing data",
				];

				public static string? QuestionNumber(string? questionnumber)
				{
					return questionnumber;
				}
				public static string? QuestionText(string? questiontext)
				{
					if (questiontext is null)
						return null;

					// 1: Trimming and Spelling Errors 
					// 2: Remove Question Prefix's [ 'Q23', 'Q23a', 'Q43b2', '23', '23a', 'Q43b2' ]
					// 3: Replace Country Names 

					questiontext = Regex.Replace(questiontext.ToLower().Trim(QuestionText_Trim), QuestionText_Regex, string.Empty);

					foreach (string replacementsenglish_country in ReplacementsEnglish_Country)
						questiontext = questiontext.Replace(replacementsenglish_country, ReplacementsEnglish_Country[0], StringComparison.OrdinalIgnoreCase);

					foreach (string replacementsenglish_nationality in ReplacementsEnglish_Nationality)
						questiontext = questiontext.Replace(replacementsenglish_nationality, ReplacementsEnglish_Nationality[0], StringComparison.OrdinalIgnoreCase);

					return questiontext.Trim(QuestionText_Trim);
				}
				public static string? VariableLabel(string? variablelabel)
				{
					return variablelabel;
				}
				public static string[]? Values(string? values)
				{
					return values?.Split(',');
				}
				public static string[]? ValueLabels(string? valuelabels)
				{
					if (valuelabels is null)
						return null;

					MatchCollection matches_one = Regex.Matches(valuelabels, ValueLabels_Regex_One);

					for (int index = 0; index < matches_one.Count; index++)
					{
						int num = int.Parse(matches_one[index].Value.Split('=')[0]);

						string _was = string.Join(string.Empty, matches_one[index].Value[0..^2]);
						string _is = string.Format("{0}{1}", _was, num + 1);

						valuelabels = valuelabels.Replace(_was, _is);
					}

					MatchCollection matches_two = Regex.Matches(valuelabels, ValueLabels_Regex_Two);

					for (int index = 0; index < matches_two.Count; index++)
					{
						string _was = matches_two[index].Value;
						string _is = string.Join(',', [matches_two[index].Value[0], matches_two[index].Value[1..^0]]);

						valuelabels = valuelabels.Replace(_was, _is);
					}

					MatchCollection matches_three = Regex.Matches(valuelabels, ValueLabels_Regex_Three);

					for (int index = 0; index < matches_three.Count; index++)
					{
						string _was = matches_three[index].Value;
						string _is = string.Format(", {0}", matches_three[index].Value.Split(' ').Last());

						valuelabels = valuelabels.Replace(_was, _is);
					}

					MatchCollection matches_four = Regex.Matches(valuelabels, ValueLabels_Regex_Four);

					for (int index = 0; index < matches_four.Count; index++)
					{
						string _was = matches_four[index].Value;
						string _is = string.Format(", {0}", matches_four[index].Value.Split(' ').Last());

						valuelabels = valuelabels.Replace(_was, _is);
					}

					for (int index = 0; index < ValueLabels_Replacements.Length; index++)
						valuelabels = valuelabels.Replace(ValueLabels_Replacements[index][0], ValueLabels_Replacements[index][1]);

					string[] _valuelabels = valuelabels
						.Replace(';', ',')
						.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

					for (int index = 0; index < _valuelabels.Length; index++)
						if (ValueLabels_NotApplicables.Contains(_valuelabels[index]) is false)
						{
							string[] split = _valuelabels[index].Split('=', '"', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

							if (index > 0 && int.TryParse(split.ElementAtOrDefault(0), out int _) is false)
							{
								_valuelabels[index - 1] = string.Format("{0}, {1}", _valuelabels[index - 1], _valuelabels[index]);
								_valuelabels = _valuelabels
									.Where((_, __) => index != __)
									.ToArray();
								index--;
							}
						}

					return _valuelabels;
				}
				public static string? Source(string? source)
				{
					return source;
				}
				public static string? Note(string? note)
				{
					return note;
				}
			}
		}
	}
}
