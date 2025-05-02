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

				string[] split = Utils.Inputs.Codebook.SplitText(pdfdocument, out string text); Text = text;

				for (int index = 0; index < split.Length; index++)
				{
					string[] texts = split[index].Split(Utils.Inputs.Codebook.ProcessCodebooksInputEnglishSplit, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

					yield return new Question
					{
						Number = Utils.Inputs.Question.QuestionNumber(texts.ElementAtOrDefault(0)),
						Text = Utils.Inputs.Question.QuestionText(texts.ElementAtOrDefault(1)),
						VariableLabel = Utils.Inputs.Question.VariableLabel(texts.ElementAtOrDefault(2)),
						Values = Utils.Inputs.Question.Values(texts.ElementAtOrDefault(3)),
						ValueLabels = Utils.Inputs.Question.ValueLabels(texts.ElementAtOrDefault(4)),
						Source = Utils.Inputs.Question.Source(texts.ElementAtOrDefault(5)),
						Note = Utils.Inputs.Question.Note(texts.ElementAtOrDefault(6)),
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
			public string? Number { get; set; }
			public string? Text { get; set; }
			public int? PkVariable { get; set; }
			public string? VariableLabel { get; set; }
			public string[]? Values { get; set; }
			public string[]? ValueLabels { get; set; }
			public string? Source { get; set; }
			public string? Note { get; set; }

			public void Log(StreamWriter streamwriter)
			{
				streamwriter.WriteLine("QuestionNumber: {0}", Number);
				streamwriter.WriteLine("Question: {0}", Text);
				streamwriter.WriteLine("VariableLabel: {0}", VariableLabel);
				streamwriter.WriteLine("Values: {0}", Values?.ElementAtOrDefault(0));

				if (Values is not null && Values.Length > 2)
					for (int index = 1; index < Values.Length; index++)
						streamwriter.WriteLine("        {0}", Values[index]);

				streamwriter.WriteLine("ValueLabels: {0}", ValueLabels?.ElementAtOrDefault(0));

				if (ValueLabels is not null && ValueLabels.Length > 2)
					for (int index = 1; index < ValueLabels.Length; index++)
						streamwriter.WriteLine("             {0}", ValueLabels[index]);

				streamwriter.WriteLine("Source: {0}", Source);
				streamwriter.WriteLine("Note: {0}", Note);
				streamwriter.WriteLine();
			}
			public void LogError(StreamWriter streamwriter)
			{
				if (Number is null || Regex.IsMatch(Number, "Q[0-9]+[A-za-z]?\\s"))
					streamwriter.WriteLine("QuestionNumber: {0}", Number);

				if (ValueLabels?.Where(_ =>
				{
					return
						Utils.Inputs.Question.ValueLabels_NotApplicables.Contains(_) is false &&
						_.Count(_ => _ == '=') >= 2;

				}) is IEnumerable<string> valuelables && valuelables.Any())
				{
					streamwriter.WriteLine("ValueLabels: {0}", valuelables.ElementAtOrDefault(0));
					for (int index = 1; valuelables.ElementAtOrDefault(index) is string valuelable; index++)
						streamwriter.WriteLine("             {0}", valuelable);
				}				
			}
		}
	}
}
