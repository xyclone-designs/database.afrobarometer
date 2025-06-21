using Database.Afrobarometer.Enums;

using System.Collections.Generic;
using System.Linq;

using XycloneDesigns.Apis.Afrobarometer.Enums;
using XycloneDesigns.Apis.General.Tables;

namespace System.IO.Compression
{
	public class ZipArchiveContainer
	{
		public ZipArchiveContainer() : this(string.Empty, string.Empty) { }
		public ZipArchiveContainer(string zippath, string zipfullname)
		{
			ZipPath = zippath;
			ZipFullName = zipfullname;
			LanguageCode = Language.Codes.English;
		}

		public InputTypes InputType { get; set; }
		public string? CountryCode { get; set; }
		public string LanguageCode { get; set; }
		public Rounds Round { get; set; }

		public string ZipPath { get; set; }
		public string ZipFullName { get; set; }

		public static IEnumerable<ZipArchiveContainer> FromZipPaths(params string[] zippaths)
		{
			return FromZipPaths(zippaths as IEnumerable<string>);
		}
		public static IEnumerable<ZipArchiveContainer> FromZipPaths(IEnumerable<string> zippaths)
		{
			foreach (string zippath in zippaths.SelectMany(_ =>
			{
				if (Directory.Exists(_))
					return Directory.EnumerateFiles(_);

				return [_];
			}))
			{
				using FileStream filestream = File.OpenRead(zippath);
				using ZipArchive ziparchive = new(filestream);

				foreach (ZipArchiveEntry ziparchiveentry in ziparchive.Entries)
				{
					Rounds round = default(Rounds).FromFilename(ziparchiveentry.Name);
					string? countrycode = ziparchiveentry.Name.FindCountryCode();
					string? languagecode = ziparchiveentry.Name.GetLanguageCode(round, countrycode);

					yield return new ZipArchiveContainer(zippath, ziparchiveentry.FullName)
					{
						Round = round,
						CountryCode = countrycode,
						LanguageCode = languagecode,
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
}
