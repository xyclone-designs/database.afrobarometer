
namespace Database.Afrobarometer.Data.Enums
{
	public enum Rounds
	{
		One = 01,
		OnePointFive = 01_5,
		Two = 02,
		Three = 03,
		ThreePointFive = 03_5,
		Four = 04,
		Five = 05,
		Six = 06,
		Seven = 07,
		Eight = 08,
		Nine = 09,
		Ten = 10,
	}

	public static class RoundsExtensions
	{
		public static Rounds? FromFilename(this Rounds _, string filename)
		{
			string code = filename.Split('-', '_')[1];

			return code switch
			{
				"r1" => Rounds.One,
				"r1-5" => Rounds.OnePointFive,
				"r2" => Rounds.Two,
				"r3" or "r3a" or "R3data" => Rounds.Three,
				"r3-5" => Rounds.ThreePointFive,
				"r4" => Rounds.Four,
				"r5" => Rounds.Five,
				"r6" => Rounds.Six,
				"r7" => Rounds.Seven,
				"r8" => Rounds.Eight,
				"r9" => Rounds.Nine,
				"r10" => Rounds.Ten,

				_ => null
			};
		}
		public static double ToDouble(this Rounds rounds)
		{
			return rounds switch
			{
				Rounds.One => 01.0,
				Rounds.OnePointFive => 01.5,
				Rounds.Two => 02.0,
				Rounds.Three => 03.0,
				Rounds.ThreePointFive => 03.5,
				Rounds.Four => 04.0,
				Rounds.Five => 05.0,
				Rounds.Six => 06.0,
				Rounds.Seven => 07.0,
				Rounds.Eight => 08.0,
				Rounds.Nine => 09.0,
				Rounds.Ten => 10.0,

				_ => -1
			};
		}
	}
}
