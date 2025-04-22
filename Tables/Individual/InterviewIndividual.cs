
namespace Database.Afrobarometer.Tables.Individual
{
	[SQLite.Table("interviews")]
    public class InterviewIndividual : Interview
    {
        public InterviewIndividual() { }
        public InterviewIndividual(Interview survey)
        {
			Language = survey.Language;
			Round = survey.Round;
		}

		[SQLite.PrimaryKey]
        public new int Pk { get; set; }
    }
}