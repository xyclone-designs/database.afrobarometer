
namespace System
{
	public static class StringExtensions
	{
		public static bool EqualsOrdinalIgnoreCase(this string str, string? _str)
		{
			if (_str is null)
				return false;

			return string.Equals(str, _str, StringComparison.OrdinalIgnoreCase);
		}
		public static string ReplaceOrdinalIgnoreCase(this string str, string _str1, string _str2)
		{
			return str.Replace(_str1, _str2, StringComparison.OrdinalIgnoreCase);
		}

		public static string[] SplitRemoveTrim(this string str, char chr, int count = int.MaxValue)
		{
			return str.Split(chr, count, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		}
		public static string[] SplitRemoveTrim(this string str, char[]? chars, int count = int.MaxValue)
		{
			return str.Split(chars, count, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		}
		public static string[] SplitRemoveTrim(this string str, string _str, int count = int.MaxValue)
		{
			return str.Split(_str, count, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		}
		public static string[] SplitRemoveTrim(this string str, string[] _str, int count = int.MaxValue)
		{
			return str.Split(_str, count, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		}
		public static string[] SplitTrim(this string str, char chr, int count = int.MaxValue)
		{
			return str.Split(chr, count, StringSplitOptions.TrimEntries);
		}
		public static string[] SplitTrim(this string str, char[] chars, int count = int.MaxValue)
		{
			return str.Split(chars, count, StringSplitOptions.TrimEntries);
		}
		public static string[] SplitTrim(this string str, string _str, int count = int.MaxValue)
		{
			return str.Split(_str, count, StringSplitOptions.TrimEntries);
		}
		public static string[] SplitTrim(this string str, string[] _str, int count = int.MaxValue)
		{
			return str.Split(_str, count, StringSplitOptions.TrimEntries);
		}
	}
}
