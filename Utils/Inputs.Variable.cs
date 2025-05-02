using System;
using System.Diagnostics.CodeAnalysis;
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
			public static class Variable
			{
				[StringSyntax("Regex")]
				public static readonly string Regex_Removals_Prefix = "^[Qq]?[0-9]+[A-Za-z_]?[0-9]?[.]?[\\s]+";
				
				public static readonly string[][] Labels_Replacements =  new string[][]
				{
					["healthcare", "health services"],
					["primary sampling unit", "PSU"],
					["Q90new Monthly Income", "Q90 New Monthly Income"],
				};
				public static readonly string[][] Labels_Substitutions = new string[][]
				{
					["Army to govern the country", "Army governs the country"],
					["What does democracy mean to you", "What does democracy mean"],
					["How interested is parliament in the well-being of you", "How interested is parliament in your well-being"],
					["How close to the party do you feel", "How close do you feel to this party"],
					["Have you heard about Economic Structural Adjustment Programe", "Have you heard about government's Economic Structural Adjustment Programme"],
					["Comparison of current and past government:interest in people", "Comparison of current and past government interest"],
					["Besides being [nationality] what other group do you feel close to", "Besides being [nationality], to which group do you feel you belong"],
					["Besides being [nationality], which group do you belong to", "Besides being [nationality], to which group do you feel you belong"],
					["Besides being citizen of [country], which group do you feel you belong to", "Besides being [nationality], to which group do you feel you belong"],
					["Army governed the country", "Army to govern the country"],
					["Army governs country", "Army to govern the country"],
					["Any stop/give way signs in last 10km of journey", "Any stop lights / give way signs along last 10km of journey"],
					["Any traffic lights in last 10km of journey", "Any traffic lights or robots along last 10km of journey"],
					["Are there any townhalls or community buildings that can be used for meetings", "Are there any townhalls or community buildings"],
					["Have to be careful what you do and say with regards to politics in this country", "Have to be careful what you do and say with regards to politics"],
					["How much of the time can you trust the President to do what is right", "How much of the time can you trust the President"],
					["How often have you felt unsafe from crime", "How often have you felt unsafe from crime in your home"],
					["Is your job part-time or full-time; looking for work", "Is your job part-time or full-time or are you looking for work"],
					["One party to stand for elections and to stand for office.", "One party to stand for elections and hold office"],
					["People Vs Govt Responsible for well-being", "People responsible for well-being VS government responsible"],
					["People responsible for selves VS Govt. responsible for people's well-being", "People responsible for well-being VS government responsible"],
					["Preference for democratic government vs non-democratic government", "Preference for democratic vs non-democratic government"],
					["Protecting nation's borders", "Protecting Borders"],
					["Trust National Assembly (Parliament)", "Trust National Assembly"],
					["Trust trade/farmer unions", "Trust trade unions/farmers organizations"],
					["What other methods used to obtain food", "What other methods would you use to obtain food"],
					["Which group interest does government mostly represent", "Which group interest is mostly represented by government"],
					["Without schooling", "Without School"],
				};
				public static readonly string[][] Labels_Substitutions_StartsWith = new string[][]
				{
					["All decisions made by economic experts", "All decisions made by economic experts rather than government"],
					["Free to choose who to vote for", "Free to choose who to vote for without feeling forced by others"],
					["Do you have an electricity hook-up", "Do you have an electricity hook-up into home"],
					["Desirable to create a united", "Desirable to create a united [nationality] nation out of all groups"],
					["Democratic society and small income gap", "Democratic society and small income gap between rich and poor"],
					["Constitution expresses", "Constitution expresses values and aspirations of [nationality]"],
					["At least two political parties compet", "At least two political parties competing"],
					["Approached by community", "Approached by community/political representatives"],
					["Govt should have the", "Govt should have the ultimate decision"],
					["How much of the last 10km journey ", "How much of the last 10km "],
					["How well does government deliver basic services like", "How well does government deliver basic services"],
					["Most people can be trusted", "Most people can be trusted vs you must be careful"],
					["Turn schooling", "Turn school"],
					["Way you vote", "Way you vote will make a difference VS it won't make a difference"],
					["What was respondent's attitude", "What was respondent's attitude during interview"],
					["Who is in power makes a difference", "Who is in power makes a difference vs it doesn't matter"],
				};
				public static readonly string[][] Value_Labels_Replacements = new string[][]
				{
					["About the same", "Same"],
					["Don't", "Do not"],
					["Couldn't tell", "Can't Determine"],
					["Just once or twice", "Once or twice"],
					["Yes, often", "Often"],
					["Yes, a few times", "A few times"],
					["Yes, once or twice", "Once or twice"],
					["No, would never do this", "Would never do this"],
					["No, but would do it if had the chance", "Would do it if I had the chance"],
					["No, but would do it if I had the chance", "Would do it if I had the chance"],
				};
				public static readonly string[][] Value_Labels_Substitutions_StartsWith = new string[][]
				{
					["Missing", "Missing Data"]
				};

				public static string CleanLabel(string label)
				{
					for (int index = 0; index < Labels_Replacements.Length; index++)
						label = label.Replace(Labels_Replacements[index][0], Labels_Replacements[index][1]);

					label = Regex
						.Replace(label, Regex_Removals_Prefix, string.Empty)
						.Replace("?", string.Empty)
						.Replace(".", string.Empty)
						.Trim();

					for (int index = 1; index < ReplacementsEnglish_Nationality.Length; index++)
						label = label.Replace(ReplacementsEnglish_Nationality[index], ReplacementsEnglish_Nationality[0], StringComparison.OrdinalIgnoreCase);

					for (int index = 1; index < ReplacementsEnglish_Country.Length; index++)
						label = label.Replace(ReplacementsEnglish_Country[index], ReplacementsEnglish_Country[0], StringComparison.OrdinalIgnoreCase);
					
					for (int index = 1; index < ReplacementsEnglish.Length; index++)
						label = label.Replace(ReplacementsEnglish[index][0], ReplacementsEnglish[index][1]);

					for (int index = 1; index < Labels_Substitutions.Length; index++)
						if (string.Equals(label, Labels_Substitutions[index][0], StringComparison.OrdinalIgnoreCase))
							label = Labels_Substitutions[index][1];

					for (int index = 1; index < Labels_Substitutions_StartsWith.Length; index++)
						if (label.StartsWith(Labels_Substitutions_StartsWith[index][0], StringComparison.OrdinalIgnoreCase))
							label = Labels_Substitutions_StartsWith[index][1];

					return label;
				}
				public static string CleanValueLabel(string valuelabel)
				{
					if (valuelabel.StartsWith("Don't"))
					{ }

					valuelabel = valuelabel.Trim(',', ' ');

					for (int index = 0; index < Value_Labels_Replacements.Length; index++)
						valuelabel = valuelabel.Replace(Value_Labels_Replacements[index][0], Value_Labels_Replacements[index][1]);

					for (int index = 0; index < Value_Labels_Substitutions_StartsWith.Length; index++)
						if (valuelabel.StartsWith(Value_Labels_Substitutions_StartsWith[index][0]))
							valuelabel = Value_Labels_Substitutions_StartsWith[index][1];

					return valuelabel;
				}

				public static TablesVariable CleanNew(SpsslyVariable spsslyvariable, StreamWriter logger)
				{
					string cleanedlabel = CleanLabel(spsslyvariable.Label);

					logger.WriteLine("Name: {0}", spsslyvariable.Name);
					logger.WriteLine("Label: '{1}' => {0}", spsslyvariable.Label, cleanedlabel == spsslyvariable.Label ? "::" : '+' + cleanedlabel + '+');

					return new TablesVariable(spsslyvariable)
					{
						Label = cleanedlabel,
						ValueLabelsDictionary = spsslyvariable.ValueLabels
							.ToDictionary(_ => _.Key, _ =>
							{
								string cleanvalue = CleanValueLabel(_.Value);

								logger.WriteLine("ValueLabel: {0} => {1}", _.Value, cleanvalue);

								return cleanvalue;
							})
					};
				}
			}
		}
	}
}
