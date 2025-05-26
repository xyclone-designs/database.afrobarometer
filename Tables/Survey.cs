using Database.Afrobarometer.Enums;

using System;
using System.IO;

namespace Database.Afrobarometer.Tables
{
	[SQLite.Table("surveys")]
    public class Survey : _AfrobarometerModel
    {
		[SQLite.Column(nameof(CountryCode))] public string? CountryCode { get; set; }
		[SQLite.Column(nameof(List_PkQuestion))] public string? List_PkQuestion { get; set; }
		[SQLite.Column(nameof(List_PkVariable))] public string? List_PkVariable { get; set; }
		[SQLite.Column(nameof(InterviewCount))] public int? InterviewCount { get; set; }
		[SQLite.Column(nameof(Language))] public string? Language { get; set; }
		[SQLite.Column(nameof(Round))] public string? Round { get; set; }

		[SQLite.Ignore] public Languages? _Language
		{
			set => Language = value?.ToString();
			get => Enum.TryParse(Language, out Languages _language) ? _language : null;
		}
		[SQLite.Ignore] public Rounds? _Round
		{
			get => Round?.AsRound();
			set => Round = value?.AsString(); 
		}


		[SQLite.Table("surveys")]
		public class Individual : Survey
		{
			public Individual() { }
			public Individual(Survey survey)
			{
				Pk = survey.Pk;
				CountryCode = survey.CountryCode;
				List_PkQuestion = survey.List_PkQuestion;
				List_PkVariable = survey.List_PkVariable;
				InterviewCount = survey.InterviewCount;
				Language = survey.Language;
				Round = survey.Round;
			}

			[SQLite.AutoIncrement]
			[SQLite.NotNull]
			[SQLite.PrimaryKey]
			[SQLite.Unique]
			public new int Pk { get; set; }
		}
	}

	public static partial class StreamWriterExtensions
	{
		public static void Log(this StreamWriter streamwriter, Survey survey)
		{
			streamwriter.Log(survey as _AfrobarometerModel);

			streamwriter.WriteLine("CountryCode: {0}", survey.CountryCode);
			streamwriter.WriteLine("List_PkQuestion: {0}", survey.List_PkQuestion);
			streamwriter.WriteLine("List_PkVariable: {0}", survey.List_PkVariable);
			streamwriter.WriteLine("InterviewCount: {0}", survey.InterviewCount);
			streamwriter.WriteLine("Language: {0}", survey.Language);
			streamwriter.WriteLine("Round: {0}", survey.Round);
			streamwriter.WriteLine();
		}
		public static void LogError(this StreamWriter streamwriter, Survey survey)
		{
			streamwriter.LogError(survey as _AfrobarometerModel);
		}
	}
}