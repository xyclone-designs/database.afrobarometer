using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Database.Afrobarometer
{
	public static partial class Utils
	{
		public static partial class Likeness
		{
			[StringSyntax("Regex")]
			public static readonly string Regex_Key_Removal = "[\\s:;,'?/.,()]+";

			public static string ToKey(string str)
			{
				return Regex.Replace(str, Regex_Key_Removal, string.Empty);
			}
		}
	}
}
