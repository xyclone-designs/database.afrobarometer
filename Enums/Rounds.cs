using System;

namespace Database.Afrobarometer.Enums
{
	public enum Rounds
	{
		_default,

		One,
		OnePointFive,
		_One = One | OnePointFive,
		Two,
		Three,
		ThreePointFive,
		_Three = Three | ThreePointFive,
		Four,
		FourPointFive,
		FourPointFiveOne,
		FourPointFiveTwo,
		_Four = Four | FourPointFive | FourPointFiveOne | FourPointFiveTwo,
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

			if (true switch
			{
				true when filename.Contains("r1-5") => Rounds.OnePointFive,
				true when filename.Contains("r1") => Rounds.One,
				true when filename.Contains("r2") => Rounds.Two,
				true when filename.Contains("r3.5") || filename.Contains("r3-5") => Rounds.ThreePointFive,
				true when filename.Contains("r3") || filename.Contains("r3a") || filename.Contains("r3data") => Rounds.Three,
				true when filename.Contains("r4-5-1") => Rounds.FourPointFiveOne,
				true when filename.Contains("r4-5-2") => Rounds.FourPointFiveTwo,
				true when filename.Contains("r4-5") => Rounds.FourPointFive,
				true when filename.Contains("r4") => Rounds.Four,
				true when filename.Contains("r5") => Rounds.Five,
				true when filename.Contains("r6") || filename.Contains("r6.data") || filename.Contains('6') => Rounds.Six,
				true when filename.Contains("r7") || filename.Contains("r7.data") => Rounds.Seven,
				true when filename.Contains("r8") || filename.Contains("r8.data") => Rounds.Eight,
				true when filename.Contains("r9") || filename.Contains("r9.data") => Rounds.Nine,
				true when filename.Contains("r10") => Rounds.Ten,

				_ => new Rounds?()

			} is Rounds roundone) return roundone;

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

				} is Rounds roundtwo) return roundtwo;

			throw new ArgumentException(string.Format("Round not found from '{0}'", filename));
		}

		public static int ToInt(this Rounds round)
		{
			return round switch
			{
				Rounds._default => 0,

				Rounds.One or Rounds.OnePointFive or Rounds._One => 01,
				Rounds.Two => 02,
				Rounds.Three or Rounds.ThreePointFive or Rounds._Three => 03,
				Rounds.Four or Rounds.FourPointFive or Rounds.FourPointFiveOne or Rounds.FourPointFiveTwo or Rounds._Four => 04,
				Rounds.Five => 05,
				Rounds.Six => 06,
				Rounds.Seven => 07,
				Rounds.Eight => 08,
				Rounds.Nine => 09,
				Rounds.Ten => 10,

				_ => throw new ArgumentException(string.Format("Round '{0}' not found", round))
			};
		}
		public static string AsString(this Rounds round)
		{
			return round switch
			{
				Rounds._default => string.Empty,

				Rounds.One => "01",
				Rounds.OnePointFive => "01.5",
				Rounds.Two => "02",
				Rounds.Three => "03",
				Rounds.ThreePointFive => "03.5",
				Rounds.Four => "04",
				Rounds.FourPointFive => "04.5",
				Rounds.FourPointFiveOne => "04.5.1",
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
		public static Rounds AsRound(this string round)
		{
			return round switch
			{
				"01" => Rounds.One,
				"01.5" => Rounds.OnePointFive,
				"02" => Rounds.Two,
				"03" => Rounds.Three,
				"03.5" => Rounds.ThreePointFive,
				"04" => Rounds.Four,
				"04.5" => Rounds.FourPointFive,
				"04.5.1" => Rounds.FourPointFiveOne,
				"04.5.2" => Rounds.FourPointFiveTwo,
				"05" => Rounds.Five,
				"06" => Rounds.Six,
				"07" => Rounds.Seven,
				"08" => Rounds.Eight,
				"09" => Rounds.Nine,
				"10" => Rounds.Ten,

				_ => Rounds._default
			};
		}
	}
}