using System.Collections.Generic;

namespace System.IO.Compression
{
	public static class ZipArchiveEntryExtensions
	{
		public static string ExtractToDirectory(this ZipArchiveEntry ziparchiveentry, string directory)
		{
			string path = Path.Combine(directory, ziparchiveentry.Name);

			using Stream entrystream = ziparchiveentry.Open();
			using FileStream filestream = File.Create(path);

			entrystream.CopyTo(filestream);
			entrystream.Close();
			filestream.Close();

			return path;
		}

		public static IEnumerable<string> ExtractToDirectoryIterable(this ZipArchive ziparchive, string directory)
		{
			return ziparchive.Entries.ExtractToDirectoryIterable(directory);
		}
		public static IEnumerable<string> ExtractToDirectoryIterable(this IEnumerable<ZipArchiveEntry> ziparchiveentries, string directory)
		{
			foreach (ZipArchiveEntry entry in ziparchiveentries)
				yield return entry.ExtractToDirectory(directory);
		}
	}
}
