using Database.Afrobarometer.Enums;

using Spssly.DataReader;

using System;
using System.IO;

namespace Database.Afrobarometer.Inputs
{
	public class SurveySAV : SpssReader
	{
		public SurveySAV(FileStream filestream, string filepath) : base(filestream)
		{
			Text = string.Empty;
			Filepath = filepath;
			Filename = Filepath.Split('\\')[^1];

			try { Country = default(Countries).FromFilename(Filename); } catch (Exception) { throw new ArgumentException(string.Format("Error: Country '{0}'", Filename)); }
			try { Round = default(Rounds).FromFilename(Filename); } catch (Exception) { throw new ArgumentException(string.Format("Error: Round '{0}'", Filename)); }
			try { Language = default(Languages).FromFilename(Filename, Round, Country); } catch (Exception) { throw new ArgumentException(string.Format("Error: Language '{0}'", Filename)); }
		}

		public string Text { get; set; }
		public string Filename { get; set; }
		public string Filepath { get; set; }

		public Countries Country { get; set; }
		public Languages Language { get; set; }
		public Rounds Round { get; set; }
	}
}
