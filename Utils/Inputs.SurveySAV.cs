using Database.Afrobarometer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

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
					public static string _General(string input, Languages language)
					{
						input = _Base.Replacements._General(input, language);

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
						input = _Base.Replacements._GeneralRegex(input, language);

						foreach (string[] _GeneralRegex in language switch
						{
							Languages.French => French.GeneralRegex,
							Languages.Portuguese => Portuguese.GeneralRegex,
							Languages.English or _ => English.GeneralRegex,

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
