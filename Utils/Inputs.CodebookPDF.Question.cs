using Database.Afrobarometer.Enums;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
					public static partial class Substitutions
					{
						public static partial class French
						{
							public static partial class ValueLabels
							{
								public static IEnumerable<string[]> General => Enumerable.Empty<string[]>();
								public static IEnumerable<string[]> GeneralStartsWith => Enumerable.Empty<string[]>();
							}
						}
						public static partial class English
						{
							public static partial class ValueLabels
							{
								public static IEnumerable<string[]> General => Enumerable.Empty<string[]>()
									.Append(["=Strongly agree", "1=Strongly agree"]);
								public static IEnumerable<string[]> GeneralStartsWith => Enumerable.Empty<string[]>();
							}
						}
						public static partial class Portuguese
						{
							public static partial class ValueLabels
							{
								public static IEnumerable<string[]> General => Enumerable.Empty<string[]>();
								public static IEnumerable<string[]> GeneralStartsWith => Enumerable.Empty<string[]>();
							}
						}
					}
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
									.Append(["1=1=Don’t", "1=Don’t"])
									.Append(["1=,1=Don’t", "1=Don’t"])
									.Append(["518 =Sonrhai, =Makua", "518 =Sonrhai, 540=Makua"])
									.Append(["55=Know someone, government welfare", "55=Know someone or government welfare"])
									.Append(["81=Would die,4=Sale of food", "81=Would die,84=Sale of food"])
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
					public static string[]? Values(string? values, Languages language, out bool changed)
					{
						changed = false;

						return values?.Split(',');
					}
					public static string[]? ValueLabels(string? valuelabels, Languages language, out bool changed)
					{
						changed = false;

						if (valuelabels is null)
							return null;

						string original = valuelabels;

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
						
						foreach (string[] _ValueLabelsGeneral in language switch
						{
							Languages.French => Replacements.French.ValueLabels.General,
							Languages.Portuguese => Replacements.Portuguese.ValueLabels.General,
							Languages.English or _ => Replacements.English.ValueLabels.General,

						}) valuelabels = valuelabels.Replace(_ValueLabelsGeneral[0], _ValueLabelsGeneral[1]);

						changed = changed || original == valuelabels;

						string[] _valuelabels = valuelabels
							.Trim(':')
							.Replace(';', ',')
							.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
							.Select((_, __) => _.Contains('=') ? _ : string.Format("{0}={1}", -(__ + 1), _))
							.ToArray();

						if (language switch
						{
							Languages.French => Replacements.French.ValueLabels.NotApplicables,
							Languages.Portuguese => Replacements.Portuguese.ValueLabels.NotApplicables,
							Languages.English or _ => Replacements.English.ValueLabels.NotApplicables,

						} is IEnumerable<string> _NotApplicables)
							for (int index = 0; index < _valuelabels.Length; index++)
								if (_NotApplicables.Contains(_valuelabels[index]) is false)
								{
									foreach (string[] _ValueLabelsGeneral in language switch
									{
										Languages.French => Substitutions.French.ValueLabels.General,
										Languages.Portuguese => Substitutions.Portuguese.ValueLabels.General,
										Languages.English or _ => Substitutions.English.ValueLabels.General,

									}) if (_valuelabels[index] == _ValueLabelsGeneral[0]) _valuelabels[index] = _ValueLabelsGeneral[1];

									string[] split = _valuelabels[index].SplitRemoveTrim('=', '"');

									if (index > 0 && int.TryParse(split.ElementAtOrDefault(0), out int _) is false)
									{
										changed = true;

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

					public static bool NotApplicables(string? str, Languages language)
					{
						if (str is null)
							return false;

						return (language switch
						{
							Languages.French => Replacements.French.ValueLabels.NotApplicables,
							Languages.Portuguese => Replacements.Portuguese.ValueLabels.NotApplicables,
							Languages.English or _ => Replacements.English.ValueLabels.NotApplicables,

						}).Contains(str);
					}

					public static string[] SplitValueLabels(string? valuelabels, Languages language)
					{
						if (valuelabels is null)
							return [];

						return SplitValueLabels(valuelabels.SplitRemoveTrim(','), language);
					}
					public static string[] SplitValueLabels(string[]? valuelabels, Languages language)
					{
						if (valuelabels is null)
							return [];

						for (int index = 0; index < valuelabels.Length; index++)
							if (NotApplicables(valuelabels[index], language) is false)
							{
								string[] split = valuelabels[index].SplitRemoveTrim('=', '"');

								if (index > 0 && int.TryParse(split.ElementAtOrDefault(0), out int _) is false)
								{
									valuelabels[index - 1] = string.Format("{0}, {1}", valuelabels[index - 1], valuelabels[index]);
									valuelabels = valuelabels
										.Where((_, __) => index != __)
										.ToArray();
									index--;
								}
							}

						return valuelabels;
					}

					public static TablesQuestion ToTablesQuestion(CodebookPDFQuestion question, Languages language, out TablesVariable tablesvariable, params StreamWriter[] loggers)
					{
						string? id = Utils.Inputs.CodebookPDF.Question.Id(question.Id, language);
						string? text = Utils.Inputs.CodebookPDF.Question.Text(question.Text, language);
						string? variablelabel = Utils.Inputs.CodebookPDF.Question.VariableLabel(question.VariableLabel, language);
						string? variablecleanlabel = variablelabel is null ? null : Utils.Inputs.SurveySAV.Variable.CleanLabel(variablelabel, language);
						string? variableid = variablecleanlabel is null ? null : Utils.Inputs._Base.Replacements._Id(variablecleanlabel, language);
						string[]? values = Utils.Inputs.CodebookPDF.Question.Values(question.Values, language, out bool valueschanged);
						string[]? valuelabels = Utils.Inputs.CodebookPDF.Question.ValueLabels(question.ValueLabels, language, out bool valueslabelschanged);
						string? source = Utils.Inputs.CodebookPDF.Question.Source(question.Source, language);
						string? note = Utils.Inputs.CodebookPDF.Question.Note(question.Note, language);

						IList<string> logs = [];

						if (question.Id != id) logs.AddFormat("Id: '{0}' => '{1}'", question.Id, id);
						if (question.Text != text) logs.AddFormat("Text: '{0}'\n      '{1}'", question.Text, text);
						if (question.VariableLabel != variablelabel) logs.AddFormat("VariableLabel: '{0}'\n               '{1}'", question.VariableLabel, variablelabel);
						if (valueschanged) logs.AddFormat("Values:\n    '{0}'\n    =>\n    '{1}'", question.Values, string.Join("\n    ", values ?? []));
						if (valuelabels?.Select(_ => double.Parse(_.Split('=')[0])) is IEnumerable<double> all && all.Distinct().Count() == all.Count())
							logs.AddFormat("ValueLabels: Duplicates Found '{0}'", string.Join(", ", all));
						if (valueslabelschanged) logs.AddFormat("ValueLabels: {0}\n             {1}", question.ValueLabels, string.Join("\n             ", valuelabels ?? []));
						if (question.Source != source) logs.AddFormat("Source: '{0}'\n        '{1}'", question.Source, source);
						if (question.Note != note) logs.AddFormat("Note: '{0}'\n      '{1}'", question.Note, note);

						foreach (StreamWriter logger in loggers)
						{
							foreach (string log in logs)
								logger.WriteLine(log); logger.WriteLine();
						}

						tablesvariable = new TablesVariable
						{
							Id = variableid,
							Label = variablecleanlabel ?? variablelabel, 
							ValueLabels = string.Join(",", valuelabels ?? []),
						};

						return new TablesQuestion
						{
							Id = id,
							Text = text,
							Source = source,
							VariableLabel = variablecleanlabel ?? variablelabel,
							Note = note,
							_Language = language,
						};
					}
				}
			}
		}
	}
}
