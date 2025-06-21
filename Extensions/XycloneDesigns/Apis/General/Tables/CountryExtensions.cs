using SQLite;

using System;
using System.Linq;

namespace XycloneDesigns.Apis.General.Tables
{
	public static class CountryExtensions
	{
		public static string? FindCountryCode(this string str)
		{
			if (str.Contains("merge", StringComparison.OrdinalIgnoreCase))
				return null;

			foreach (string split in str.Split('_', '-', '.').Select(_ => _.ToLower()))
			{
				string code = split switch
				{
					"mal" or "mali" => "mli",
					"mrc" => "mor",
					"namibia" => "nam",
					"swz" => "esw",

					_ => split
				};

				if (Country.Codes._All.Contains(code))
					return code;
			}

			return null; 
		}
		public static Country? FromFilename(this TableQuery<Country> countries, string filename)
		{
			if (filename.Contains("merge", StringComparison.OrdinalIgnoreCase))
				return null;

			foreach (string split in filename.Split('_', '-', '.').Select(_ => _.ToLower()))
			{
				string code = split switch
				{
					"mal" or "mali" => "mli",
					"mrc" => "mor",
					"namibia" => "nam",
					"swz" => "esw",

					_ => split
				};

				if (countries.FirstOrDefault(_ => _.Code == code) is Country country)
					return country;
			}

			return null; 
		}
	}
}
