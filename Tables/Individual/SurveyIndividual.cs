
namespace Database.Afrobarometer.Tables.Individual
{
	[SQLite.Table("surveys")]
    public class SurveyIndividual : Survey
    {
        public SurveyIndividual() { }
        public SurveyIndividual(Survey survey)
        {
			Language = survey.Language;
			Round = survey.Round;
		}

		[SQLite.PrimaryKey]
        public new int Pk { get; set; }
    }
}