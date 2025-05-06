using Database.Afrobarometer.Enums;

using Spssly.DataReader;

using System;
using System.IO;

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

			try { Country = default(Countries).FromFilename(Filename); } catch (Exception) { throw new ArgumentException(string.Format("Error: Country '{0}'", Filename)); }
			try { Round = default(Rounds).FromFilename(Filename); } catch (Exception) { throw new ArgumentException(string.Format("Error: Round '{0}'", Filename)); }
			try { Language = default(Languages).FromFilename(Filename, Round, Country); } catch (Exception) { throw new ArgumentException(string.Format("Error: Language '{0}'", Filename)); }
		}

		private readonly FileStream _FileStream;

		public string Text { get; set; }
		public string Filename { get; set; }
		public string Filepath { get; set; }

		public Countries Country { get; set; }
		public Languages Language { get; set; }
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
		public static void Log(this StreamWriter streamwriter, SurveySAV surveysav) { }
		public static void Log(this StreamWriter streamwriter, SurveySAVVariable surveysavvariable) { }

		public static void LogError(this StreamWriter streamwriter, SurveySAV surveysav) { }
		public static void LogError(this StreamWriter streamwriter, SurveySAVVariable surveysavvariable) { }
	}
}
