
namespace Database.Afrobarometer.Tables.Individual
{
	[SQLite.Table("questions")]
    public class QuestionIndividual : Question
    {
        public QuestionIndividual() { }
        public QuestionIndividual(Question question)
        {
			QuestionNumber = question.QuestionNumber;
			QuestionText = question.QuestionText;
			VariableLabel = question.VariableLabel;
			Values = question.Values;
			ValueLabels = question.ValueLabels;
			Source = question.Source;
			Note = question.Note;
		}

		[SQLite.PrimaryKey]
        public new int Pk { get; set; }
    }
}