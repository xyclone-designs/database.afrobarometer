using Database.Afrobarometer.Enums;
using Database.Afrobarometer.Tables;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using UglyToad.PdfPig;

namespace Database.Afrobarometer.Inputs
{
	public class CodebookPDF
	{
		[StringSyntax("Regex")] public static readonly string RegexPageFooter = "\\s*Copyright Afrobarometer\\s*[0-9]?[0-9]?[0-9]?\\s*";
		[StringSyntax("Regex")] public static readonly string RegexQuestionNumber = "Question Number: Q27\\s*Question Number:";

		public static readonly string ReplacementPageFooter = " ";
		public static readonly string ReplacementQuestionNumber = "Question Number: Q27 Question:";
		
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

			try { Country = default(Countries).FromFilename(Filename); } catch (Exception) { throw new ArgumentException(string.Format("Error: Country '{0}'", Filename)); }
			try { Round = default(Rounds).FromFilename(Filename); } catch (Exception) { throw new ArgumentException(string.Format("Error: Round '{0}'", Filename)); }
			try { Language = default(Languages).FromFilename(Filename, Round, Country); } catch (Exception) { throw new ArgumentException(string.Format("Error: Language '{0}'", Filename)); }
		}

		public IEnumerable<Question> Questions 
		{
			get
			{
				StringBuilder stringbuilder = new();
				PdfDocument pdfdocument = PdfDocument.Open(Filepath);

				foreach (string text in pdfdocument.GetPages().Select(_ => _.Text))
					stringbuilder.Append(text);

				Text = stringbuilder.ToString();
				Text = Regex.Replace(Text, RegexPageFooter, ReplacementPageFooter);
				Text = Regex.Replace(Text, RegexQuestionNumber, ReplacementQuestionNumber);
				Text = Text
					.Replace("Know 98=", "Know, 98=")
					.Replace("102 QuestionIn", "102 Question: In")
					.Replace("\n", string.Empty)
					.Replace("\r", string.Empty);

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
