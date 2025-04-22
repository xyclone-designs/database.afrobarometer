
using Database.Afrobarometer.Enums;

namespace Database.Afrobarometer.Tables
{
	[SQLite.Table("interviews")]
    public class Interview : _AfrobarometerModel
    {
		public Languages? Language { get; set; }
		public Rounds? Round { get; set; }
	}
}