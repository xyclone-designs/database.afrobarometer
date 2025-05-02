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
			Number = question.Number;
			Text = question.Text;
			Source = question.Source;
			Note = question.Note;
		}

		[SQLite.Column(nameof(Language))] public Languages? Language { get; set; }
		[SQLite.Column(nameof(Round))] public Rounds? Round { get; set; }
		[SQLite.Column(nameof(ListPkSurvey))] public string? ListPkSurvey { get; set; }
		[SQLite.Column(nameof(Number))] public string? Number { get; set; }
		[SQLite.Column(nameof(PkVariable))] public int? PkVariable { get; set; }
		[SQLite.Column(nameof(Text))] public string? Text { get; set; }
		[SQLite.Column(nameof(Source))] public string? Source { get; set; }
		[SQLite.Column(nameof(Note))] public string? Note { get; set; }

		public override void Log(StreamWriter streamwriter)
		{
			base.Log(streamwriter);

			streamwriter.WriteLine("Number: {0}", Number);
			streamwriter.WriteLine("Text: {0}", Text);
			streamwriter.WriteLine("Language: {0}", Language);
			streamwriter.WriteLine("Round: {0}", Round);
			streamwriter.WriteLine("PkVariable: {0}", PkVariable);
			streamwriter.WriteLine("Source: {0}", Source);
			streamwriter.WriteLine("Note: {0}", Note);
			streamwriter.WriteLine();
		}
	}
}