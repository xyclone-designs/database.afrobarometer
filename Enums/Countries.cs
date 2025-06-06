﻿using System;

namespace Database.Afrobarometer.Enums
{
	public enum Countries
	{
		_none,
		_merged,
		
		Algeria,
		Angola,
		Benin,
		Botswana,
		Burundi,
		BurkinaFaso,
		Cameroon,
		CaboVerde,
		CoteDIvoire,
		CongoBrazzaville,
		Egypt,
		Ethiopia,
		Eswatini,
		Gabon,
		Ghana,
		Gambia,
		Guinea,
		Kenya,
		Lesotho,
		Liberia,
		Madagascar,
		Mauritius,
		Mauritania,
		Malawi,
		Mali,
		Morocco,
		Mozambique,
		Namibia,
		Niger,
		Nigeria,
		SaoTomeAndPrincipe,
		Senegal,
		Seychelles,
		SierraLeone,
		SouthAfrica,
		Sudan,
		Tanzania,
		Togo,
		Tunisia,
		Uganda,
		Zambia,
		Zimbabwe,
	}

	public static class CountriesExtensions
	{
		public static Countries FromFilename(this Countries _, string filename)
		{
			if (filename.Contains("merge", StringComparison.OrdinalIgnoreCase))
				return Countries._merged;
		
			foreach (string split in filename.Split('_', '-', '.'))
				if (split.ToLower() switch
				{
					"alg" => Countries.Algeria,
					"ang" => Countries.Angola,
					"bdi" => Countries.Burundi,
					"ben" => Countries.Benin,
					"bfo" => Countries.BurkinaFaso,
					"bot" => Countries.Botswana,
					"cam" => Countries.Cameroon,
					"cbz" => Countries.CongoBrazzaville,
					"cve" => Countries.CaboVerde,
					"cdi" => Countries.CoteDIvoire,
					"egy" => Countries.Egypt,
					"eth" => Countries.Ethiopia,
					"gab" => Countries.Gabon,
					"gha" => Countries.Ghana,
					"gam" => Countries.Gambia,
					"gui" => Countries.Guinea,
					"ken" => Countries.Kenya,
					"les" => Countries.Lesotho,
					"lib" => Countries.Liberia,
					"mad" => Countries.Madagascar,
					"mau" => Countries.Mauritius,
					"mal" or "mali" or "mli" => Countries.Mali,
					"mlw" => Countries.Malawi,
					"mor" or "mrc" => Countries.Morocco,
					"moz" => Countries.Mozambique,
					"mta" => Countries.Mauritania,
					"nam" or "namibia" => Countries.Namibia,
					"ngr" => Countries.Niger,
					"nig" => Countries.Nigeria,
					"stp" => Countries.SaoTomeAndPrincipe,
					"saf" => Countries.SouthAfrica,
					"sen" => Countries.Senegal,
					"sey" => Countries.Seychelles,
					"srl" => Countries.SierraLeone,
					"sud" => Countries.Sudan,
					"swz" or "esw" => Countries.Eswatini,
					"tan" => Countries.Tanzania,
					"tog" => Countries.Togo,
					"tun" => Countries.Tunisia,
					"uga" => Countries.Uganda,
					"zam" => Countries.Zambia,
					"zim" => Countries.Zimbabwe,

					_ => new Countries?()

				} is Countries country) return country;

			throw new ArgumentException(string.Format("Countries not found from '{0}'", filename));
		}
		public static string ToCode(this Countries country)
		{
			return country switch
			{
				Countries._merged => string.Empty,
				Countries._none => string.Empty,

				Countries.Algeria => "alg",
				Countries.Angola => "ang",
				Countries.Burundi => "bdi",
				Countries.Benin => "ben",
				Countries.BurkinaFaso => "bfo",
				Countries.Botswana => "bot",
				Countries.Cameroon => "cam",
				Countries.CongoBrazzaville => "cbz",
				Countries.CaboVerde => "cve",
				Countries.CoteDIvoire => "cdi",
				Countries.Egypt => "egy",
				Countries.Eswatini => "esw",
				Countries.Ethiopia => "eth",
				Countries.Gabon => "gab",
				Countries.Ghana => "gha",
				Countries.Gambia => "gam",
				Countries.Guinea => "gui",
				Countries.Kenya => "ken",
				Countries.Lesotho => "les",
				Countries.Liberia => "lib",
				Countries.Madagascar => "mad",
				Countries.Mauritius => "mau",
				Countries.Mali => "mli",
				Countries.Malawi => "mlw",
				Countries.Morocco => "mor",
				Countries.Mozambique => "moz",
				Countries.Mauritania => "mta",
				Countries.Namibia => "nam",
				Countries.Niger => "nig",
				Countries.Nigeria => "ngr",
				Countries.SaoTomeAndPrincipe => "stp",
				Countries.SouthAfrica => "saf",
				Countries.Senegal => "sen",
				Countries.Seychelles => "sey",
				Countries.SierraLeone => "srl",
				Countries.Sudan => "sud",
				Countries.Tanzania => "tan",
				Countries.Togo => "tog",
				Countries.Tunisia => "tun",
				Countries.Uganda => "uga",
				Countries.Zambia => "zam",
				Countries.Zimbabwe => "zim",

				_ => throw new ArgumentException(string.Format("Round '{0}' not found", country))
			};
		}
		public static string ToString(this Countries country)
		{
			return country switch
			{
				Countries._merged => string.Empty,
				Countries._none => string.Empty,

				Countries.Algeria => "Algeria",
				Countries.Angola => "Angola",
				Countries.Burundi => "Burundi",
				Countries.Benin => "Benin",
				Countries.BurkinaFaso => "Burkina Faso",
				Countries.Botswana => "Botswana",
				Countries.Cameroon => "Cameroon",
				Countries.CongoBrazzaville => "Congo-Brazzaville",
				Countries.CaboVerde => "Cabo Verde",
				Countries.CoteDIvoire => "Cote d'Ivoire",
				Countries.Egypt => "Egypt",
				Countries.Eswatini => "Eswatini",
				Countries.Ethiopia => "Ethiopia",
				Countries.Gabon => "Gabon",
				Countries.Ghana => "Ghana",
				Countries.Gambia => "Gambia",
				Countries.Guinea => "Guinea",
				Countries.Kenya => "Kenya",
				Countries.Lesotho => "Lesotho",
				Countries.Liberia => "Liberia",
				Countries.Madagascar => "Madagascar",
				Countries.Mauritius => "Mauritius",
				Countries.Mali => "Mali",
				Countries.Malawi => "Malawi",
				Countries.Morocco => "Morocco",
				Countries.Mozambique => "Mozambique",
				Countries.Mauritania => "Mauritania",
				Countries.Namibia => "Namibia",
				Countries.Niger => "Niger",
				Countries.Nigeria => "Nigeria",
				Countries.SaoTomeAndPrincipe => "São Tomé and Príncipe",
				Countries.SouthAfrica => "South Africa",
				Countries.Senegal => "Senegal",
				Countries.Seychelles => "Seychelles",
				Countries.SierraLeone => "Sierra Leone",
				Countries.Sudan => "Sudan",
				Countries.Tanzania => "Tanzania",
				Countries.Togo => "Togo",
				Countries.Tunisia => "Tunisia",
				Countries.Uganda => "Uganda",
				Countries.Zambia => "Zambia",
				Countries.Zimbabwe => "Zimbabwe",

				_ => throw new ArgumentException(string.Format("Round '{0}' not found", country))
			};
		}
	}
}
