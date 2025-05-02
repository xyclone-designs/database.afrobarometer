using Database.Afrobarometer.Enums;
using Database.Afrobarometer.Inputs;
using Database.Afrobarometer.Tables;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

using SpsslyVariable = Spssly.SpssDataset.Variable;

namespace Database.Afrobarometer
{
	internal partial class Program
	{
		public static void Process(Args args)
		{
			Console.WriteLine("Process...");

			string allerrorfilepath = Path.Combine(DirectoryOutputProcess, "_all.error.txt");

			using FileStream allerrorfilestream = File.Create(allerrorfilepath);
			using StreamWriter allerrorstreamwriter = new(allerrorfilestream);

			IEnumerable<InputContainer> surveys = args.Inputs.Where(_ =>
			{
				return _.InputType == InputTypes.SurveySAV && args.Rounds.Contains(_.Round) && args.Languages.Contains(_.Language);
			});
			IEnumerable<InputContainer> codebooks = args.Inputs.Where(_ =>
			{
				return _.InputType == InputTypes.CoebookPDF && args.Rounds.Contains(_.Round) && args.Languages.Contains(_.Language);
			});

			foreach (IGrouping<Rounds, InputContainer> surveygroupround in surveys.GroupBy(_ => _.Round))
				if (surveygroupround.FirstOrDefault() is InputContainer surveyroundcontainer &&
					codebooks.FirstOrDefault(_ => _.Round == surveyroundcontainer.Round) is InputContainer codebookroundcontainer)
				{
					string roundname = string.Format("round-{0:00}", surveygroupround.Key.ToInt());
					string rounddirectory = Path.Combine(DirectoryOutputProcess, roundname);
					string rounderrorfilename = string.Format("_{0}.error.txt", roundname);
					string rounderrorfilepath = Path.Combine(rounddirectory, rounderrorfilename);

					if (Directory.Exists(rounddirectory) is false) Directory.CreateDirectory(rounddirectory);

					using FileStream rounderrorfilestream = File.Create(rounderrorfilepath);
					using StreamWriter rounderrorstreamwriter = new(rounderrorfilestream);

					using FileStream surveygrouproundfilestream = File.OpenRead(surveyroundcontainer.ZipPath);
					using FileStream codebookgrouproundfilestream = File.OpenRead(codebookroundcontainer.ZipPath);

					using ZipArchive surveygrouproundziparchive = new(surveygrouproundfilestream);
					using ZipArchive codebookgrouproundziparchive = new(codebookgrouproundfilestream);

					foreach (IGrouping<Languages, InputContainer> grouplanguage in surveygroupround.GroupBy(_ => _.Language))
						foreach (InputContainer surveycontainer in grouplanguage)
							if (codebooks.FirstOrDefault(_ =>
							{
								return
									surveycontainer.Round == _.Round &&
									surveycontainer.Country == _.Country &&
									surveycontainer.Language == _.Language;

							}) is not InputContainer codebookcontainer)
							{
								rounderrorstreamwriter.WriteLine("Language {0}, Country {1},  'codebook not found'", surveycontainer.Language, surveycontainer.Country);
								allerrorstreamwriter.WriteLine(
									"Round {0}, Language {1}, Country {2},  'codebook not found'",
									surveycontainer.Round,
									surveycontainer.Language,
									surveycontainer.Country);
							}
							else
							{
								Console.WriteLine("Round {0}, Language {1}, Country {2}", surveycontainer.Round.ToString(), surveycontainer.Language, surveycontainer.Country);

								ZipArchiveEntry? surveyentry = surveygrouproundziparchive.Entries.FirstOrDefault(_ => _.FullName == surveycontainer.ZipFullName);
								ZipArchiveEntry? codebookentry = codebookgrouproundziparchive.Entries.FirstOrDefault(_ => _.FullName == codebookcontainer.ZipFullName);

								string? surveytempfilepath = surveyentry?.ExtractToDirectory(DirectoryTemp);
								string? codeboooktempfilepath = codebookentry?.ExtractToDirectory(DirectoryTemp);

								SurveySAV? surveysav = surveytempfilepath is null ? null : new(surveytempfilepath);
								CodebookPDF? codebook = codeboooktempfilepath is null ? null : new(codeboooktempfilepath);

								if (surveysav is null && codebook is null) { }
								else if (surveysav is null)
								{
									allerrorstreamwriter.WriteLine("Zip '{0}' does not contain survey file '{1}'", surveycontainer.ZipPath.Split('/')[^1], surveycontainer.ZipFullName);
									rounderrorstreamwriter.WriteLine("Zip '{0}' does not contain survey file '{1}'", surveycontainer.ZipPath.Split('/')[^1], surveycontainer.ZipFullName);
								}
								else if (codebook is null)
								{
									allerrorstreamwriter.WriteLine("Zip '{0}' does not contain codebook file '{1}'", surveycontainer.ZipPath.Split('/')[^1], surveycontainer.ZipFullName);
									rounderrorstreamwriter.WriteLine("Zip '{0}' does not contain codebook file '{1}'", surveycontainer.ZipPath.Split('/')[^1], surveycontainer.ZipFullName);
								}
								else
								{
									string processlogfilename = string.Format("{0}.{1}.log.txt", surveysav.Country, surveysav.Language).ToLower();
									string processlogfilepath = Path.Combine(rounddirectory, processlogfilename);

									using FileStream processlogfilestream = File.Create(processlogfilepath);
									using StreamWriter processlogstreamwriter = new(processlogfilestream);

									List<CodebookPDF.Question> codebookquestions = codebook.Questions
										.Select(_ => { processlogstreamwriter.WriteLine("Question: [{0}] {1}", _.Number, _.VariableLabel); return _; })
										.ToList();

									foreach (SpsslyVariable variable in surveysav.Variables)
									{
										Console.WriteLine(variable.Name, variable.Label);

										if (codebookquestions.FirstOrDefault(_ => _.Number == variable.Name) is CodebookPDF.Question question)
										{
											processlogstreamwriter.WriteLine("Variable: [{0}] '{1}'", variable.Name, variable.Label);
											processlogstreamwriter.WriteLine("Question: [{0}] '{1}'", question.Number, question.VariableLabel);
											processlogstreamwriter.WriteLine("    {0}", question.Text);
											processlogstreamwriter.WriteLine();
										}
										else
										{
											processlogstreamwriter.WriteLine("Variable: [{0}] '{1}'", variable.Name, variable.Label);
											processlogstreamwriter.WriteLine("    No Question Found");
											processlogstreamwriter.WriteLine();
										}
									}
								}

								codebook?.Dispose();
								surveysav?.Dispose();
							}
				}
		}
		public static void ProcessCodebooks(Args args)
		{
			Console.WriteLine("ProcessCodebooks...");

			Dictionary<string, IList<string>> keys = [], questions = [];

			foreach (IGrouping<string, InputContainer> ziparchive in args.Inputs.Where(_ => _.InputType == InputTypes.CoebookPDF).GroupBy(_ => _.ZipPath))
			{
				string ziparchiveoutputdirectory = Path.Combine(DirectoryOutputCodebooks, ziparchive.Key.Split('\\')[^1].Replace(".zip", ""));

				if (Directory.Exists(ziparchiveoutputdirectory) is false) Directory.CreateDirectory(ziparchiveoutputdirectory);

				using FileStream datastream = File.OpenRead(ziparchive.Key);
				using ZipArchive datazip = new(datastream);

				List<string> _questiontexts = [], _questionnumbers = [], _variablelabels = [];

				foreach (string tempfilepath in datazip.Entries.OrderMergeLast().ExtractToDirectoryIterable(DirectoryTemp))
				{
					string codebookname = tempfilepath.Split('\\')[^1];

					CodebookPDF codebook = new(tempfilepath);

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

					foreach (CodebookPDF.Question question in codebook.Questions)
					{
						Console.WriteLine("Question Number: {0}", question.Number);

						question.Log(codebooklogstreamwriter);
						question.LogError(codebookerrorstreamwriter);

						codebookoperationsstreamwriter.WriteLine("Question Number: {0}", question.Number);
						codebookoperationsstreamwriter.WriteLine("Variable Label: {0}", question.VariableLabel);
						codebookoperationsstreamwriter.WriteLine("Question Text: {0}", question.Text);

						if (question.Text is null)
							codebookoperationsstreamwriter.WriteLine("    Skipping: 'Question Text is null'");
						else
						{
							string? key = null, value = null;
							//UtilAddQuestion(question, questions, keys, out string? key, out string? value);

							codebookoperationsstreamwriter.WriteLine("    Variable Label: '{0}'", key);
							codebookoperationsstreamwriter.WriteLine("    Question Text: '{0}'", value);

							if (key is not null) question.VariableLabel = key;
							if (value is not null) question.Text = value;

							if (question.Text is not null) _questiontexts.Add(question.Text);
							if (question.VariableLabel is not null) _variablelabels.Add(question.VariableLabel);
							if (question.Number is not null) _questionnumbers.Add(question.Number);
						}

						codebookoperationsstreamwriter.WriteLine();
					}
					
					File.Delete(tempfilepath);
					Console.WriteLine();
				}

				string _questiontextsfilepath = Path.Combine(ziparchiveoutputdirectory, "_questiontexts.txt");
				string _variablelabelsfilepath = Path.Combine(ziparchiveoutputdirectory, "_variablelabels.txt");
				string _questionnumbersfilepath = Path.Combine(ziparchiveoutputdirectory, "_questionnumbers.txt");

				using FileStream _questiontextsfilestream = File.Create(_questiontextsfilepath);
				using FileStream _variablelabelsfilestream = File.Create(_variablelabelsfilepath);
				using FileStream _questionnumbersfilestream = File.Create(_questionnumbersfilepath);

				using StreamWriter _questiontextsstreamwriter = new(_questiontextsfilestream);
				using StreamWriter _variablelabelsstreamwriter = new(_variablelabelsfilestream);
				using StreamWriter _questionnumbersstreamwriter = new(_questionnumbersfilestream);

				foreach (string _questiontext in _questiontexts.Distinct().Order())
					_questiontextsstreamwriter.WriteLine(_questiontext);
				
				foreach (string _variablelabel in _variablelabels.Distinct().Order())
					_variablelabelsstreamwriter.WriteLine(_variablelabel);
				
				foreach (string _questionnumber in _questionnumbers.Distinct().Order())
					_questionnumbersstreamwriter.WriteLine(_questionnumber);
			}
		}
		public static void ProcessSurveys(Args args)
		{
			Console.WriteLine("ProcessSurveys...");

			List<Variable> allvariables = [];
			List<Variable> roundvariables = [];

			Dictionary<string, List<string>> allvariableslabels = [];
			Dictionary<string, int> allvariableslabelscount = [];

			foreach (IGrouping<string, InputContainer> ziparchive in args.Inputs.Where(_ =>
			{
				return
					_.InputType == InputTypes.SurveySAV &&
					_.Round == _.Round &&
					_.Country == _.Country &&
					_.Language == _.Language;

			}).GroupBy(_ => _.ZipPath))
			{
				string ziparchiveoutputdirectory = Path.Combine(DirectoryOutputSurveys, ziparchive.Key.Split('\\')[^1].Replace(".zip", ""));

				if (Directory.Exists(ziparchiveoutputdirectory) is false) Directory.CreateDirectory(ziparchiveoutputdirectory);

				using FileStream datastream = File.OpenRead(ziparchive.Key);
				using ZipArchive datazip = new(datastream);

				foreach (string tempfilepath in datazip.Entries.OrderMergeLast().ExtractToDirectoryIterable(DirectoryTemp))
				{
					string surveysavname = tempfilepath.Split('\\')[^1];
					SurveySAV surveysav = new(tempfilepath);

					Console.WriteLine(surveysavname);

					string surveysavlogfilename = surveysavname.Replace(".sav", ".log.txt");
					string surveysaverrorfilename = surveysavname.Replace(".sav", ".error.txt");
					string surveysavoperationsfilename = surveysavname.Replace(".sav", ".operations.txt");

					string surveysavlogfilepath = Path.Combine(ziparchiveoutputdirectory, surveysavlogfilename);
					string surveysaverrorfilepath = Path.Combine(ziparchiveoutputdirectory, surveysaverrorfilename);
					string surveysavoperationsfilepath = Path.Combine(ziparchiveoutputdirectory, surveysavoperationsfilename);

					using FileStream surveysavlogfilestream = File.Create(surveysavlogfilepath);
					using FileStream surveysaverrorfilestream = File.Create(surveysaverrorfilepath);
					using FileStream surveysavoperationsfilestream = File.Create(surveysavoperationsfilepath);

					using StreamWriter surveysavlogstreamwriter = new(surveysavlogfilestream);
					using StreamWriter surveysaverrorstreamwriter = new(surveysaverrorfilestream);
					using StreamWriter surveysavoperationsstreamwriter = new(surveysavoperationsfilestream);

					foreach (SpsslyVariable spsslyvariable in surveysav.Variables)
					{
						Console.WriteLine("Variable Name: {0}", spsslyvariable.Name);

						if (string.IsNullOrWhiteSpace(spsslyvariable.Label))
							continue;

						string operation = string.Format("{0}: '{1}'", spsslyvariable.Name, spsslyvariable.Label);
						string labelcleaned = Utils.Inputs.Variable.CleanLabel(spsslyvariable.Label);
						allvariableslabels.Add(labelcleaned, out string key, out string value);
						if (spsslyvariable.Label != labelcleaned)
							operation = string.Format("{0} => '{1}'", operation, labelcleaned);
						if (labelcleaned != value)
							operation = string.Format("{0} => '{1}'", operation, value);

						Variable variable = new(spsslyvariable)
						{
							Label = value,
							ValueLabelsDictionary = spsslyvariable.ValueLabels
								.ToDictionary(_ => _.Key, _ => Utils.Inputs.Variable.CleanValueLabel(_.Value))
						};

						if (allvariables.Where(_ => variable.Label.EqualsOrdinalIgnoreCase(_.Label)) is IEnumerable<Variable> _allvariables && _allvariables.Any())
						{
							for (int index = 0; true; index++)
							{
								if (_allvariables.ElementAtOrDefault(index) is not Variable _allvariable)
								{
									allvariables.Add(variable);
									allvariableslabelscount.TryAdd(variable.Label, 1);
									break;
								}
								else if (variable.ValueLabels?.EqualsOrdinalIgnoreCase(_allvariable.ValueLabels) ?? false)
								{
									allvariableslabelscount[variable.Label]++;
									break;
								}
								else if (variable.ValueLabelsDictionary.All(_ => _allvariable.ValueLabelsDictionary.TryGetValue(_.Key, out string? _value) && _.Value.EqualsOrdinalIgnoreCase(_value)))
								{
									variable.ValueLabelsDictionary = _allvariable.ValueLabelsDictionary;
									allvariableslabelscount[variable.Label]++;
									break;
								}
								else if (_allvariable.ValueLabelsDictionary.All(_ => variable.ValueLabelsDictionary.TryGetValue(_.Key, out string? _value) && _.Value.EqualsOrdinalIgnoreCase(_value)))
								{
									_allvariable.ValueLabelsDictionary = variable.ValueLabelsDictionary;
									allvariableslabelscount[variable.Label]++;
									break;
								}
								else if (variable.ValueLabelsDictionary.Keys
									.Where(_ => _allvariable.ValueLabelsDictionary.ContainsKey(_))
									.All(_ => _allvariable.ValueLabelsDictionary[_].EqualsOrdinalIgnoreCase(variable.ValueLabelsDictionary[_])))
								{
									foreach (double variablekey in variable.ValueLabelsDictionary.Keys)
										_allvariable.ValueLabelsDictionary.TryAdd(variablekey, variable.ValueLabelsDictionary[variablekey]);

									variable.ValueLabelsDictionary = _allvariable.ValueLabelsDictionary;
									allvariableslabelscount[variable.Label]++;
									break;
								}
							}
						}
						else
						{
							allvariables.Add(variable);
							allvariableslabelscount.TryAdd(variable.Label, 1);

						} 
						
						roundvariables.Add(variable);
						variable.Log(surveysavlogstreamwriter);
						variable.LogError(surveysaverrorstreamwriter);

						surveysavoperationsstreamwriter.WriteLine(operation);
					}

					surveysav.Dispose();
					roundvariables.Clear();

					File.Delete(tempfilepath);
					Console.WriteLine();
				}

				string _roundvariablelabelsfilepath = Path.Combine(ziparchiveoutputdirectory, "_variables.txt");
				using FileStream _roundvariablelabelsfilestream = File.Create(_roundvariablelabelsfilepath);
				using StreamWriter _roundvariablelabelsstreamwriter = new(_roundvariablelabelsfilestream);

				foreach (Variable value in roundvariables.OrderBy(_ => _.Label))
					_roundvariablelabelsstreamwriter.WriteLine(
						"({1}) [{0}]: {2} {3}", value.Label, value.Label is null || allvariableslabelscount.TryGetValue(value.Label, out int _count) is false ? "?" : _count,
						string.Join("\n    ", value.ValueLabelsDictionary.Select(_ => string.Format("{0}: {1}", _.Key, _.Value))));

				Console.WriteLine();
			}

			string _allvariablelabelsfilepath = Path.Combine(DirectoryOutputSurveys, "_variables.txt");
			using FileStream _allvariablelabelsfilestream = File.Create(_allvariablelabelsfilepath);
			using StreamWriter _allvariablelabelsstreamwriter = new(_allvariablelabelsfilestream);

			foreach (Variable value in allvariables.OrderBy(_ => _.Label))
				_allvariablelabelsstreamwriter.WriteLine(
					"({1}) [{0}]: {2} {3}", value.Label, value.Label is null || allvariableslabelscount.TryGetValue(value.Label, out int _count) is false ? "?" : _count, 
					value.ValueLabelsDictionary.Any() ? "\n   " : string.Empty,
					string.Join("\n    ", value.ValueLabelsDictionary.Select(_ => string.Format("{0}: {1}", _.Key, _.Value)) ?? Enumerable.Empty<string>()));
		}
	}
}
