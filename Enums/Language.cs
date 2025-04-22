using System;

namespace Database.Afrobarometer.Enums
{
	public enum Languages
	{
		English,
		French,
		Portuguese,
	}

	public static class LanguageExtensions
	{
		public static Languages FromFilename(this Languages _, string filename, Rounds round, Countries countries)
		{
			return (round, countries) switch
			{
				(_, _) when filename == "ben_r6_data_0.sav" => Languages.French,
				(_, _) when filename == "afrobarometer_release-dataset_tun_r9_en_2023-03-01.sav" => Languages.French,

				(_, _) when filename == "ben_r6_data.sav" => Languages.English,
				(_, _) when filename == "afrobarometer_release-dataset_gab_r9_fr_2023-03-01.sav" => Languages.English,

				(_, _) when
					filename.EndsWith("_port.sav", StringComparison.OrdinalIgnoreCase) ||
					filename.Contains("_port_", StringComparison.OrdinalIgnoreCase) ||
					filename.Contains("portuguese", StringComparison.OrdinalIgnoreCase) => Languages.Portuguese,
				(_, _) when
					filename.EndsWith("_fr.sav", StringComparison.OrdinalIgnoreCase) ||
					filename.Contains("_fr_", StringComparison.OrdinalIgnoreCase) ||
					filename.Contains("french", StringComparison.OrdinalIgnoreCase) => Languages.French,
				(_, _) when
					filename.EndsWith("_en.sav") ||
					filename.EndsWith("_en.sav", StringComparison.OrdinalIgnoreCase) ||
					filename.EndsWith("_eng.sav", StringComparison.OrdinalIgnoreCase) ||
					filename.Contains("_en_", StringComparison.OrdinalIgnoreCase) ||
					filename.Contains("_eng_", StringComparison.OrdinalIgnoreCase) ||
					filename.Contains("English", StringComparison.OrdinalIgnoreCase) => Languages.English,

				(Rounds.One, Countries.Mali) => Languages.English,
				(Rounds.Two, Countries.CaboVerde) => Languages.English,
				(Rounds.Three, Countries.Senegal) => Languages.English,
				(Rounds.Four, Countries.Senegal) => Languages.English,
				(Rounds.Four, Countries.CaboVerde) => Languages.English,
				(Rounds.Four, Countries.BurkinaFaso) => Languages.English,
				(Rounds.Four, Countries.Mali) => Languages.English,
				(Rounds.Four, Countries.Madagascar) => Languages.English,
				(Rounds.Five, Countries.Mauritius) => Languages.English,
				(Rounds.Five, Countries.Togo) => Languages.English,
				(Rounds.Five, Countries.Guinea) => Languages.English,
				(Rounds.Five, Countries.Tunisia) => Languages.English,
				(Rounds.Five, Countries.Senegal) => Languages.English,
				(Rounds.Five, Countries.BurkinaFaso) => Languages.English,
				(Rounds.Five, Countries.Mali) => Languages.English,
				(Rounds.Six, Countries.Benin) => Languages.English,
				(Rounds.Six, Countries.CaboVerde) => Languages.English,
				(Rounds.Six, Countries.Mali) => Languages.English,
				(Rounds.Six, Countries.Senegal) => Languages.English,
				(Rounds.Six, Countries.Mauritius) => Languages.English,
				(Rounds.Six, Countries.BurkinaFaso) => Languages.English,
				(Rounds.Seven, Countries.Niger) => Languages.English,
				(Rounds.Seven, Countries.Tunisia) => Languages.English,
				(Rounds.Nine, Countries.Gabon) => Languages.English,
				(Rounds.Nine, Countries.CaboVerde) => Languages.English,
				(Rounds.Nine, Countries.Mauritius) => Languages.English,

				(_, Countries.Angola) => Languages.Portuguese,
				(_, Countries.CaboVerde) => Languages.Portuguese,

				(Rounds.Eight, Countries.Benin) => Languages.French,
				(Rounds.Nine, Countries.Benin) => Languages.French,
				(Rounds.Eight, Countries.CoteDIvoire) => Languages.French,
				(Rounds.Nine, Countries.Tunisia) => Languages.French,
				(_, Countries.BurkinaFaso) => Languages.French,
				(_, Countries.Cameroon) => Languages.French,
				(_, Countries.CongoBrazzaville) => Languages.French,
				(_, Countries.Gabon) => Languages.French,
				(_, Countries.Guinea) => Languages.French,
				(_, Countries.Mali) => Languages.French,
				(_, Countries.Madagascar) => Languages.French,
				(_, Countries.Mauritania) => Languages.French,
				(_, Countries.Mauritius) => Languages.French,
				(_, Countries.Niger) => Languages.French,
				(_, Countries.Senegal) => Languages.French,
				(_, Countries.Togo) => Languages.French,
				(_, Countries.Tunisia) => Languages.French,

				(_, _) => Languages.English
			};
		}

		public static string ToString(this Languages language)
		{
			return language switch
			{
				Languages.French => "French",
				Languages.English => "English",
				Languages.Portuguese => "Portuguese",

				_ => throw new ArgumentException()
			};
		}
	}
}
