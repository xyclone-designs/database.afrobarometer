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

		public override void Log(StreamWriter streamwriter)
		{
			base.Log(streamwriter);

			streamwriter.WriteLine("Code: {0}", Code);
			streamwriter.WriteLine("Capital: {0}", Capital);
			streamwriter.WriteLine("List_PkSurvey: {0}", List_PkSurvey);
			streamwriter.WriteLine("Population: {0}", Population);
			streamwriter.WriteLine("SquareKMs: {0}", SquareKMs);
			streamwriter.WriteLine("UrlFlag: {0}", UrlFlag);
			streamwriter.WriteLine("UrlPoster: {0}", UrlPoster);
			streamwriter.WriteLine("UrlWebsite: {0}", UrlWebsite);
			streamwriter.WriteLine();
		}
	}
}