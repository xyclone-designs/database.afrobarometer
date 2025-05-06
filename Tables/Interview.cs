using Database.Afrobarometer.Enums;
using System.IO;

namespace Database.Afrobarometer.Tables
{
	[SQLite.Table("interviews")]
    public class Interview : _AfrobarometerModel
    {
		[SQLite.Column(nameof(Language))] public Languages? Language { get; set; }
		[SQLite.Column(nameof(Round))] public Rounds? Round { get; set; }
		[SQLite.Column(nameof(Pk_Survey))] public int Pk_Survey { get; set; }
		[SQLite.Column(nameof(List_PkVariables))] public string? List_PkVariables { get; set; }
		[SQLite.Column(nameof(List_PkQuestions_Record))] public string? List_PkQuestions_Record { get; set; }

		[SQLite.Table("interviews")]
		public class Individual : Interview
		{
			public Individual(Interview interview)
			{
				Language = interview.Language;
				Round = interview.Round;
				Pk_Survey = interview.Pk_Survey;
				List_PkVariables = interview.List_PkVariables;
				List_PkQuestions_Record = interview.List_PkQuestions_Record;
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