using System.IO;

namespace Database.Afrobarometer.Tables
{
	[SQLite.Table("countries")]
	public class CountryBase : _AfrobarometerModel
	{
		[SQLite.Column(nameof(Capital))] public string? Capital { get; set; }
		[SQLite.Column(nameof(Code))] public string? Code { get; set; }
		[SQLite.Column(nameof(List_PkSurvey))] public string? List_PkSurvey { get; set; }
		[SQLite.Column(nameof(Population))] public int? Population { get; set; }
		[SQLite.Column(nameof(SquareKMs))] public decimal? SquareKMs { get; set; }
		[SQLite.Column(nameof(UrlFlag))] public string? UrlFlag { get; set; }
		[SQLite.Column(nameof(UrlInsignia))] public string? UrlInsignia { get; set; }
		[SQLite.Column(nameof(UrlPoster))] public string? UrlPoster { get; set; }
		[SQLite.Column(nameof(UrlWebsite))] public string? UrlWebsite { get; set; }


		[SQLite.Table("countries")]
		public class Individual : CountryBase
		{
			public Individual(Country country)
			{
				Capital = country.Capital;
				Code = country.Code;
				List_PkSurvey = country.List_PkSurvey;
				Population = country.Population;
				SquareKMs = country.SquareKMs;
				UrlFlag = country.UrlFlag;
				UrlInsignia = country.UrlInsignia;
				UrlPoster = country.UrlPoster;
				UrlWebsite = country.UrlWebsite;
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
		public static void Log(this StreamWriter streamwriter, CountryBase countrybase)
		{
			streamwriter.Log(countrybase as _AfrobarometerModel);

			streamwriter.WriteLine("Code: {0}", countrybase.Code);
			streamwriter.WriteLine("Capital: {0}", countrybase.Capital);
			streamwriter.WriteLine("List_PkSurvey: {0}", countrybase.List_PkSurvey);
			streamwriter.WriteLine("Population: {0}", countrybase.Population);
			streamwriter.WriteLine("SquareKMs: {0}", countrybase.SquareKMs);
			streamwriter.WriteLine("UrlFlag: {0}", countrybase.UrlFlag);
			streamwriter.WriteLine("UrlPoster: {0}", countrybase.UrlPoster);
			streamwriter.WriteLine("UrlWebsite: {0}", countrybase.UrlWebsite);
			streamwriter.WriteLine();
		}
		public static void LogError(this StreamWriter streamwriter, CountryBase countrybase)
		{
			streamwriter.LogError(countrybase as _AfrobarometerModel);
		}
	}
}