using Spssly.DataReader;
using SQLite;
using System;
using System.IO;

using XycloneDesigns.Apis.Afrobarometer.Enums;

using SurveySAVVariable = Spssly.SpssDataset.Variable;

namespace Database.Afrobarometer.Inputs
{
	public class SurveySAV : SpssReader
	{
		public SurveySAV(string filepath) : this(new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.None, 2048, FileOptions.SequentialScan), filepath) { }
		public SurveySAV(FileStream filestream, string filepath) : base(filestream)
		{
			_FileStream = filestream;

			Text = string.Empty;
			Filepath = filepath;
			Filename = Filepath.Split('\\')[^1];
		}

		private readonly FileStream _FileStream;

		public string Text { get; set; }
		public string Filename { get; set; }
		public string Filepath { get; set; }

		public string? CountryCode { get; set; }
		public string? LanguageCode { get; set; }
		public Rounds Round { get; set; }

		public new void Dispose()
		{
			_FileStream.Flush();
			_FileStream.Close();

			base.Dispose();
		}
	}

	public static partial class StreamWriterExtensions
	{
		public static string? GetId(this SurveySAVVariable surveysavvariable, string language) 
		{
			if (string.IsNullOrWhiteSpace(surveysavvariable.Label))
				return null;

			string cleanlabel = Utils.Inputs.SurveySAV.Variable.CleanLabel(surveysavvariable.Label, language);
			string id = Utils.Inputs._Base.Replacements._Id(cleanlabel, language);

			return id;
		}
	}
}
