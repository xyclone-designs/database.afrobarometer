using Database.Afrobarometer.Enums;

namespace Database.Afrobarometer.Inputs
{
	public class FilePair
	{
		public FilePair() { }
		public FilePair(string? codebookpdffilepath, string? surveysavfilepath)
		{
			CodebookPDFFilepath = codebookpdffilepath;
			SurveySAVFilepath = surveysavfilepath;

			if ((CodebookPDFFilename ?? SurveySAVFilename) is string filename)
			{
				Round = Round.FromFilename(filename);
				Country = Country.FromFilename(filename);
				Language = Language.FromFilename(filename, Round, Country);
			}				
		}

		private string? _CodebookPDFFilename;
		private string? _SurveySAVFilename;

		public string? CodebookPDFFilepath { get; set; }
		public string? SurveySAVFilepath { get; set; }

		public string? CodebookPDFFilename
		{
			get => _CodebookPDFFilename ??= CodebookPDFFilepath?.Split('\\')[^0];
		}
		public string? SurveySAVFilename
		{
			get => _SurveySAVFilename ??= SurveySAVFilepath?.Split('\\')[^0];
		}

		public Countries Country { get; set; }
		public Languages Language { get; set; }
		public Rounds Round { get; set; }
	}
}
