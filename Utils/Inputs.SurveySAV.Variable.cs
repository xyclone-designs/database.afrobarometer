using Database.Afrobarometer.Enums;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using TablesVariable = Database.Afrobarometer.Tables.Variable;
using SpsslyVariable = Spssly.SpssDataset.Variable;

namespace Database.Afrobarometer
{
	public static partial class Utils
	{
		public static partial class Inputs
		{
			public static partial class SurveySAV
			{
				public static class Variable
				{
					public static class Replacements
					{
						public static string _LabelGeneral(string input, Languages language)
						{
							foreach (string[] _General in language switch
							{
								Languages.French => French.Label.General,
								Languages.Portuguese => Portuguese.Label.General,
								Languages.English or _ => English.Label.General,

							}) input = input.Replace(_General[0], _General[1]);

							return input;
						}
						public static string _LabelGeneralRegex(string input, Languages language)
						{
							foreach (string[] _General in language switch
							{
								Languages.French => French.Label.GeneralRegex,
								Languages.Portuguese => Portuguese.Label.GeneralRegex,
								Languages.English or _ => English.Label.GeneralRegex,

							}) input = Regex.Replace(input, _General[0], _General[1]);

							return input;
						}
						public static string _ValueLabelsGeneral(string input, Languages language)
						{
							foreach (string[] _General in language switch
							{
								Languages.French => French.ValueLabels.General,
								Languages.Portuguese => Portuguese.ValueLabels.General,
								Languages.English or _ => English.ValueLabels.General,

							}) input = input.Replace(_General[0], _General[1]);

							return input;
						}

						public static class French
						{
							public static class Label
							{
								public static IEnumerable<string[]> General => Enumerable.Empty<string[]>();
								public static IEnumerable<string[]> GeneralRegex => Enumerable.Empty<string[]>();
							}
							public static class ValueLabels
							{
								public static IEnumerable<string[]> General => Enumerable.Empty<string[]>();
							}
						}
						public static class English
						{
							public static class Label
							{
								public static IEnumerable<string[]> General => _Base.Replacements.English.General
									.Append(["healthcare", "health services"])
									.Append(["primary sampling unit", "PSU"])
									.Append(["Q90new Monthly Income", "Q90 New Monthly Income"])
									.Append(["?", string.Empty])
									.Append([".", string.Empty]);
								public static IEnumerable<string[]> GeneralRegex => Enumerable.Empty<string[]>()
									.Append(["^[Qq]?[0-9]+[A-Za-z_]?[0-9]?[.]?[\\s]+", string.Empty]);
							}
							public static class ValueLabels
							{
								public static IEnumerable<string[]> General => Enumerable.Empty<string[]>()
									.Append(["About the same", "Same"])
									.Append(["Don't", "Do not"])
									.Append(["Couldn't tell", "Can't Determine"])
									.Append(["Just once or twice", "Once or twice"])
									.Append(["Yes, often", "Often"])
									.Append(["Yes, a few times", "A few times"])
									.Append(["Yes, once or twice", "Once or twice"])
									.Append(["No, would never do this", "Would never do this"])
									.Append(["No, but would do it if had the chance", "Would do it if I had the chance"])
									.Append(["No, but would do it if I had the chance", "Would do it if I had the chance"]);

							}
						}
						public static class Portuguese
						{
							public static class Label
							{
								public static IEnumerable<string[]> General => Enumerable.Empty<string[]>();
								public static IEnumerable<string[]> GeneralRegex => Enumerable.Empty<string[]>();
							}
							public static class ValueLabels
							{
								public static IEnumerable<string[]> General => Enumerable.Empty<string[]>();
							}
						}
					}
					public static class Substitutions
					{
						public static string _LabelGeneral(string input, Languages language)
						{
							foreach (string[] _General in language switch
							{
								Languages.French => French.Label.General,
								Languages.Portuguese => Portuguese.Label.General,
								Languages.English or _ => English.Label.General,

							}) if (input == _General[0]) input = input = _General[1];

							return input;
						}
						public static string _LabelGeneralStartsWith(string input, Languages language)
						{
							foreach (string[] _GeneralStartsWith in language switch
							{
								Languages.French => French.Label.GeneralStartsWith,
								Languages.Portuguese => Portuguese.Label.GeneralStartsWith,
								Languages.English or _ => English.Label.GeneralStartsWith,

							}) if (input.StartsWith(_GeneralStartsWith[0])) input = input = _GeneralStartsWith[1];

							return input;
						}

						public static string _ValueLabelsGeneral(string input, Languages language)
						{
							foreach (string[] _General in language switch
							{
								Languages.French => French.ValueLabels.General,
								Languages.Portuguese => Portuguese.ValueLabels.General,
								Languages.English or _ => English.ValueLabels.General,

							}) if (input == _General[0]) input = input = _General[1];

							return input;
						}
						public static string _ValueLabelsGeneralStartsWith(string input, Languages language)
						{
							foreach (string[] _GeneralStartsWith in language switch
							{
								Languages.French => French.ValueLabels.GeneralStartsWith,
								Languages.Portuguese => Portuguese.ValueLabels.GeneralStartsWith,
								Languages.English or _ => English.ValueLabels.GeneralStartsWith,

							}) if (input.StartsWith(_GeneralStartsWith[0])) input = input = _GeneralStartsWith[1];

							return input;
						}

						public static partial class French
						{
							public static class Label
							{
								public static IEnumerable<string[]> General => Enumerable.Empty<string[]>();
								public static IEnumerable<string[]> GeneralStartsWith => Enumerable.Empty<string[]>();
							}
							public static class ValueLabels
							{
								public static IEnumerable<string[]> General => Enumerable.Empty<string[]>();
								public static IEnumerable<string[]> GeneralStartsWith => Enumerable.Empty<string[]>();
							}
						}
						public static partial class English
						{
							public static class Label
							{
								public static IEnumerable<string[]> General => Enumerable.Empty<string[]>()
									.Append(["Army to govern the country", "Army governs the country"])
									.Append(["What does democracy mean to you", "What does democracy mean"])
									.Append(["How interested is parliament in the well-being of you", "How interested is parliament in your well-being"])
									.Append(["How close to the party do you feel", "How close do you feel to this party"])
									.Append(["Have you heard about Economic Structural Adjustment Programe", "Have you heard about government's Economic Structural Adjustment Programme"])
									.Append(["Comparison of current and past government:interest in people", "Comparison of current and past government interest"])
									.Append(["Besides being [nationality] what other group do you feel close to", "Besides being [nationality], to which group do you feel you belong"])
									.Append(["Besides being [nationality], which group do you belong to", "Besides being [nationality], to which group do you feel you belong"])
									.Append(["Besides being citizen of [country], which group do you feel you belong to", "Besides being [nationality], to which group do you feel you belong"])
									.Append(["Army governed the country", "Army to govern the country"])
									.Append(["Army governs country", "Army to govern the country"])
									.Append(["Any stop/give way signs in last 10km of journey", "Any stop lights / give way signs along last 10km of journey"])
									.Append(["Any traffic lights in last 10km of journey", "Any traffic lights or robots along last 10km of journey"])
									.Append(["Are there any townhalls or community buildings that can be used for meetings", "Are there any townhalls or community buildings"])
									.Append(["Have to be careful what you do and say with regards to politics in this country", "Have to be careful what you do and say with regards to politics"])
									.Append(["How much of the time can you trust the President to do what is right", "How much of the time can you trust the President"])
									.Append(["How often have you felt unsafe from crime", "How often have you felt unsafe from crime in your home"])
									.Append(["Is your job part-time or full-time; looking for work", "Is your job part-time or full-time or are you looking for work"])
									.Append(["One party to stand for elections and to stand for office.", "One party to stand for elections and hold office"])
									.Append(["People Vs Govt Responsible for well-being", "People responsible for well-being VS government responsible"])
									.Append(["People responsible for selves VS Govt. responsible for people's well-being", "People responsible for well-being VS government responsible"])
									.Append(["Preference for democratic government vs non-democratic government", "Preference for democratic vs non-democratic government"])
									.Append(["Protecting nation's borders", "Protecting Borders"])
									.Append(["Trust National Assembly (Parliament)", "Trust National Assembly"])
									.Append(["Trust trade/farmer unions", "Trust trade unions/farmers organizations"])
									.Append(["What other methods used to obtain food", "What other methods would you use to obtain food"])
									.Append(["Which group interest does government mostly represent", "Which group interest is mostly represented by government"])
									.Append(["Without schooling", "Without School"]);
								public static IEnumerable<string[]> GeneralStartsWith => Enumerable.Empty<string[]>()
									.Append(["All decisions made by economic experts", "All decisions made by economic experts rather than government"])
									.Append(["Free to choose who to vote for", "Free to choose who to vote for without feeling forced by others"])
									.Append(["Do you have an electricity hook-up", "Do you have an electricity hook-up into home"])
									.Append(["Desirable to create a united", "Desirable to create a united [nationality] nation out of all groups"])
									.Append(["Democratic society and small income gap", "Democratic society and small income gap between rich and poor"])
									.Append(["Constitution expresses", "Constitution expresses values and aspirations of [nationality]"])
									.Append(["At least two political parties compet", "At least two political parties competing"])
									.Append(["Approached by community", "Approached by community/political representatives"])
									.Append(["Govt should have the", "Govt should have the ultimate decision"])
									.Append(["How much of the last 10km journey ", "How much of the last 10km "])
									.Append(["How well does government deliver basic services like", "How well does government deliver basic services"])
									.Append(["Most people can be trusted", "Most people can be trusted vs you must be careful"])
									.Append(["Turn schooling", "Turn school"])
									.Append(["Way you vote", "Way you vote will make a difference VS it won't make a difference"])
									.Append(["What was respondent's attitude", "What was respondent's attitude during interview"])
									.Append(["Who is in power makes a difference", "Who is in power makes a difference vs it doesn't matter"]);
							}
							public static class ValueLabels
							{
								public static IEnumerable<string[]> General => Enumerable.Empty<string[]>();
								public static IEnumerable<string[]> GeneralStartsWith => Enumerable.Empty<string[]>();
							}
						}
						public static partial class Portuguese
						{
							public static class Label
							{
								public static IEnumerable<string[]> General => Enumerable.Empty<string[]>();
								public static IEnumerable<string[]> GeneralStartsWith => Enumerable.Empty<string[]>();
							}
							public static class ValueLabels
							{
								public static IEnumerable<string[]> General => Enumerable.Empty<string[]>();
								public static IEnumerable<string[]> GeneralStartsWith => Enumerable.Empty<string[]>();
							}
						}
					}

					public static string CleanLabel(string label, Languages language)
					{
						label = Replacements._LabelGeneral(label, language);
						label = Replacements._LabelGeneralRegex(label, language);
						label = _Base.Replacements._Nationality(label, language);
						label = _Base.Replacements._Country(label, language);
						label = Substitutions._LabelGeneral(label, language);
						label = Substitutions._LabelGeneralStartsWith(label, language);
						
						return label.Trim();
					}
					public static string CleanValueLabel(string valuelabel, Languages language)
					{
						valuelabel = valuelabel.Trim(',', ' ');
						valuelabel = Replacements._ValueLabelsGeneral(valuelabel, language);
						valuelabel = Substitutions._ValueLabelsGeneral(valuelabel, language);
						valuelabel = Substitutions._ValueLabelsGeneralStartsWith(valuelabel, language);

						return valuelabel;
					}

					public static TablesVariable ToTableVariable(SpsslyVariable spsslyvariable, Languages language, StreamWriter logger)
					{
						string cleanedlabel = CleanLabel(spsslyvariable.Label, language);

						logger.WriteLine("Name: {0}", spsslyvariable.Name);
						logger.WriteLine("Label: '{0}' => {1}", spsslyvariable.Label, cleanedlabel == spsslyvariable.Label ? "::" : cleanedlabel);

						return new TablesVariable(spsslyvariable)
						{
							Id = _Base.Replacements._Id(cleanedlabel, language),
							Label = cleanedlabel,
							ValueLabelsDictionary = spsslyvariable.ValueLabels
								.ToDictionary(_ => _.Key, _ =>
								{
									string cleanvalue = CleanValueLabel(_.Value, language);

									logger.WriteLine("Value Label: {0} => {1}", _.Value, cleanvalue);

									return cleanvalue;
								})
						};
					}
				}
			}
		}
	}
}
