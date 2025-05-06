using System.IO;

namespace Database.Afrobarometer.Tables
{
	[SQLite.Table("countries")]
	public class Country : CountryBase
	{
		[SQLite.Column(nameof(Blurb))] public string? Blurb { get; set; }
        [SQLite.Column(nameof(Languages))] public string? Languages { get; set; }
		[SQLite.Column(nameof(Name))] public string? Name { get; set; }

		[SQLite.Table("countries")]
		public new class Individual : Country
		{
			public Individual(Country country)
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
			streamwriter.Log(country as CountryBase);

			streamwriter.WriteLine("Blurb: {0}", country.Blurb);
			streamwriter.WriteLine("Languages: {0}", country.Languages);
			streamwriter.WriteLine("Name: {0}", country.Name);
		}
		public static void LogError(this StreamWriter streamwriter, Country country)
		{
			streamwriter.LogError(country as _AfrobarometerModel);
		}
	}
}