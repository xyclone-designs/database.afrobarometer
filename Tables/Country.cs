
namespace Database.Afrobarometer.Tables
{
	[SQLite.Table("countries")]
    public class Country : _AfrobarometerModel
    {
        public string? Blurbs { get; set; }
        public string? Capitals { get; set; }
		public string? Languages { get; set; }
		public string? Names { get; set; }
		public int? Population { get; set; }
        public decimal? SquareKMs { get; set; }
		public string? UrlFlag { get; set; }
		public string? UrlPoster { get; set; }
		public string? UrlWebsite { get; set; }
		public string? UrlLogo { get; set; }
	}
}