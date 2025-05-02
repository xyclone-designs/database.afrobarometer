
namespace Database.Afrobarometer.Tables.Individual
{
    public class CountryIndividual : Country 
	{
		public CountryIndividual(Country country)
		{
			Blurb = country.Blurb;
			Languages = country.Languages;
			Name = country.Name;
		}

		[SQLite.Column(nameof(Pk))] public new int Pk { get; set; }
	}
}