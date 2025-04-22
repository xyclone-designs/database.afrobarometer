using System.Linq;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics.CodeAnalysis;

namespace Database.Afrobarometer.Tables
{
	[SQLite.Table("questions")]
    public class Question : _AfrobarometerModel
    {
		[StringSyntax("Regex")]
		public static readonly string RegexQuestionText = "^[qQ]?[0-9][0-9]?[0-9]?[a-zA-Z]?[0-9]?.?";

		public static readonly char[] TrimQuestionText = ['?', ' ', '.'];
		public static readonly char[] TrimQuestionTexts = [',', '.'];
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
			string[] values = texts.ElementAtOrDefault(3)?.Split(TrimQuestionTexts, StringSplitOptions.TrimEntries) ?? [];
			string[] valuelables = texts.ElementAtOrDefault(4)?
				.Replace(';', ',')
				//.Replace(", 1-", ", 1=")
				//.Replace(", 6-", ", 6=")
				.Replace("service) 9=Don’t", "service), 9=Don’t")
				.Replace("lot, =Don’t know", "lot, 5=Don’t know")
				.Replace("officials, =Govt,", "officials, 58=Govt")
				.Replace("Know 6=Don’t", "Know, 6=Don’t")
				.Replace("war 13=Death", "war, 13=Death")
				.Replace("Know98=Refused", "Know, 98=Refused")
				.Replace("answer 97=No", "answer, 97=No")
				.Replace("non=permanent", "non-permanent")
				.Replace("110=Khasonke 111=Bozo", "110=Khasonke, 111=Bozo")
				.Replace("Samaritan 28=Patriotic", "Samaritan, 128=Patriotic")
				.Replace("Other, =Nothing/No", "Other, 38=Nothing/No")
				.Replace("0=No Answer ", "0=No Answer,")
				.Replace("Church, =Social", "Church, 35=Social")
				.Replace("Immigration 16=Private", "Immigration, 16=Private")
				.Replace("me, =Looking", "me, 30=Looking")
				.Replace(": 0=Worst form", "0=Worst form")
				.Replace("405=Refengkgotso, =Reservoir", "405=Refengkgotso, 406=Reservoir")
				.Replace("lives, =Disruption", "lives, 144=Disruption")
				.Replace("data, =Senior", "data, 100=Senior")
				.Replace("house 3=Temporary", "house ,3=Temporary")
				.Replace("worship, =Employment", "worship, 65=Employment")
				.Replace("assistance, =Leaders", "assistance, 154=Leaders")
				.Replace("job, =Nothing", "job, 31=Nothing")
				.Replace("respect, 211-Economic", "respect, 211=Economic")
				.Replace("Sotho 15=Setswana,", "Sotho, 15=Setswana,")
				.Replace("prices, =Other ", "prices, 35=Other ")
				.Replace("28=Education (Equality) 29=Education (too expensive)", "28=Education (Equality), 29=Education (too expensive)")
				.Split(TrimQuestionTexts, StringSplitOptions.TrimEntries) ?? [];

			for (int index = 0; index < valuelables.Length; index++)
			{
				string[] split = valuelables[index].Split('=');

				if (Regex.IsMatch(valuelables[index], "[A-Za-z][A-Za-z]?=[0-9][0-9]?"))
					valuelables[index] = string.Format("{0}={1}", split[1], split[0]);

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
			bool error = base.LogErrors(streamwriter);

			if (ValueLabels is not null)
				foreach (string valuelabel in ValueLabels)
					if (valuelabel.Count(_ => '=' == _) > 1)
					{
						error = true;

						streamwriter.WriteLine("ValueLabels: {0}", ValueLabels?.ElementAtOrDefault(0));

						if (ValueLabels is not null && ValueLabels.Length > 2)
							for (int index = 1; index < ValueLabels.Length; index++)
								streamwriter.WriteLine("             {0}", ValueLabels[index]);
					}

			if (error)
				streamwriter.WriteLine();

			return error;
		}
	}
}