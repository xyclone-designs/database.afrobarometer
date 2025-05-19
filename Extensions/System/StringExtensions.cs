using System.Collections.Generic;
using System.Linq;

namespace System
{
	public static class StringExtensions
	{
		public static string[] SplitWithoutRemoval(this string str, string? separator) 
		{
			return str.SplitWithoutRemoval(separator is null ? null : [separator], int.MaxValue);
		}
		public static string[] SplitWithoutRemoval(this string str, string? separator, int count) 
		{
			return str.SplitWithoutRemoval(separator is null ? null : [separator], count);
		}

		public static string[] SplitWithoutRemoval(this string str, string[]? separators) 
		{
			return str.SplitWithoutRemoval(separators, int.MaxValue);
		}
		public static string[] SplitWithoutRemoval(this string str, string[]? separators, int count) 
		{
			ArgumentOutOfRangeException.ThrowIfNegative(count);

			if (count <= 1 || separators is null)
				return [str];

			IEnumerable<string> _separators = separators
				.OrderBy(_ => str.IndexOf(_))
				.Where(_ => str.Contains(_));

			string? separator = _separators.FirstOrDefault();

			if (separator is null)
				return [str];

			string[] spl = [], temp = str.Split(separator, 2);

			while (spl.Length < count)
			{
				if (separator is null || temp.Length == 1) { spl = [.. spl, .. temp]; break; }

				string? nextseparator = separators
					.OrderBy(_ => temp[1].IndexOf(_))
					.FirstOrDefault(_ => temp[1].Contains(_));

				if (str.StartsWith(separator))
				{
					string[] _temp = temp[1].Split(nextseparator, 2);

					spl = [.. spl, string.Format("{0}{1}", separator, _temp[0])];

					if (_temp.Length == 1)
					{
						str = string.Format("{0}{1}", nextseparator, _temp[0]); 
						break;
					}
					else str = string.Format("{0}{1}", nextseparator, _temp[1]);
				}
				else
				{
					str = temp[1];
					spl = spl.Length == 0 
						? [temp[0]] 
						: [.. spl, string.Format("{0}{1}", separator, temp[0])];

				}

				if (nextseparator is null) { spl = [.. spl, string.Format("{0}{1}", separator, str)]; break; }

				_separators = separators
					.OrderBy(_ => temp[1].IndexOf(_))
					.Where(_ => temp[1].Contains(_));

				temp = str.Split(nextseparator, 2);
				separator = nextseparator;
			}
				
			return spl;		
		}

		public static string ToEmptyCharacters(this string val)
		{
			return string.Join(' ', Enumerable.Range(0, val.Length + 1).Select(_ => string.Empty));
		}
	}
}
