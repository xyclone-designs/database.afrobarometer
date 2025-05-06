using Database.Afrobarometer.Enums;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using CodebookPDFQuestion = Database.Afrobarometer.Inputs.CodebookPDF.Question;
using TablesQuestion = Database.Afrobarometer.Tables.Question;
using TablesVariable = Database.Afrobarometer.Tables.Variable;

namespace Database.Afrobarometer
{
	public static partial class Utils
	{
		public static partial class Inputs
		{
			public static partial class CodebookPDF
			{
				public static class Question
				{
					public static class Replacements
					{
						public static class French
						{
							public static class Text
							{
								public static IEnumerable<string> Country => _Base.Replacements.French.Country;
								public static IEnumerable<string[]> GeneralRegex => Enumerable.Empty<string[]>();
								public static char[] Trim => [];
							}
							public static class ValueLabels
							{
								public static IEnumerable<string[]> General => Enumerable.Empty<string[]>();
								public static IEnumerable<(string Regex, Func<MatchCollection, string, string> OnMatches)> GeneralRegex => Enumerable
									.Empty<(string Regex, Func<MatchCollection, string, string> OnMatches)>();
								public static IEnumerable<string> NotApplicables => Enumerable.Empty<string>();
							}
						}
						public static class English
						{
							public static class Text
							{
								public static IEnumerable<string[]> GeneralRegex => Enumerable.Empty<string[]>()
									.Append(["^[qQ]?[0-9][0-9]?[0-9]?[a-zA-Z]?[0-9]?.?", string.Empty]);
								public static char[] Trim => [',', '.', '"'];
							}
							public static class ValueLabels
							{
								public static IEnumerable<string[]> General => Enumerable.Empty<string[]>()
									.Append(["518 =Sonrhai, =Makua", "518 =Sonrhai, 540=Makua"])
									.Append(["9= Turkey, =Ghana", "9= Turkey, 10=Ghana"])
									.Append(["1=A, strongly", "1=A 'strongly'"])
									.Append(["2=A, somewhat", "2=A 'somewhat'"])
									.Append(["3=B, somewhat", "3=B 'strongly'"])
									.Append(["4=B, strongly", "4=B 'strongly'"])
									.Append(["Know Answer, but can’t remember", "Know Answer but can’t remember"])
									.Append(["Government of people, by people, for the people", "Government of the people by the people and for the people"]);

								public static string RegexOne(MatchCollection matches, string input)
								{
									for (int index = 0; index < matches.Count; index++)
									{
										int num = int.Parse(matches[index].Value.Split('=')[0]);

										string _was = string.Join(string.Empty, matches[index].Value[0..^2]);
										string _is = string.Format("{0}{1}", _was, num + 1);

										input = input.Replace(_was, _is);
									}

									return input;
								}
								public static string RegexTwo(MatchCollection matches, string input)
								{
									for (int index = 0; index < matches.Count; index++)
									{
										string _was = matches[index].Value;
										string _is = string.Join(',', [matches[index].Value[0], matches[index].Value[1..^0]]);

										input = input.Replace(_was, _is);
									}

									return input;
								}
								public static string RegexThree(MatchCollection matches, string input)
								{
									for (int index = 0; index < matches.Count; index++)
									{
										string _was = matches[index].Value;
										string _is = string.Format(", {0}", matches[index].Value.Split(' ').Last());

										input = input.Replace(_was, _is);
									}

									return input;
								}
								public static string RegexFour(MatchCollection matches, string input)
								{
									for (int index = 0; index < matches.Count; index++)
									{
										string _was = matches[index].Value;
										string _is = string.Format(", {0}", matches[index].Value.Split(' ').Last());

										input = input.Replace(_was, _is);
									}

									return input;
								}

								// 661=DIOURBEL, =FATICK => 661=DIOURBEL, 662=FATICK (Add one to previous)
								// know8=Refused => know, 8=Refused (Add comma)
								// 661=migrated, 44  18=Travel (Remove lone number)
								// 661=migrated, 6737 44  18=Travel (Remove dual lone number)	
								public static IEnumerable<(string Regex, Func<MatchCollection, string, string> OnMatches)> GeneralRegex => Enumerable
									.Empty<(string Regex, Func<MatchCollection, string, string> OnMatches)>()
									.Append(("[0-9]*=[A-Za-z]+[,\\s]+=[A-Za-z]", RegexOne))         
									.Append(("[^,\\s0-9]\\s*[0-9]*=", RegexTwo))                    
									.Append(("[,]\\s*[0-9]+\\s+[0-9]+\\s?=", RegexThree))           
									.Append(("[,]\\s*[0-9]+\\s*[0-9]+\\s+[0-9]+\\s?=", RegexFour));

								public static IEnumerable<string> NotApplicables => Enumerable.Empty<string>()
									.Append("1=very happy, 2= ++, 3= +, 4= =, 5=  -, 6= --,7=not at all happy,  9=don’t know, -1=Missing data");
							}
						}
						public static class Portuguese
						{
							public static class Text
							{
								public static IEnumerable<string[]> GeneralRegex => Enumerable.Empty<string[]>();
								public static char[] Trim => [];
							}
							public static class ValueLabels
							{
								public static IEnumerable<string[]> General => Enumerable.Empty<string[]>();
								public static IEnumerable<(string Regex, Func<MatchCollection, string, string> OnMatches)> GeneralRegex => Enumerable
									.Empty<(string Regex, Func<MatchCollection, string, string> OnMatches)>();
								public static IEnumerable<string> NotApplicables => Enumerable.Empty<string>();
							}
						}
					}

					public static string? Id(string? questionid, Languages language)
					{
						if (questionid is null)
							return null;

						if (questionid.SplitRemoveTrim(" ") is string[] questionidsplit && questionidsplit.Length > 1 && int.TryParse(questionidsplit[^1], out int _))
							questionid = string.Join(" ", questionidsplit[0..^1]);

						if (questionid.StartsWith("Q", StringComparison.OrdinalIgnoreCase) is false)
							questionid = string.Format("Q{0}", questionid);

						return questionid;
					}
					public static string? Text(string? questiontext, Languages language)
					{
						if (questiontext is null)
							return null;

						// 1: Trimming and Spelling Errors 
						// 2: Remove Question Prefix's [ 'Q23', 'Q23a', 'Q43b2', '23', '23a', 'Q43b2' ]
						// 3: Replace Country Names 

						questiontext = questiontext.ToLower().Trim(language switch
						{
							Languages.French => Replacements.French.Text.Trim,
							Languages.Portuguese => Replacements.Portuguese.Text.Trim,
							Languages.English or _ => Replacements.English.Text.Trim,
						});

						foreach (string[] _ValueLabelsGeneral in language switch
						{
							Languages.French => Replacements.French.Text.GeneralRegex,
							Languages.Portuguese => Replacements.Portuguese.Text.GeneralRegex,
							Languages.English or _ => Replacements.English.Text.GeneralRegex,

						}) questiontext = Regex.Replace(questiontext, _ValueLabelsGeneral[0], _ValueLabelsGeneral[1]);

						questiontext = _Base.Replacements._Country(questiontext, language);
						questiontext = _Base.Replacements._Nationality(questiontext, language);

						return questiontext.Trim(language switch
						{
							Languages.French => Replacements.French.Text.Trim,
							Languages.Portuguese => Replacements.Portuguese.Text.Trim,
							Languages.English or _ => Replacements.English.Text.Trim,
						});
					}
					public static string? VariableLabel(string? variablelabel, Languages language)
					{
						return variablelabel;
					}
					public static string[]? Values(string? values, Languages language)
					{
						return values?.Split(',');
					}
					public static string[]? ValueLabels(string? valuelabels, Languages language)
					{
						if (valuelabels is null)
							return null;

						foreach ((string _Regex, Func<MatchCollection, string, string> _OnMatches) in language switch
						{
							Languages.French => Replacements.French.ValueLabels.GeneralRegex,
							Languages.Portuguese => Replacements.Portuguese.ValueLabels.GeneralRegex,
							Languages.English or _ => Replacements.English.ValueLabels.GeneralRegex,
						
						}) valuelabels = _OnMatches(Regex.Matches(valuelabels, _Regex), valuelabels);

						foreach (string[] _ValueLabelsGeneral in language switch
						{
							Languages.French => Replacements.French.ValueLabels.General,
							Languages.Portuguese => Replacements.Portuguese.ValueLabels.General,
							Languages.English or _ => Replacements.English.ValueLabels.General,

						}) valuelabels = valuelabels.Replace(_ValueLabelsGeneral[0], _ValueLabelsGeneral[1]);

						string[] _valuelabels = valuelabels
							.Replace(';', ',')
							.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

						if (language switch
						{
							Languages.French => Replacements.French.ValueLabels.NotApplicables,
							Languages.Portuguese => Replacements.Portuguese.ValueLabels.NotApplicables,
							Languages.English or _ => Replacements.English.ValueLabels.NotApplicables,

						} is IEnumerable<string> _NotApplicables)
							for (int index = 0; index < _valuelabels.Length; index++)
								if (_NotApplicables.Contains(_valuelabels[index]) is false)
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
					public static string? Source(string? source, Languages language)
					{
						return source;
					}
					public static string? Note(string? note, Languages language)
					{
						return note;
					}

					public static TablesQuestion ToTablesQuestion(CodebookPDFQuestion question, Languages language, StreamWriter logger, out TablesVariable tablesvariable)
					{
						string? id = Utils.Inputs.CodebookPDF.Question.Id(question.Id, language);
						string? text = Utils.Inputs.CodebookPDF.Question.Text(question.Text, language);
						string? variablelabel = Utils.Inputs.CodebookPDF.Question.VariableLabel(question.VariableLabel, language);
						string[]? values = Utils.Inputs.CodebookPDF.Question.Values(question.Values, language);
						string[]? valuelabels = Utils.Inputs.CodebookPDF.Question.ValueLabels(question.ValueLabels, language);
						string? source = Utils.Inputs.CodebookPDF.Question.Source(question.Source, language);
						string? note = Utils.Inputs.CodebookPDF.Question.Note(question.Note, language);

						if (logger is not null)
						{
							logger.WriteLine("Id: '{0}' => '{1}'", question.Id, id);
							logger.WriteLine("Text: '{0}' => '{1}'", question.Text, text);
							logger.WriteLine("VariableLabel: '{0}' => '{1}'", question.VariableLabel, variablelabel);
							logger.WriteLine("Values:");
							logger.WriteLine("    '{0}'", question.Values);
							logger.WriteLine(" => ");
							logger.WriteLine("    '{0}'", question.Values, string.Join("\\n    ", values ?? []));
							logger.WriteLine("ValueLabels:");
							logger.WriteLine("    '{0}'", question.ValueLabels);
							logger.WriteLine(" => ");
							logger.WriteLine("    '{0}'", question.ValueLabels, string.Join("\\n    ", valuelabels ?? []));
							logger.WriteLine("Source: '{0}' => '{1}'", question.Source, source);
							logger.WriteLine("Note: '{0}' => '{1}'", question.Note, note);
						}

						tablesvariable = new TablesVariable
						{
							Label = variablelabel, 
							//ValueLabels = string.Join(",", valuelabels ?? []),
						};

						return new TablesQuestion
						{
							Id = id,
							Text = text,
							Source = source,
							Note = note,
						};
					}
				}
			}
		}
	}
}
