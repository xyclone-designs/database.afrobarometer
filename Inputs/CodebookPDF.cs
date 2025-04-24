using Database.Afrobarometer.Enums;
using Database.Afrobarometer.Tables;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace Database.Afrobarometer.Inputs
{
	public class CodebookPDF
	{
		public static readonly string[][] ProcessCodebooksInputEnglishRegexes = 
		[
			[ "Copyright Afrobarometer\\s*[0-9]*", string.Empty ],
			[ ",\\s*Post\\s*Code=Other\\s*[A-Za-z0-9]*\\s*\\[[Ss]pecify\\]", string.Empty ],
			[ "Question Number: Q27\\s*Question Number:", "Question Number: Q27 Question:"],
		];			
		public static readonly string[][] ProcessCodebooksInputEnglishReplacements = 
		[
			[ "\n", string.Empty ],
			[ "\r", string.Empty ],
			[ "A=1, B=2, C=3", "1=A, 2=B, 3=C"],
			[ "102 QuestionIn", "102 Question: In" ],
			[ "non=permanent", "non-permanent" ],
			[ "North=Sotho", "North-Sotho" ],
			[ "South=Sotho", "South-Sotho" ],
			[ "Sesotho, Sotho, S. Sotho ", "Sesotho/Sotho/S-Sotho, "],
		];			
		public static readonly string[] ProcessCodebooksInputEnglishSplit =
		[
			"Question Number:",
			"Question:",
			"Variable Label:", "Variable label:",
			"Values:",
			"Value Label:", "Value Labels:", "Values Labels:",
			"Source:", "DataSource:",
			"Note:", "Notes:"
		];

		public CodebookPDF(string filepath)
		{
			Text = string.Empty;
			Filepath = filepath;
			Filename = Filepath.Split('\\')[^1];

			Country = default;
			Round = default;
			Language = default;

			Round = Round.FromFilename(Filename);
			Country = Country.FromFilename(Filename);
			Language = Language.FromFilename(Filename, Round, Country);
		}

		public IEnumerable<Question> Questions 
		{
			get
			{
				bool copyrightnumbered = false;
				StringBuilder stringbuilder = new();
				PdfDocument pdfdocument = PdfDocument.Open(Filepath);

				foreach (Page page in pdfdocument.GetPages())
				{
					stringbuilder.Append(page.Text);

					if (page.Number == 1)
						copyrightnumbered = Regex.IsMatch(page.Text, "Copyright Afrobarometer\\s*1{1}", RegexOptions.Singleline);
				}

				Text = stringbuilder.ToString();

				if (copyrightnumbered is false)
					Text = Regex.Replace(Text, "Copyright Afrobarometer", string.Empty);
				else for (int pagenumber = 1; pagenumber < pdfdocument.NumberOfPages; pagenumber++)
					Text = Regex.Replace(Text, string.Format("Copyright Afrobarometer\\s*{0}{1}", pagenumber, "{1}"), string.Empty, RegexOptions.Singleline);

				foreach (string[] _ in ProcessCodebooksInputEnglishRegexes)
					Text = Regex.Replace(Text, _[0], _[1]);

				foreach (string[] _ in ProcessCodebooksInputEnglishReplacements)
					Text = Text.Replace(_[0], _[1]);

				string[] split = Text.Split(ProcessCodebooksInputEnglishSplit[0], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
				split = split.Length > 2 ? split[1..^1] : split;

				for (int index = 0; index < split.Length; index++)
				{
					string[] spl = split[index].Split(ProcessCodebooksInputEnglishSplit, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

					yield return new Question(spl);
				}
			}
		}

		public string Text { get; set; }
		public string Filename { get; set; }
		public string Filepath { get; set; }

		public Countries Country { get; set; }
		public Languages Language { get; set; }
		public Rounds Round { get; set; }
	}
}
