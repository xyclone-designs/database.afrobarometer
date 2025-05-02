using System.IO;

namespace Database.Afrobarometer.Tables
{
    public class Country : CountryBase
	{
		[SQLite.Column(nameof(Blurb))] public string? Blurb { get; set; }
        [SQLite.Column(nameof(Languages))] public string? Languages { get; set; }
		[SQLite.Column(nameof(Name))] public string? Name { get; set; }

		public override void Log(StreamWriter streamwriter)
		{
			streamwriter.WriteLine("Blurb: {0}", Blurb);
			streamwriter.WriteLine("Languages: {0}", Languages);
			streamwriter.WriteLine("Name: {0}", Name);

			base.Log(streamwriter);
		}
	}
}