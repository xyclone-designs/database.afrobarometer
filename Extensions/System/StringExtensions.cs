
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
	}
}
