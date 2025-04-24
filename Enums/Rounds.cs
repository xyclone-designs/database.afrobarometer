using System;

namespace Database.Afrobarometer.Enums
{
	public enum Rounds
	{
		_none,

		One,
		OnePointFive,
		Two,
		Three,
		ThreePointFive,
		Four,
		FourPointFive,
		FourPointFiveOne,
		FourPointFiveTwo,
		Five,
		Six,
		Seven,
		Eight,
		Nine,
		Ten,
	}

	public static class RoundsExtensions
	{
		public static Rounds FromFilename(this Rounds _, string filename)
		{
			filename = filename.Replace("Codebook", string.Empty);

			foreach (string split in filename.Split('_', '-', '.'))
				if (split.ToLower() switch
				{
					"r1" => Rounds.One,
					"r1-5" => Rounds.OnePointFive,
					"r2" => Rounds.Two,
					"r3" or "r3a" or "r3data" => Rounds.Three,
					"r3.5" or "r3-5" => Rounds.ThreePointFive,
					"r4" => Rounds.Four,
					"r4-5" => Rounds.FourPointFive,
					"r4-5-1" => Rounds.FourPointFiveOne,
					"r4-5-2" => Rounds.FourPointFiveTwo,
					"r5" => Rounds.Five,
					"r6" or "r6.data" or "6" => Rounds.Six,
					"r7" or "r7.data" => Rounds.Seven,
					"r8" or "r8.data" => Rounds.Eight,
					"r9" or "r9.data" => Rounds.Nine,
					"r10" => Rounds.Ten,

					_ => new Rounds?()

				} is Rounds round) return round;

			throw new ArgumentException(string.Format("Round not found from '{0}'", filename));
		}

		public static string ToString(this Rounds round)
		{
			return round switch
			{
				Rounds.One => "01",
				Rounds.OnePointFive => "01.5",
				Rounds.Two => "02",
				Rounds.Three => "03",
				Rounds.ThreePointFive => "03.5",
				Rounds.Four => "04",
				Rounds.FourPointFive => "04.5",
				Rounds.FourPointFiveOne => "04.5.2",
				Rounds.FourPointFiveTwo => "04.5.2",
				Rounds.Five => "05",
				Rounds.Six => "06",
				Rounds.Seven => "07",
				Rounds.Eight => "08",
				Rounds.Nine => "09",
				Rounds.Ten => "10",

				_ => throw new ArgumentException(string.Format("Round '{0}' not found", round))
			};
		}
	}
}