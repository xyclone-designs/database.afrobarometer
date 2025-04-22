
namespace Database.Afrobarometer.Tables.Individual
{
	[SQLite.Table("countries")]
    public class CountryIndividual : Country
    {
        public CountryIndividual() { }
        public CountryIndividual(Country country)
        {
			Blurbs = country.Blurbs;
			Capitals = country.Capitals;
			Languages = country.Languages;
			Names = country.Names;
			Population = country.Population;
			SquareKMs = country.SquareKMs;
			UrlFlag = country.UrlFlag;
			UrlPoster = country.UrlPoster;
			UrlWebsite = country.UrlWebsite;
			UrlLogo = country.UrlLogo;
		}

		[SQLite.PrimaryKey]
        public new int Pk { get; set; }
    }
}