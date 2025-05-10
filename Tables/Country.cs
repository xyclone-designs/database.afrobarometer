using System.IO;

namespace Database.Afrobarometer.Tables
{
	[SQLite.Table("countries")]
	public class Country : CountryBase
	{
		public Country() { }
		public Country(CountryBase countrybase)
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

		[SQLite.Column(nameof(Blurb))] public string? Blurb { get; set; }
		[SQLite.Column(nameof(Languages))] public string? Languages { get; set; }
		[SQLite.Column(nameof(Name))] public string? Name { get; set; }


		[SQLite.Table("countries")]
		public new class Individual : Country
		{
			public Individual() { }
			public Individual(Country country) : base(country)
			{
				Blurb = country.Blurb;
				Languages = country.Languages;
				Name = country.Name;
			}

			[SQLite.AutoIncrement]
			[SQLite.NotNull]
			[SQLite.PrimaryKey]
			[SQLite.Unique]
			public new int Pk { get; set; }
		}
	}

	public static partial class StreamWriterExtensions
	{
		public static void Log(this StreamWriter streamwriter, Country country)
		{
			streamwriter.Log(country as _AfrobarometerModel);

			streamwriter.WriteLine("Blurb: {0}", country.Blurb);
			streamwriter.WriteLine("Languages: {0}", country.Languages);
			streamwriter.WriteLine("Name: {0}", country.Name);
			streamwriter.WriteLine("Code: {0}", country.Code);
			streamwriter.WriteLine("Capital: {0}", country.Capital);
			streamwriter.WriteLine("List_PkSurvey: {0}", country.List_PkSurvey);
			streamwriter.WriteLine("Population: {0}", country.Population);
			streamwriter.WriteLine("SquareKMs: {0}", country.SquareKMs);
			streamwriter.WriteLine("UrlFlag: {0}", country.UrlFlag);
			streamwriter.WriteLine("UrlPoster: {0}", country.UrlPoster);
			streamwriter.WriteLine("UrlWebsite: {0}", country.UrlWebsite);
			streamwriter.WriteLine();
		}
		public static void LogError(this StreamWriter streamwriter, Country country)
		{
			streamwriter.LogError(country as _AfrobarometerModel);
		}
	}
}