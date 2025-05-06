using Database.Afrobarometer.Enums;

using System.Collections.Generic;

namespace System.IO.Compression
{
	public class ZipArchiveContainer
	{
		public ZipArchiveContainer() : this(string.Empty, string.Empty) { }
		public ZipArchiveContainer(string zippath, string zipfullname)
		{
			ZipPath = zippath;
			ZipFullName = zipfullname;
		}

		public InputTypes InputType { get; set; }
		public Countries Country { get; set; }
		public Languages Language { get; set; }
		public Rounds Round { get; set; }

		public string ZipPath { get; set; }
		public string ZipFullName { get; set; }

		public static IEnumerable<ZipArchiveContainer> FromZipPaths(params string[] zippaths)
		{
			foreach (string zippath in zippaths)
			{
				using FileStream filestream = File.OpenRead(zippath);
				using ZipArchive ziparchive = new(filestream);

				foreach (ZipArchiveEntry ziparchiveentry in ziparchive.Entries)
					yield return new ZipArchiveContainer(zippath, ziparchiveentry.FullName)
					{
						Country = default(Countries).FromFilename(ziparchiveentry.Name),
						Language = default(Languages).FromFilename(ziparchiveentry.Name),
						Round = default(Rounds).FromFilename(ziparchiveentry.Name),

						InputType = ziparchiveentry.FullName.Split('.')[^1] is string ext ? ext switch
						{
							"pdf" => InputTypes.CodebookPDF,
							"sav" => InputTypes.SurveySAV,

							_ => throw new ArgumentException(string.Format("Extension '{0}' from file '{1}' from zip '{2}'", ext, ziparchiveentry.FullName, zippath)),

						} : throw new ArgumentException("Shouldnt be happening"),
					};
			}
		}
	}
}
