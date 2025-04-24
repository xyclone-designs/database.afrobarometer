using System.Collections.Generic;
using System.Linq;

namespace System.IO.Compression
{
	public static class ZipArchiveEntryExtensions
	{
		public static IEnumerable<string> ExtractToDirectoryIterable(this ZipArchive ziparchive, string directory)
		{
			return ziparchive.Entries.ExtractToDirectoryIterable(directory);
		}
		public static IEnumerable<string> ExtractToDirectoryIterable(this IEnumerable<ZipArchiveEntry> ziparchiveentries, string directory)
		{
			foreach (ZipArchiveEntry entry in ziparchiveentries)
			{
				string path = Path.Combine(directory, entry.Name);

				using Stream entrystream = entry.Open();
				using FileStream filestream = File.Create(path);

				entrystream.CopyTo(filestream);
				entrystream.Close();
				filestream.Close();

				yield return path;
			}
		}
	}
}
