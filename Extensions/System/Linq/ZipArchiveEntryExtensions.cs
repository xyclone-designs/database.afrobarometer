using System.Collections.Generic;
using System.IO.Compression;

namespace System.Linq
{
	public static class ZipArchiveEntryExtensions
	{
		public static IEnumerable<ZipArchiveEntry> OrderMergeLast(this IReadOnlyCollection<ZipArchiveEntry> ziparchiveentries, params string[] mergekeys)
		{
			mergekeys = mergekeys.Length == 0 ? ["merge"] : mergekeys;

			return ziparchiveentries.OrderBy(
				_ => _.Name,
				Comparer<string>.Create((one, two) => true switch
				{
					true when mergekeys.Any(_ => one.Contains(_)) => +1,
					true when mergekeys.Any(_ => two.Contains(_)) => -1,

					_ => Comparer<string>.Default.Compare(one, two)
				}));
		}
		public static IEnumerable<ZipArchiveContainer> OrderMergeLast(this IEnumerable<ZipArchiveContainer> inputcontainers, params string[] mergekeys)
		{
			mergekeys = mergekeys.Length == 0 ? ["merge"] : mergekeys;

			return inputcontainers.OrderBy(
				_ => _.ZipFullName,
				Comparer<string>.Create((one, two) => true switch
				{
					true when mergekeys.Any(_ => one.Contains(_)) => +1,
					true when mergekeys.Any(_ => two.Contains(_)) => -1,

					_ => Comparer<string>.Default.Compare(one, two)
				}));
		}
	}
}
