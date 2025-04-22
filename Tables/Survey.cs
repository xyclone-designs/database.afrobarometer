
using Database.Afrobarometer.Enums;

namespace Database.Afrobarometer.Tables
{
	[SQLite.Table("surveys")]
    public class Survey : _AfrobarometerModel
    {
		public Languages? Language { get; set; }
		public Rounds? Round { get; set; }
	}
}