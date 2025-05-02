
namespace Database.Afrobarometer.Tables.Individual
{
	[SQLite.Table("surveys")]
    public class SurveyIndividual : Survey
    {
		public SurveyIndividual(Survey survey)
		{
			Country = survey.Country;
			ListPkQuestion = survey.ListPkQuestion;
			Language = survey.Language;
			Round = survey.Round;
		}

		[SQLite.Column(nameof(Pk))] public new int Pk { get; set; }
	}
}