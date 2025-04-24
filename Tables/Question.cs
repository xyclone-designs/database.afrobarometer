using System.Linq;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;

namespace Database.Afrobarometer.Tables
{
	[SQLite.Table("questions")]
    public class Question : _AfrobarometerModel
    {
		[StringSyntax("Regex")]
		public static readonly string RegexQuestionText = "^[qQ]?[0-9][0-9]?[0-9]?[a-zA-Z]?[0-9]?.?";

		public static readonly char[] TrimQuestionText = ['?', ' ', '.'];
		public static readonly char[] TrimQuestionTexts = [',', '.', '"'];
		public static readonly string[] ReplacementQuestionTextCountry =
		[
			"ghana",
			"mali",
			"malawi",
			"namibia",
			"nigeria",
			"south africa",
			"tanzania",
			"uganda",
			"zambia",
			"zimbabwe",
		];
		public static readonly string[] ReplacementQuestionTextNationality =
		[
			"basotho",
			"ghanaian",
			"ghanaians",
			"ghanians",
			"ghanian",
			"malians",
			"malian",
			"malawians",
			"malawian",
			"mosotho",
			"namibians",
			"namibian",
			"nigerians",
			"nigerian",
			"south african",
			"south africans",
			"south african people",
			"tanzanians",
			"tanzanian",
			"ugandans",
			"ugandan",
			"zambians",
			"zambian",
			"zimbabweans",
			"zimbabwean",
		];

		public Question() { }
		public Question(params string[] texts)
		{
			if (texts.Length >= 5)
			{
				// 661=DIOURBEL, =FATICK => 661=DIOURBEL, 662=FATICK (Add one to previous)
				MatchCollection matches1 = Regex.Matches(texts[4], "[0-9]*=[A-Za-z]+[,\\s]+=[A-Za-z]");

				for (int index = 0; index < matches1.Count; index++)
				{
					int num = int.Parse(matches1[index].Value.Split('=')[0]);

					string _was = string.Join(string.Empty, matches1[index].Value[0..^2]);
					string _is = string.Format("{0}{1}", _was, num + 1);

					texts[4] = texts[4].Replace(_was, _is);
				}

				// college 7=Some => college ,7=Some (Add comma)
				// know8=Refused => know, 8=Refused
				MatchCollection matches2 = Regex.Matches(texts[4], "[^,\\s0-9]\\s*[0-9]*=");

				for (int index = 0; index < matches2.Count; index++)
				{
					string _was = matches2[index].Value; 
					string _is = string.Join(',', [matches2[index].Value[0], matches2[index].Value[1..^0]]); 

					texts[4] = texts[4].Replace(_was, _is);
				}
			}

			string[] values = texts.ElementAtOrDefault(3)?.Split(TrimQuestionTexts, StringSplitOptions.TrimEntries) ?? [];
			string[] valuelables = texts.ElementAtOrDefault(4)?
				.Replace(';', ',')
				.Split(TrimQuestionTexts, StringSplitOptions.TrimEntries) ?? [];

			for (int index = 0; index < valuelables.Length; index++)
			{
				string[] split = valuelables[index].Split('=', '"', StringSplitOptions.TrimEntries);

				if (index > 0 && int.TryParse(split[0], out int _) is false)
				{
					valuelables[index - 1] = string.Format("{0}, {1}", valuelables[index - 1], valuelables[index]);
					valuelables = valuelables
						.Where((_, __) => index != __)
						.ToArray();
					index--;
				}
			}

			QuestionNumber = texts.ElementAtOrDefault(0)?.Trim();
			QuestionText = texts.ElementAtOrDefault(1)?.Trim();
			VariableLabel = texts.ElementAtOrDefault(2)?.Trim();
			Values = values.Select(_ => _.Trim()).ToArray();
			ValueLabels = valuelables.Select(_ => _.Trim()).ToArray();
			Source = texts.ElementAtOrDefault(5)?.Trim();
			Note = texts.ElementAtOrDefault(6)?.Trim();

			if (QuestionText is not null)
			{
				// 1: Trimming and Spelling Errors 
				// 2: Remove Question Prefix's [ 'Q23', 'Q23a', 'Q43b2', '23', '23a', 'Q43b2' ]
				// 3: Replace Country Names 

				QuestionText = QuestionText
					.ToLower()
					.Replace("/ ", "/")
					.Replace("/", " or ")
					.Replace("&", "and")
					.Replace("vs.", "vs")
					.Replace("canellation", "cacnellation")
					.Replace("demonstration", "demostartion")
					.Replace("dififculty", "difficulty")
					.Replace("health care", "healthcare")
					.Replace("govt", "government")
					.Replace("govt.", "government")
					.Replace("government.", "government")
					.Replace("groups", "group")
					.Replace("group's", "group")
					.Replace("programme", "programme")
					.Replace("regards to 1999", "regards to the 1999")
					.Replace("responsibilty", "responsibility")
					.Replace("resonsibility", "responsibility")
					.Replace("storey", "story")
					.Replace("Pidgin=English", "Pidgin-English")
					.Replace("Beri-beri", "Beri-beri")
					.Replace("Mixed-race", "Mixed-race")
					.Replace("Mixed=-=English/Afrikaans", "Mixed:English/Afrikaans")
					.Trim(TrimQuestionText);

				QuestionText = Regex.Replace(QuestionText, RegexQuestionText, string.Empty);

				foreach (string replacementquestiontextcountry in ReplacementQuestionTextCountry)
					QuestionText = QuestionText.Replace(replacementquestiontextcountry, "[country]", StringComparison.OrdinalIgnoreCase);

				foreach (string replacementquestiontextnationality in ReplacementQuestionTextNationality)
					QuestionText = QuestionText.Replace(replacementquestiontextnationality, "[nationality]", StringComparison.OrdinalIgnoreCase);

				QuestionText = QuestionText.Trim(TrimQuestionText);
			}
		}

		public string? QuestionNumber { get; set; }
        public string? QuestionText { get; set; }
		public string? VariableLabel { get; set; }
		public string[]? Values { get; set; }
		public string[]? ValueLabels { get; set; }
		public string? Source { get; set; }
		public string? Note { get; set; }

		public override void Log(StreamWriter streamwriter)
		{
			base.Log(streamwriter);

			streamwriter.WriteLine("QuestionNumber: {0}", QuestionNumber);
			streamwriter.WriteLine("Question: {0}", QuestionText);
			streamwriter.WriteLine("VariableLabel: {0}", VariableLabel);
			streamwriter.WriteLine("Values: {0}", Values?.ElementAtOrDefault(0));

			if (Values is not null && Values.Length > 2)
				for (int index = 1; index < Values.Length; index++)
					streamwriter.WriteLine("        {0}", Values[index]);

			streamwriter.WriteLine("ValueLabels: {0}", ValueLabels?.ElementAtOrDefault(0));

			if (ValueLabels is not null && ValueLabels.Length > 2)
				for (int index = 1; index < ValueLabels.Length; index++)
					streamwriter.WriteLine("             {0}", ValueLabels[index]);

			streamwriter.WriteLine("Source: {0}", Source);
			streamwriter.WriteLine("Note: {0}", Note);
			streamwriter.WriteLine();
		}
		public override bool LogErrors(StreamWriter streamwriter)
		{
			IEnumerable<string> valuelabels = ValueLabels?.Where(_ => _.Count(_ => '=' == _) >= 2) ?? Enumerable.Empty<string>();

			bool error = base.LogErrors(streamwriter) || valuelabels.Any();

			if (error)
			{
				error = true;
				streamwriter.WriteLine("QuestionNumber: {0}", QuestionNumber);

				if (valuelabels.Any())
				{
					streamwriter.WriteLine("ValueLabels");
					foreach (string valuelabel in valuelabels)
						streamwriter.WriteLine("    {0}", valuelabel);
				}
			}
			
			return error;
		}
	}
}