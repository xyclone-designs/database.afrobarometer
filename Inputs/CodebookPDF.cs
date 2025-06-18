using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using UglyToad.PdfPig;

using XycloneDesigns.Apis.General.Tables;
using XycloneDesigns.Apis.Afrobarometer.Enums;

namespace Database.Afrobarometer.Inputs
{
	public class CodebookPDF : IDisposable
	{
		public CodebookPDF(string filepath)
		{
			Text = string.Empty;
			Filepath = filepath;
			Filename = Filepath.Split('\\')[^1];
			Round = Round.FromFilename(Filename);
		}

		private PdfDocument? pdfdocument;

		public string Text { get; set; }
		public string Filename { get; set; }
		public string Filepath { get; set; }

		public string? CountryCode { get; set; }
		public string? LanguageCode { get; set; }
		public Rounds Round { get; set; }

		public IEnumerable<Question> Questions
		{
			get
			{
				pdfdocument ??= PdfDocument.Open(Filepath);

				string[] split = Utils.Inputs.CodebookPDF.SplitText(pdfdocument, LanguageCode ?? Language.Codes.English, out string rawtext); Text = rawtext;

				for (int index = 0; index < split.Length; index++)
				{
					string[] texts = split[index].SplitWithoutRemoval(Utils.Inputs.CodebookPDF.ProcessCodebooksInputEnglishSplit);

					Question question = new();

					if (texts.FirstOrDefault(_ => Utils.Inputs.CodebookPDF.ProcessCodebooksInputEnglishSplit_Id.Any(__ => _.Contains(__))) is string id)
						question.Id = id.SplitTrim(Utils.Inputs.CodebookPDF.ProcessCodebooksInputEnglishSplit_Id)[1];

					if (texts.FirstOrDefault(_ => Utils.Inputs.CodebookPDF.ProcessCodebooksInputEnglishSplit_Text.Any(__ => _.Contains(__))) is string text)
						question.Text = text.SplitTrim(Utils.Inputs.CodebookPDF.ProcessCodebooksInputEnglishSplit_Text)[1];

					if (texts.FirstOrDefault(_ => Utils.Inputs.CodebookPDF.ProcessCodebooksInputEnglishSplit_VariableLabel.Any(__ => _.Contains(__))) is string variablelabel)
						question.VariableLabel = variablelabel.SplitTrim(Utils.Inputs.CodebookPDF.ProcessCodebooksInputEnglishSplit_VariableLabel)[1];

					if (texts.FirstOrDefault(_ => Utils.Inputs.CodebookPDF.ProcessCodebooksInputEnglishSplit_Values.Any(__ => _.Contains(__))) is string values)
						question.Values = values.SplitTrim(Utils.Inputs.CodebookPDF.ProcessCodebooksInputEnglishSplit_Values)[1];

					if (texts.FirstOrDefault(_ => Utils.Inputs.CodebookPDF.ProcessCodebooksInputEnglishSplit_ValueLabels.Any(__ => _.Contains(__))) is string valuelabels)
						question.ValueLabels = valuelabels.SplitTrim(Utils.Inputs.CodebookPDF.ProcessCodebooksInputEnglishSplit_ValueLabels)[1];

					if (texts.FirstOrDefault(_ => Utils.Inputs.CodebookPDF.ProcessCodebooksInputEnglishSplit_Source.Any(__ => _.Contains(__))) is string source)
						question.Source = source.SplitTrim(Utils.Inputs.CodebookPDF.ProcessCodebooksInputEnglishSplit_Source)[1];

					if (texts.FirstOrDefault(_ => Utils.Inputs.CodebookPDF.ProcessCodebooksInputEnglishSplit_Note.Any(__ => _.Contains(__))) is string note)
						question.Note = note.SplitTrim(Utils.Inputs.CodebookPDF.ProcessCodebooksInputEnglishSplit_Note)[1];

					yield return question;
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
