using Database.Afrobarometer.Enums;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using UglyToad.PdfPig;

namespace Database.Afrobarometer.Inputs
{
	public class CodebookPDF : IDisposable
	{
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

		private PdfDocument? pdfdocument;

		public string Text { get; set; }
		public string Filename { get; set; }
		public string Filepath { get; set; }

		public Countries Country { get; set; }
		public Languages Language { get; set; }
		public Rounds Round { get; set; }

		public IEnumerable<Question> Questions
		{
			get
			{
				pdfdocument ??= PdfDocument.Open(Filepath);

				string[] split = Utils.Inputs.CodebookPDF.SplitText(pdfdocument, Language, out string text); Text = text;

				for (int index = 0; index < split.Length; index++)
				{
					string[] texts = split[index].SplitRemoveTrim(Utils.Inputs.CodebookPDF.ProcessCodebooksInputEnglishSplit);

					yield return new Question
					{
						Id = texts.ElementAtOrDefault(0),
						Text = texts.ElementAtOrDefault(1),
						VariableLabel = texts.ElementAtOrDefault(2),
						Values = texts.ElementAtOrDefault(3),
						ValueLabels = texts.ElementAtOrDefault(4),
						Source = texts.ElementAtOrDefault(5),
						Note = texts.ElementAtOrDefault(6),
					};
				}
			}
		}

		public void Dispose()
		{
			Text = string.Empty;
			pdfdocument?.Dispose();
		}

		public class Question 
		{
			public string? Id { get; set; }
			public string? Text { get; set; }
			public string? VariableLabel { get; set; }
			public string? Values { get; set; }
			public string? ValueLabels { get; set; }
			public string? Source { get; set; }
			public string? Note { get; set; }
		}
	}

	public static partial class StreamWriterExtensions
	{
		public static void Log(this StreamWriter streamwriter, CodebookPDF codebookpdf) { }
		public static void Log(this StreamWriter streamwriter, CodebookPDF.Question codebookpdfquestion)
		{
			streamwriter.WriteLine("QuestionId: {0}", codebookpdfquestion.Id);
			streamwriter.WriteLine("Question: {0}", codebookpdfquestion.Text);
			streamwriter.WriteLine("VariableLabel: {0}", codebookpdfquestion.VariableLabel);
			streamwriter.WriteLine("Values: {0}", codebookpdfquestion.Values?.ElementAtOrDefault(0));

			if (codebookpdfquestion.Values is not null && codebookpdfquestion.Values.Length > 2)
				for (int index = 1; index < codebookpdfquestion.Values.Length; index++)
					streamwriter.WriteLine("        {0}", codebookpdfquestion.Values[index]);

			streamwriter.WriteLine("ValueLabels: {0}", codebookpdfquestion.ValueLabels?.ElementAtOrDefault(0));

			if (codebookpdfquestion.ValueLabels is not null && codebookpdfquestion.ValueLabels.Length > 2)
				for (int index = 1; index < codebookpdfquestion.ValueLabels.Length; index++)
					streamwriter.WriteLine("             {0}", codebookpdfquestion.ValueLabels[index]);

			streamwriter.WriteLine("Source: {0}", codebookpdfquestion.Source);
			streamwriter.WriteLine("Note: {0}", codebookpdfquestion.Note);
			streamwriter.WriteLine();
		}

		public static void LogError(this StreamWriter streamwriter, CodebookPDF codebookpdf) { }
		public static void LogError(this StreamWriter streamwriter, CodebookPDF.Question codebookpdfquestion)
		{
			if (codebookpdfquestion.Id is null || Regex.IsMatch(codebookpdfquestion.Id, "Q[0-9]+[A-za-z]?\\s"))
				streamwriter.WriteLine("QuestionId: {0}", codebookpdfquestion.Id);
		}
	}
}
