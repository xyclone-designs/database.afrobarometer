using System.Linq;

namespace System
{
	public static class NumberExtensions
	{
		public static string ToEmptyCharacters(this int val)
		{
			return string.Join(string.Empty, Enumerable.Range(0, val.ToString().Length).Select(_ => " "));
		}
		public static string ToString(this int val, int listcount)
		{
			string _val = val.ToString();

			listcount = listcount.ToString().Length;

			while (_val.Length != listcount)
				_val = string.Format("0{0}", _val);

			return _val;
		}
	}
}
