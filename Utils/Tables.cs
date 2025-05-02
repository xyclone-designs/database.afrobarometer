using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Database.Afrobarometer
{
	public static partial class Utils
	{
		public static partial class Tables
		{
			[StringSyntax("Regex")]
			public static readonly string Regex_Id_Removal = "[\\s:;,'?/.,()]+";

			public static string ToId(string str)
			{
				return Regex.Replace(str, Regex_Id_Removal, string.Empty);
			}
		}
	}
}
