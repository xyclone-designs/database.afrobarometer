
namespace Database.Afrobarometer.Tables.Individual
{
	[SQLite.Table("questions")]
    public class QuestionIndividual : Question
	{
		public QuestionIndividual(Question question)
		{
			Language = question.Language;
			Round = question.Round;
			Number = question.Number;
			Number = question.Number;
			PkVariable = question.PkVariable;
			Text = question.Text;
			Source = question.Source;
			Note = question.Note;
		}

		[SQLite.Column(nameof(Pk))] public new int Pk { get; set; }
	}
}