using Database.Afrobarometer.Enums;

using System.IO;

namespace Database.Afrobarometer.Tables
{
	[SQLite.Table("surveys")]
    public class Survey : _AfrobarometerModel
    {
		[SQLite.Column(nameof(Country))] public Countries? Country { get; set; }
		[SQLite.Column(nameof(ListPkQuestion))] public string? ListPkQuestion { get; set; }
		[SQLite.Column(nameof(Language))] public Languages? Language { get; set; }
		[SQLite.Column(nameof(Round))] public Rounds? Round { get; set; }

		public override void Log(StreamWriter streamwriter)
		{
			base.Log(streamwriter);

			streamwriter.WriteLine("Country: {0}", Country);
			streamwriter.WriteLine("ListPkQuestion: {0}", ListPkQuestion);
			streamwriter.WriteLine("Language: {0}", Language);
			streamwriter.WriteLine("Round: {0}", Round);
			streamwriter.WriteLine();
		}
	}
}