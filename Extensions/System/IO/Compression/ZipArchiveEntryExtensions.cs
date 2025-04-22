using System.Collections.Generic;

namespace System.IO.Compression
{
	public static class ZipArchiveEntryExtensions
	{
		public static IEnumerable<string> ExtractToDirectoryIterable(this ZipArchive ziparchive, string directory)
		{
			foreach (ZipArchiveEntry entry in ziparchive.Entries)
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
