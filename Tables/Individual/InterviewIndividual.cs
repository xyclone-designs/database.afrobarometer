
namespace Database.Afrobarometer.Tables.Individual
{
	[SQLite.Table("interviews")]
    public class InterviewIndividual : Interview
    {
		public InterviewIndividual(Interview interview)
		{
			Language = interview.Language;
			Round = interview.Round;
			Pk_Survey = interview.Pk_Survey;
			List_PkVariables = interview.List_PkVariables;
			List_PkQuestions_Record = interview.List_PkQuestions_Record;
		}

		[SQLite.Column(nameof(Pk))] public new int Pk { get; set; }
	}
}