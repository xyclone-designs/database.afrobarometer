using Database.Afrobarometer.Enums;
using System.IO;

namespace Database.Afrobarometer.Tables
{
	[SQLite.Table("surveys")]
    public class Survey : _AfrobarometerModel
    {
		[SQLite.Column(nameof(Country))] public Countries? Country { get; set; }
		[SQLite.Column(nameof(ListPkQuestion))] public string? ListPkQuestion { get; set; }
		[SQLite.Column(nameof(Language))] public Languages? Language { get; set; }
		[SQLite.Column(nameof(Round))] public Rounds? Round { get; set; }

		[SQLite.Table("surveys")]
		public class Individual : Survey
		{
			public Individual(Survey survey)
			{
				Country = survey.Country;
				ListPkQuestion = survey.ListPkQuestion;
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

			streamwriter.WriteLine("Country: {0}", survey.Country);
			streamwriter.WriteLine("ListPkQuestion: {0}", survey.ListPkQuestion);
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