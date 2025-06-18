using System.Collections.Generic;
using System.Text.RegularExpressions;

using XycloneDesigns.Apis.General.Tables;

namespace Database.Afrobarometer
{
	public static partial class Utils
	{
		public static partial class Inputs
		{
			public static partial class SurveySAV
			{
				public static class Replacements
				{
					public static string _General(string input, string language)
					{
						input = _Base.Replacements._General(input, language);

						foreach (string[] _General in language switch
						{
							Language.Codes.French => French.General,
							Language.Codes.Portuguese => Portuguese.General,
							Language.Codes.English or _ => English.General,

						}) input = input.Replace(_General[0], _General[1]);

						return input;
					}
					public static string _GeneralRegex(string input, string language)
					{
						input = _Base.Replacements._GeneralRegex(input, language);

						foreach (string[] _GeneralRegex in language switch
						{
							Language.Codes.French => French.GeneralRegex,
							Language.Codes.Portuguese => Portuguese.GeneralRegex,
							Language.Codes.English or _ => English.GeneralRegex,

						}) input = Regex.Replace(input, _GeneralRegex[0], _GeneralRegex[1]);

						return input;
					}

					public static class French
					{
						public static IEnumerable<string[]> General => _Base.Replacements.French.General;
						public static IEnumerable<string[]> GeneralRegex => _Base.Replacements.French.GeneralRegex;

					}
					public static class English
					{
						public static IEnumerable<string[]> General => _Base.Replacements.English.General;
						public static IEnumerable<string[]> GeneralRegex => _Base.Replacements.English.GeneralRegex;
					}
					public static class Portuguese
					{
						public static IEnumerable<string[]> General => _Base.Replacements.Portuguese.General;
						public static IEnumerable<string[]> GeneralRegex => _Base.Replacements.Portuguese.GeneralRegex;
					}
				}
			}
		}
	}
}
