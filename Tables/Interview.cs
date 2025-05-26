using Database.Afrobarometer.Enums;

using System;
using System.IO;

namespace Database.Afrobarometer.Tables
{
	[SQLite.Table("interviews")]
    public class Interview : _AfrobarometerModel
    {
		[SQLite.Column(nameof(Language))] public string? Language { get; set; }
		[SQLite.Column(nameof(Round))] public string? Round { get; set; }
		[SQLite.Column(nameof(PkSurvey))] public int PkSurvey { get; set; }
		[SQLite.Column(nameof(List_PkVariable_Record))] public string? List_PkVariable_Record { get; set; }

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


		[SQLite.Table("interviews")]
		public class Individual : Interview
		{
			public Individual() { }
			public Individual(Interview interview)
			{
				Pk = interview.Pk;
				Language = interview.Language;
				Round = interview.Round;
				PkSurvey = interview.PkSurvey;
				List_PkVariable_Record = interview.List_PkVariable_Record;
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
		public static void Log(this StreamWriter streamwriter, Interview interview)
		{
			streamwriter.Log(interview as _AfrobarometerModel);
		}
		public static void LogError(this StreamWriter streamwriter, Interview interview)
		{
			streamwriter.LogError(interview as _AfrobarometerModel);
		}
	}
}