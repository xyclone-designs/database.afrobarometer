using Database.Afrobarometer.Enums;
using Database.Afrobarometer.Inputs;
using Database.Afrobarometer.Tables;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Database.Afrobarometer
{
	internal partial class Program
	{
		public class ProcessArgs
		{
			public ProcessArgs(params string[] ziparchives) { ZipArchives = ziparchives; }

			public Languages[] Languages { get; set; } = Enum.GetValues<Languages>();
			public Rounds[] Rounds { get; set; } = Enum.GetValues<Rounds>();
			public string[] ZipArchives { get; set; } = [];
		}

		public static void ProcessCodebooks(ProcessArgs args)
		{
			Dictionary<string, IList<string>> keys = [], questions = [];

			foreach (string ziparchive in args.ZipArchives)
			{
				string ziparchiveoutputdirectory = Path.Combine(DirectoryOutputCodebooks, ziparchive.Split('\\')[^1].Replace(".zip", ""));

				if (Directory.Exists(ziparchiveoutputdirectory) is false) Directory.CreateDirectory(ziparchiveoutputdirectory);

				using FileStream datastream = File.OpenRead(ziparchive);
				using ZipArchive datazip = new(datastream);

				List<string> _variablelabels = [], _questiontexts = [];

				foreach (string tempfilepath in datazip.Entries.OrderBy(
					_ => _.Name,
					Comparer<string>.Create((one, two) => true switch
					{
						true when one.Contains("merge") => +1,
						true when two.Contains("merge") => -1,

						_ => Comparer<string>.Default.Compare(one, two)

					})).ExtractToDirectoryIterable(DirectoryTemp))
				{
					string codebookname = tempfilepath.Split('\\')[^1];

					CodebookPDF codebook = new(tempfilepath);

					bool valid = args.Rounds.Contains(codebook.Round) && args.Languages.Contains(codebook.Language);

					if (valid is false)
						continue;

					Console.WriteLine(codebookname);

					string codebooklogfilename = codebookname.Replace(".pdf", ".log.txt");
					string codebookerrorfilename = codebookname.Replace(".pdf", ".error.txt");
					string codebookoperationsfilename = codebookname.Replace(".pdf", ".operations.txt");

					string codebooklogfilepath = Path.Combine(ziparchiveoutputdirectory, codebooklogfilename);
					string codebookerrorfilepath = Path.Combine(ziparchiveoutputdirectory, codebookerrorfilename);
					string codebookoperationsfilepath = Path.Combine(ziparchiveoutputdirectory, codebookoperationsfilename);

					using FileStream codebooklogfilestream = File.Create(codebooklogfilepath);
					using FileStream codebookerrorfilestream = File.Create(codebookerrorfilepath);
					using FileStream codebookoperationsfilestream = File.Create(codebookoperationsfilepath);

					using StreamWriter codebooklogstreamwriter = new(codebooklogfilestream);
					using StreamWriter codebookerrorstreamwriter = new(codebookerrorfilestream);
					using StreamWriter codebookoperationsstreamwriter = new(codebookoperationsfilestream);

					foreach (Question question in codebook.Questions)
					{
						Console.WriteLine("Question Number: {0}", question.QuestionNumber);

						question.Log(codebooklogstreamwriter);
						if (question.LogErrors(codebookerrorstreamwriter))
							codebookerrorstreamwriter.WriteLine();

						codebookoperationsstreamwriter.WriteLine("Question Number: {0}", question.QuestionNumber);
						codebookoperationsstreamwriter.WriteLine("Variable Label: {0}", question.VariableLabel);
						codebookoperationsstreamwriter.WriteLine("Question Text: {0}", question.QuestionText);

						if (question.QuestionText is null)
							codebookoperationsstreamwriter.WriteLine("    Skipping: 'Question Text is null'");
						else
						{
							string? key = null, value = null;
							//UtilAddQuestion(question, questions, keys, out string? key, out string? value);

							codebookoperationsstreamwriter.WriteLine("    Variable Label: '{0}'", key);
							codebookoperationsstreamwriter.WriteLine("    Question Text: '{0}'", value);

							if (key is not null) question.VariableLabel = key;
							if (value is not null) question.QuestionText = value;

							if (question.QuestionText is not null) _questiontexts.Add(question.QuestionText);
							if (question.VariableLabel is not null) _variablelabels.Add(question.VariableLabel);
						}

						codebookoperationsstreamwriter.WriteLine();
					}

					File.Delete(tempfilepath);
					Console.WriteLine();
				}

				string _questiontextsfilepath = Path.Combine(ziparchiveoutputdirectory, "_questiontexts.txt");
				string _variablelabelsfilepath = Path.Combine(ziparchiveoutputdirectory, "_variablelabels.txt");

				using FileStream _questiontextsfilestream = File.Create(_questiontextsfilepath);
				using FileStream _variablelabelsfilestream = File.Create(_variablelabelsfilepath);

				using StreamWriter _questiontextsstreamwriter = new(_questiontextsfilestream);
				using StreamWriter _variablelabelsstreamwriter = new(_variablelabelsfilestream);

				foreach (string _questiontext in _questiontexts.Distinct().Order())
					_questiontextsstreamwriter.WriteLine(_questiontext);
				
				foreach (string _variablelabel in _variablelabels.Distinct().Order())
					_variablelabelsstreamwriter.WriteLine(_variablelabel);
			}
		}
	}
}
