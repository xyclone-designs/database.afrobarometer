using Database.Afrobarometer.Enums;
using Database.Afrobarometer.Inputs;
using System.IO;

namespace Database.Afrobarometer.Tables
{
	[SQLite.Table("questions")]
    public class Question : _AfrobarometerModel
    {
		public Question() { }
		public Question(CodebookPDF.Question question)
		{
			Id = question.Id;
			Text = question.Text;
			Source = question.Source;
			Note = question.Note;
		}

		[SQLite.Column(nameof(Language))] public Languages? Language { get; set; }
		[SQLite.Column(nameof(Round))] public Rounds? Round { get; set; }
		[SQLite.Column(nameof(ListPkSurvey))] public string? ListPkSurvey { get; set; }
		[SQLite.Column(nameof(Id))] public string? Id { get; set; }
		[SQLite.Column(nameof(PkVariable))] public int? PkVariable { get; set; }
		[SQLite.Column(nameof(Text))] public string? Text { get; set; }
		[SQLite.Column(nameof(Source))] public string? Source { get; set; }
		[SQLite.Column(nameof(Note))] public string? Note { get; set; }

		[SQLite.Table("questions")]
		public class Individual : Question
		{
			public Individual(Question question)
			{
				Language = question.Language;
				Round = question.Round;
				Id = question.Id;
				Id = question.Id;
				PkVariable = question.PkVariable;
				Text = question.Text;
				Source = question.Source;
				Note = question.Note;
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
		public static void Log(this StreamWriter streamwriter, Question question)
		{
			streamwriter.Log(question as _AfrobarometerModel);

			streamwriter.WriteLine("Id: {0}", question.Id);
			streamwriter.WriteLine("Text: {0}", question.Text);
			streamwriter.WriteLine("Language: {0}", question.Language);
			streamwriter.WriteLine("Round: {0}", question.Round);
			streamwriter.WriteLine("PkVariable: {0}", question.PkVariable);
			streamwriter.WriteLine("Source: {0}", question.Source);
			streamwriter.WriteLine("Note: {0}", question.Note);
			streamwriter.WriteLine();
		}
		public static void LogError(this StreamWriter streamwriter, Question question)
		{
			streamwriter.LogError(question as _AfrobarometerModel);
		}
	}
}