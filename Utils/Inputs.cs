using Database.Afrobarometer.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Database.Afrobarometer
{
	public static partial class Utils
	{
		public static partial class Inputs
		{
			public static class _Base
			{
				public static partial class Substitutions
				{
					public static partial class French
					{
						public static IEnumerable<string[]> General => Enumerable.Empty<string[]>();
						public static IEnumerable<string[]> GeneralStartsWith => Enumerable.Empty<string[]>();
					}
					public static partial class English
					{
						public static IEnumerable<string[]> General => Enumerable.Empty<string[]>();
						public static IEnumerable<string[]> GeneralStartsWith => Enumerable.Empty<string[]>();
					}
					public static partial class Portuguese
					{
						public static IEnumerable<string[]> General => Enumerable.Empty<string[]>();
						public static IEnumerable<string[]> GeneralStartsWith => Enumerable.Empty<string[]>();
					}
				}
				public static partial class Replacements
				{
					public static string _Id(string input, Languages language)
					{
						(IEnumerable<string> id, string replacemenid) = language switch
						{
							Languages.French => (French.Id, French.IdReplacement),
							Languages.Portuguese => (Portuguese.Id, Portuguese.IdReplacement),
							Languages.English or _ => (English.Id, English.IdReplacement),
						};

						foreach (string _id in id)
							input = input.Replace(_id, replacemenid);

						return input.ToLower();
					}
					public static string _General(string input, Languages language)
					{
						foreach (string[] _General in language switch
						{
							Languages.French => French.General,
							Languages.Portuguese => Portuguese.General,
							Languages.English or _ => English.General,

						}) input = input.Replace(_General[0], _General[1]);

						return input;
					}
					public static string _GeneralRegex(string input, Languages language)
					{
						foreach (string[] _GeneralRegex in language switch
						{
							Languages.French => French.GeneralRegex,
							Languages.Portuguese => Portuguese.GeneralRegex,
							Languages.English or _ => English.GeneralRegex,

						}) input = Regex.Replace(input, _GeneralRegex[0], _GeneralRegex[1]);

						return input;
					}
					public static string _Country(string input, Languages language)
					{
						switch (language)
						{
							case Languages.French:
								foreach (string _FrenchCountry in French.Country)
									input = input.ReplaceOrdinalIgnoreCase(_FrenchCountry, French.CountryReplacement);
								break;

							case Languages.Portuguese:
								foreach (string _PortugueseCountry in Portuguese.Country)
									input = input.ReplaceOrdinalIgnoreCase(_PortugueseCountry, Portuguese.CountryReplacement);
								break;

							case Languages.English:
							default:
								foreach (string _EnglishCountry in English.Country)
									input = input.ReplaceOrdinalIgnoreCase(_EnglishCountry, English.CountryReplacement);
								break;
						}

						return input;
					}
					public static string _Nationality(string input, Languages language)
					{
						switch (language)
						{
							case Languages.French:
								foreach (string _FrenchNationality in French.Nationality)
									input = input.ReplaceOrdinalIgnoreCase(_FrenchNationality, French.CountryReplacement);
								break;

							case Languages.Portuguese:
								foreach (string _PortugueseNationality in Portuguese.Nationality)
									input = input.ReplaceOrdinalIgnoreCase(_PortugueseNationality, Portuguese.CountryReplacement);
								break;

							case Languages.English:
							default:
								foreach (string _EnglishNationality in English.Nationality)
									input = input.ReplaceOrdinalIgnoreCase(_EnglishNationality, English.CountryReplacement);
								break;
						}

						return input;
					}

					public static partial class French
					{
						public static string IdReplacement => string.Empty;
						public static IEnumerable<string> Id => Enumerable.Empty<string>();

						public static IEnumerable<string[]> General => Enumerable.Empty<string[]>();
						public static IEnumerable<string[]> GeneralRegex => Enumerable.Empty<string[]>();

						public static string CountryReplacement => string.Empty;
						public static IEnumerable<string> Country => Enumerable.Empty<string>();

						public static string NationalityReplacement => string.Empty;
						public static IEnumerable<string> Nationality => Enumerable.Empty<string>();
					}
					public static partial class English
					{
						public static string IdReplacement => string.Empty;
						public static IEnumerable<string> Id => Enumerable.Empty<string>()
							.Append(".")
							.Append(",")
							.Append("?")
							.Append("(")
							.Append(")")
							.Append(":")
							.Append(";")
							.Append("!")
							.Append("=")
							.Append("-")
							.Append("\'")
							.Append("\\")
							.Append(" ");

						public static IEnumerable<string[]> General => Enumerable.Empty<string[]>()
							.Append(["&", "and"])
							.Append(["vs.", "vs"])
							.Append(["don't", "do not"])
							.Append(["Don't", "Do not"])
							.Append(["canellation", "cacnellation"])
							.Append(["demonstration", "demostartion"])
							.Append(["dififculty", "difficulty"])
							.Append(["health care", "healthcare"])
							.Append(["govt", "government"])
							.Append(["govt.", "government"])
							.Append(["government.", "government"])
							.Append(["groups", "group"])
							.Append(["group's", "group"])
							.Append(["programme", "programme"])
							.Append(["regards to 1999", "regards to the 1999"])
							.Append(["responsibilty", "responsibility"])
							.Append(["resonsibility", "responsibility"])
							.Append(["storey", "story"]);
						public static IEnumerable<string[]> GeneralRegex => Enumerable.Empty<string[]>();

						public static string CountryReplacement => "[country]";
						public static IEnumerable<string> Country => Enumerable.Empty<string>()
							.Append("botswana")
							.Append("ghana")
							.Append("lesotho")
							.Append("mali")
							.Append("malawi")
							.Append("namibia")
							.Append("nigeria")
							.Append("south africa")
							.Append("tanzania")
							.Append("uganda")
							.Append("zambia")
							.Append("zimbabwe");

						public static string NationalityReplacement => "[nationality]";
						public static IEnumerable<string> Nationality => Enumerable.Empty<string>()
							.Append("basotho")
							.Append("batswana")
							.Append("ghanaian")
							.Append("ghanaians")
							.Append("ghanians")
							.Append("ghanian")
							.Append("malians")
							.Append("malian")
							.Append("malawians")
							.Append("malawian")
							.Append("mosotho")
							.Append("namibians")
							.Append("namibian")
							.Append("nigerians")
							.Append("nigerian")
							.Append("south african")
							.Append("south africans")
							.Append("south african people")
							.Append("tanzanians")
							.Append("tanzanian")
							.Append("ugandans")
							.Append("ugandan")
							.Append("zambians")
							.Append("zambian")
							.Append("zimabaweans")
							.Append("zimbabweans")
							.Append("zimbabwean");
					}
					public static partial class Portuguese
					{
						public static string IdReplacement => string.Empty;
						public static IEnumerable<string> Id => Enumerable.Empty<string>();

						public static IEnumerable<string[]> General => Enumerable.Empty<string[]>();
						public static IEnumerable<string[]> GeneralRegex => Enumerable.Empty<string[]>();

						public static string CountryReplacement => string.Empty;
						public static IEnumerable<string> Country => Enumerable.Empty<string>();

						public static string NationalityReplacement => string.Empty;
						public static IEnumerable<string> Nationality => Enumerable.Empty<string>();
					}
				}
			}
		}
	}
}
