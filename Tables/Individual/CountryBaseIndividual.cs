
namespace Database.Afrobarometer.Tables.Individual
{
	[SQLite.Table("countries")]
    public class CountryBaseIndividual : CountryBase 
	{
		public CountryBaseIndividual(CountryBase countrybase)
		{
			Capital = countrybase.Capital;
			Code = countrybase.Code;
			List_PkSurvey = countrybase.List_PkSurvey;
			Population = countrybase.Population;
			SquareKMs = countrybase.SquareKMs;
			UrlFlag = countrybase.UrlFlag;
			UrlInsignia = countrybase.UrlInsignia;
			UrlPoster = countrybase.UrlPoster;
			UrlWebsite = countrybase.UrlWebsite;
		}

		[SQLite.Column(nameof(Pk))] public new int Pk { get; set; }
	}
}