using SQLite;

using System;

using XycloneDesigns.Apis.General.Tables;
using XycloneDesigns.Apis.Afrobarometer.Enums;

namespace XycloneDesigns.Apis.General.Tables
{
	public static class LanguageExtensions
	{
		public static string GetLanguageCode(this string str, Rounds round, string? countrycode)
		{
			return (round, countrycode) switch
			{
				(_, _) when str == "ben_r6_data_0.sav" => Language.Codes.French,
				(_, _) when str == "afrobarometer_release-dataset_tun_r9_en_2023-03-01.sav" => Language.Codes.French,

				(_, _) when str == "ben_r6_data.sav" => Language.Codes.English,
				(_, _) when str == "afrobarometer_release-dataset_gab_r9_fr_2023-03-01.sav" => Language.Codes.English,

				(_, _) when
					str.EndsWith("_port.sav", StringComparison.OrdinalIgnoreCase) ||
					str.Contains("_port_", StringComparison.OrdinalIgnoreCase) ||
					str.Contains("portuguese", StringComparison.OrdinalIgnoreCase) => Language.Codes.Portuguese,
				(_, _) when
					str.EndsWith("_fr.sav", StringComparison.OrdinalIgnoreCase) ||
					str.Contains("_fr_", StringComparison.OrdinalIgnoreCase) ||
					str.Contains("french", StringComparison.OrdinalIgnoreCase) => Language.Codes.French,
				(_, _) when
					str.EndsWith("_en.sav") ||
					str.EndsWith("_en.sav", StringComparison.OrdinalIgnoreCase) ||
					str.EndsWith("_eng.sav", StringComparison.OrdinalIgnoreCase) ||
					str.Contains("_en_", StringComparison.OrdinalIgnoreCase) ||
					str.Contains("_eng_", StringComparison.OrdinalIgnoreCase) ||
					str.Contains("English", StringComparison.OrdinalIgnoreCase) => Language.Codes.English,

				(Rounds.Two, Country.Codes.CaboVerde) => Language.Codes.English,
				(Rounds.Three, Country.Codes.Senegal) => Language.Codes.English,
				(Rounds.Four, Country.Codes.Senegal) => Language.Codes.English,
				(Rounds.Four, Country.Codes.CaboVerde) => Language.Codes.English,
				(Rounds.Four, Country.Codes.BurkinaFaso) => Language.Codes.English,
				(Rounds.Four, Country.Codes.Mali) => Language.Codes.English,
				(Rounds.Four, Country.Codes.Madagascar) => Language.Codes.English,
				(Rounds.Five, Country.Codes.Mauritius) => Language.Codes.English,
				(Rounds.Five, Country.Codes.Togo) => Language.Codes.English,
				(Rounds.Five, Country.Codes.Guinea) => Language.Codes.English,
				(Rounds.Five, Country.Codes.Tunisia) => Language.Codes.English,
				(Rounds.Five, Country.Codes.Senegal) => Language.Codes.English,
				(Rounds.Five, Country.Codes.BurkinaFaso) => Language.Codes.English,
				(Rounds.Five, Country.Codes.Mali) => Language.Codes.English,
				(Rounds.Six, Country.Codes.Benin) => Language.Codes.English,
				(Rounds.Six, Country.Codes.CaboVerde) => Language.Codes.English,
				(Rounds.Six, Country.Codes.Mali) => Language.Codes.English,
				(Rounds.Six, Country.Codes.Senegal) => Language.Codes.English,
				(Rounds.Six, Country.Codes.Mauritius) => Language.Codes.English,
				(Rounds.Six, Country.Codes.BurkinaFaso) => Language.Codes.English,
				(Rounds.Seven, Country.Codes.Niger) => Language.Codes.English,
				(Rounds.Seven, Country.Codes.Tunisia) => Language.Codes.English,
				(Rounds.Nine, Country.Codes.Gabon) => Language.Codes.English,
				(Rounds.Nine, Country.Codes.CaboVerde) => Language.Codes.English,
				(Rounds.Nine, Country.Codes.Mauritius) => Language.Codes.English,

				(_, Country.Codes.Angola) => Language.Codes.Portuguese,
				(_, Country.Codes.CaboVerde) => Language.Codes.Portuguese,

				(Rounds.Eight, Country.Codes.Benin) => Language.Codes.French,
				(Rounds.Nine, Country.Codes.Benin) => Language.Codes.French,
				(Rounds.Eight, Country.Codes.CoteDIvoire) => Language.Codes.French,
				(Rounds.Nine, Country.Codes.Tunisia) => Language.Codes.French,
				(_, Country.Codes.BurkinaFaso) => Language.Codes.French,
				(_, Country.Codes.Cameroon) => Language.Codes.French,
				(_, Country.Codes.CongoBrazzaville) => Language.Codes.French,
				(_, Country.Codes.Gabon) => Language.Codes.French,
				(_, Country.Codes.Guinea) => Language.Codes.French,
				(_, Country.Codes.Mali) => Language.Codes.French,
				(_, Country.Codes.Madagascar) => Language.Codes.French,
				(_, Country.Codes.Mauritania) => Language.Codes.French,
				(_, Country.Codes.Mauritius) => Language.Codes.French,
				(_, Country.Codes.Niger) => Language.Codes.French,
				(_, Country.Codes.Senegal) => Language.Codes.French,
				(_, Country.Codes.Togo) => Language.Codes.French,
				(_, Country.Codes.Tunisia) => Language.Codes.French,

				(_, _) => Language.Codes.English
			};
		}
		public static Language? FromFilename(this SQLiteConnection sqliteconnection, string filename)
		{
			return sqliteconnection
				.Table<Language>()
				.FromFilename(filename, default(Rounds).FromFilename(filename), sqliteconnection.Table<Country>().FromFilename(filename));
		}
		public static Language? FromFilename(this TableQuery<Language> languages, string filename, Rounds round, Country? country)
		{
			return languages.FromFilename(filename, round, country?.Code);
		}
		public static Language? FromFilename(this TableQuery<Language> languages, string filename, Rounds round, string? countrycode)
		{
			if (filename.Contains("merge", StringComparison.OrdinalIgnoreCase))
				return null;

			string code = (round, countrycode) switch
			{
				(_, _) when filename == "ben_r6_data_0.sav" => Language.Codes.French,
				(_, _) when filename == "afrobarometer_release-dataset_tun_r9_en_2023-03-01.sav" => Language.Codes.French,

				(_, _) when filename == "ben_r6_data.sav" => Language.Codes.English,
				(_, _) when filename == "afrobarometer_release-dataset_gab_r9_fr_2023-03-01.sav" => Language.Codes.English,

				(_, _) when
					filename.EndsWith("_port.sav", StringComparison.OrdinalIgnoreCase) ||
					filename.Contains("_port_", StringComparison.OrdinalIgnoreCase) ||
					filename.Contains("portuguese", StringComparison.OrdinalIgnoreCase) => Language.Codes.Portuguese,
				(_, _) when
					filename.EndsWith("_fr.sav", StringComparison.OrdinalIgnoreCase) ||
					filename.Contains("_fr_", StringComparison.OrdinalIgnoreCase) ||
					filename.Contains("french", StringComparison.OrdinalIgnoreCase) => Language.Codes.French,
				(_, _) when
					filename.EndsWith("_en.sav") ||
					filename.EndsWith("_en.sav", StringComparison.OrdinalIgnoreCase) ||
					filename.EndsWith("_eng.sav", StringComparison.OrdinalIgnoreCase) ||
					filename.Contains("_en_", StringComparison.OrdinalIgnoreCase) ||
					filename.Contains("_eng_", StringComparison.OrdinalIgnoreCase) ||
					filename.Contains("English", StringComparison.OrdinalIgnoreCase) => Language.Codes.English,

				(Rounds.Two, Country.Codes.CaboVerde) => Language.Codes.English,
				(Rounds.Three, Country.Codes.Senegal) => Language.Codes.English,
				(Rounds.Four, Country.Codes.Senegal) => Language.Codes.English,
				(Rounds.Four, Country.Codes.CaboVerde) => Language.Codes.English,
				(Rounds.Four, Country.Codes.BurkinaFaso) => Language.Codes.English,
				(Rounds.Four, Country.Codes.Mali) => Language.Codes.English,
				(Rounds.Four, Country.Codes.Madagascar) => Language.Codes.English,
				(Rounds.Five, Country.Codes.Mauritius) => Language.Codes.English,
				(Rounds.Five, Country.Codes.Togo) => Language.Codes.English,
				(Rounds.Five, Country.Codes.Guinea) => Language.Codes.English,
				(Rounds.Five, Country.Codes.Tunisia) => Language.Codes.English,
				(Rounds.Five, Country.Codes.Senegal) => Language.Codes.English,
				(Rounds.Five, Country.Codes.BurkinaFaso) => Language.Codes.English,
				(Rounds.Five, Country.Codes.Mali) => Language.Codes.English,
				(Rounds.Six, Country.Codes.Benin) => Language.Codes.English,
				(Rounds.Six, Country.Codes.CaboVerde) => Language.Codes.English,
				(Rounds.Six, Country.Codes.Mali) => Language.Codes.English,
				(Rounds.Six, Country.Codes.Senegal) => Language.Codes.English,
				(Rounds.Six, Country.Codes.Mauritius) => Language.Codes.English,
				(Rounds.Six, Country.Codes.BurkinaFaso) => Language.Codes.English,
				(Rounds.Seven, Country.Codes.Niger) => Language.Codes.English,
				(Rounds.Seven, Country.Codes.Tunisia) => Language.Codes.English,
				(Rounds.Nine, Country.Codes.Gabon) => Language.Codes.English,
				(Rounds.Nine, Country.Codes.CaboVerde) => Language.Codes.English,
				(Rounds.Nine, Country.Codes.Mauritius) => Language.Codes.English,

				(_, Country.Codes.Angola) => Language.Codes.Portuguese,
				(_, Country.Codes.CaboVerde) => Language.Codes.Portuguese,

				(Rounds.Eight, Country.Codes.Benin) => Language.Codes.French,
				(Rounds.Nine, Country.Codes.Benin) => Language.Codes.French,
				(Rounds.Eight, Country.Codes.CoteDIvoire) => Language.Codes.French,
				(Rounds.Nine, Country.Codes.Tunisia) => Language.Codes.French,
				(_, Country.Codes.BurkinaFaso) => Language.Codes.French,
				(_, Country.Codes.Cameroon) => Language.Codes.French,
				(_, Country.Codes.CongoBrazzaville) => Language.Codes.French,
				(_, Country.Codes.Gabon) => Language.Codes.French,
				(_, Country.Codes.Guinea) => Language.Codes.French,
				(_, Country.Codes.Mali) => Language.Codes.French,
				(_, Country.Codes.Madagascar) => Language.Codes.French,
				(_, Country.Codes.Mauritania) => Language.Codes.French,
				(_, Country.Codes.Mauritius) => Language.Codes.French,
				(_, Country.Codes.Niger) => Language.Codes.French,
				(_, Country.Codes.Senegal) => Language.Codes.French,
				(_, Country.Codes.Togo) => Language.Codes.French,
				(_, Country.Codes.Tunisia) => Language.Codes.French,

				(_, _) => Language.Codes.English
			};

			return languages.FirstOrDefault(_ => _.Code == code); 
		}
	}
}
