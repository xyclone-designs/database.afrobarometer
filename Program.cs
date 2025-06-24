using Database.Afrobarometer.Enums;
using Database.Afrobarometer.Inputs;

using ICSharpCode.SharpZipLib.GZip;

using Newtonsoft.Json.Linq;

using SQLite;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

using XycloneDesigns.Apis.General.Tables;
using XycloneDesigns.Apis.Afrobarometer.Enums;
using XycloneDesigns.Apis.Afrobarometer.Tables;

using ISpsslyRawRecord = Spssly.SpssDataset.IRawRecord;
using SurveySAVVariable = Spssly.SpssDataset.Variable;
using _TableGeneral = XycloneDesigns.Apis.General.Tables._Table;

namespace Database.Afrobarometer
{
	internal partial class Program
	{
		public class Args
		{
			public Args() { }
			public Args(string[] args) { }

			public ZipArchiveContainer[] Inputs { get; set; } = [];
			public string[] CountryCodes { get; set; } = Country.Codes._All;
			public string[] LanguageCodes { get; set; } = Language.Codes._All;
			public Rounds[] Rounds { get; set; } = Enum.GetValues<Rounds>();
		}

		//static readonly string DirectoryCurrent = Directory.GetCurrentDirectory();
		static readonly string DirectoryCurrent = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName!;

		static readonly string DirectoryOutput = Path.Combine(DirectoryCurrent, ".output");
		static readonly string DirectoryTemp = Path.Combine(DirectoryCurrent, ".temp");
		static readonly string DirectoryInput = Path.Combine(DirectoryCurrent, ".input");
		static readonly string DirectoryInputCodebooks = Path.Combine(DirectoryInput, "codebooks");
		static readonly string DirectoryInputDatabases = Path.Combine(DirectoryInput, "databases");
		static readonly string DirectoryInputSurveys = Path.Combine(DirectoryInput, "surveys");
		static readonly string DirectoryInputTopics = Path.Combine(DirectoryInput, "topics");

		static string DirectoryOutputFolder(Rounds? round, string? country)
		{
			string directoryoutputfolder = true switch
			{
				true when round.HasValue && country is not null => string.Format("{0}.{1}", round.Value.AsString(), country),
				true when round.HasValue => round.Value.AsString(),
				true when country is not null => country,

				_ => string.Empty
			};

			return directoryoutputfolder.ToLower();
		}

		static void Main(string[] args)
		{
			_CleaningPre();

			using FileStream filestream_countries = File.OpenRead(Path.Combine(DirectoryInputDatabases, "countries.db.zip"));
			using FileStream filestream_languages = File.OpenRead(Path.Combine(DirectoryInputDatabases, "languages.db.zip"));
			using FileStream filestream_municipalities = File.OpenRead(Path.Combine(DirectoryInputDatabases, "municipalities.db.zip"));
			using FileStream filestream_provinces = File.OpenRead(Path.Combine(DirectoryInputDatabases, "provinces.db.zip"));

			string sqlconnectionpathcountries = Path.Combine(DirectoryTemp, "countries.db");
			string sqlconnectionpathlanguages = Path.Combine(DirectoryTemp, "languages.db");
			string sqlconnectionpathmunicipalities = Path.Combine(DirectoryTemp, "municipalities.db");
			string sqlconnectionpathprovinces = Path.Combine(DirectoryTemp, "provinces.db");

			new ZipArchive(filestream_countries).ExtractToDirectory(DirectoryTemp);
			new ZipArchive(filestream_languages).ExtractToDirectory(DirectoryTemp);
			new ZipArchive(filestream_municipalities).ExtractToDirectory(DirectoryTemp);
			new ZipArchive(filestream_provinces).ExtractToDirectory(DirectoryTemp);

			SQLiteConnection? sqliteconnection_countries = File.Exists(sqlconnectionpathcountries) ? new (sqlconnectionpathcountries) : null;
			SQLiteConnection? sqliteconnection_languages = File.Exists(sqlconnectionpathlanguages) ? new (sqlconnectionpathlanguages) : null;
			SQLiteConnection? sqliteconnection_municipalities = File.Exists(sqlconnectionpathmunicipalities) ? new (sqlconnectionpathmunicipalities) : null;
			SQLiteConnection? sqliteconnection_provinces = File.Exists(sqlconnectionpathprovinces) ? new (sqlconnectionpathprovinces) : null;

			sqliteconnection_countries?.CreateTable<Country>();
			sqliteconnection_languages?.CreateTable<Language>();
			sqliteconnection_municipalities?.CreateTable<Municipality>();
			sqliteconnection_provinces?.CreateTable<Province>();

			string sqlconnectionpath = Path.Combine(DirectoryOutput, "afrobarometer.db");

			SQLiteConnection sqliteconnection = _SQLiteConnection(sqlconnectionpath, false);

			JArray apifiles = [];
			StreamWriters streamwriters = [];
			Args _args = new(args)
			{
				LanguageCodes = [Language.Codes.English],
				Inputs = ZipArchiveContainer
					.FromZipPaths(DirectoryInputSurveys, DirectoryInputCodebooks)
					.ToArray(),
			};

			IEnumerable<ZipArchiveContainer> inputs = _args.Inputs.Where(_ =>
			{
				return
					_.InputType == InputTypes.SurveySAV &&
					_args.Rounds.Contains(_.Round) &&
					_args.CountryCodes.Contains(_.CountryCode) &&
					_args.LanguageCodes.Contains(_.LanguageCode);
			});

			foreach (IGrouping<string, ZipArchiveContainer> surveysziparchivecontainergrouping in inputs.GroupBy(_ => _.ZipPath))
			{
				ZipArchiveContainer surveysziparchivecontainer = surveysziparchivecontainergrouping.First();

				string round = surveysziparchivecontainer.Round.AsString();

				using FileStream surveysstream = File.OpenRead(surveysziparchivecontainer.ZipPath);
				using ZipArchive surveyszip = new(surveysstream);
				
				ZipArchiveContainer? codebookssziparchivecontainer = _args.Inputs.FirstOrDefault(_ =>
				{
					return
						_.InputType == InputTypes.CodebookPDF &&
						_.Round == default(Rounds).FromFilename(surveysziparchivecontainer.ZipPath);
				});

				using FileStream? codebooksstream = codebookssziparchivecontainer is null ? null : File.OpenRead(codebookssziparchivecontainer.ZipPath);
				using ZipArchive? codebookszip = codebooksstream is null ? null : new(codebooksstream);

				StreamWriters streamwritersround = [];
				Directory.CreateDirectory(streamwritersround.PathBase = Path.Combine(DirectoryOutput, DirectoryOutputFolder(surveysziparchivecontainer.Round, null)));
				string sqliteconnectionroundpath = Path.Combine(streamwritersround.PathBase, string.Format("afrobarometer.{0}.db", round));
				SQLiteConnection sqliteconnectionround = _SQLiteConnection(sqliteconnectionroundpath, true);

				foreach (ZipArchiveContainer inputcontainer in surveysziparchivecontainergrouping)
				{
					if (surveyszip.GetEntry(inputcontainer.ZipFullName)?.ExtractToDirectory(DirectoryTemp) is not string surveytempfilepath)
						continue;

					string? codebooktempfilepath = codebookszip?.Entries
						.FirstOrDefault(_ =>
						{
							return
								default(Rounds).FromFilename(_.Name) == inputcontainer.Round &&
								_.Name.FindCountryCode() == inputcontainer.CountryCode;

						})?.ExtractToDirectory(DirectoryTemp);

					string surveysavname = surveytempfilepath.Split('\\')[^1];
					string? codebookpdfname = codebooktempfilepath?.Split('\\')[^1];

					Console.WriteLine("\n{0}\n{1}\n", surveysavname, codebookpdfname);

					string directoryoutputfolder = DirectoryOutputFolder(inputcontainer.Round, inputcontainer.CountryCode);
					string directoryoutputpath = Path.Combine(DirectoryOutput, directoryoutputfolder);

					Directory.CreateDirectory(streamwriters.PathBase = directoryoutputpath);

					SurveySAV surveysav = new(surveytempfilepath);
					Survey survey = sqliteconnection.InsertAndReturn(new Survey
					{
						InterviewCount = surveysav.Records.Count(),
						Round = inputcontainer.Round,
						PkCountry = sqliteconnection_countries?.Table<Country>().FirstOrDefault(_ => _.Code == surveysav.CountryCode)?.Pk,
						PkLanguage = sqliteconnection_languages?.Table<Language>().FirstOrDefault(_ => _.Code == surveysav.LanguageCode)?.Pk,
					});

					List<Variable> variables = [];
					List<Question> questions = [];
					List<Interview> interviews = [];

					IEnumerable<Variable> variablesupdate = Enumerable.Empty<Variable>(), variablesinsert = Enumerable.Empty<Variable>();
					IEnumerable<Question> questionsupdate = Enumerable.Empty<Question>(), questionsinsert = Enumerable.Empty<Question>();

					void _Variables()
					{
						streamwriters.PathBase ??= directoryoutputpath;
						streamwriters.TryAdd("table.variable.label", "table.variable.label.txt", true);

						variables = ProcessSurveySAV(surveysav, new ProcessArgs(
							streamwriters: streamwriters,
							sqliteconnection: sqliteconnection,
							languagecode: inputcontainer.LanguageCode));

						variablesupdate = variables.Where(_ => _.Pk != 0);
						variablesinsert = variables.Where(_ => _.Pk == 0);

						sqliteconnection
							.UpdateAll(variablesupdate, out int _)
							.InsertAll(variablesinsert, out int _)
							.Commit();

						variables.RemoveAll(_ => _.Pk == 0);
						variables.AddRange(sqliteconnection
							.Table<Variable>()
							.TakeLast(variablesinsert.Count())
							.ToList());

						foreach (Variable _variable in variables)
							survey.List_PkVariable = _TableGeneral.AddPKIfUnique(survey.List_PkVariable, _variable.Pk);

						foreach (Variable variable in variables.OrderBy(_ => _.Label))
							streamwriters.PerformAction(_ => _.WriteLine("{0} [{1}]: {2}", variable.Pk, variable.Id, variable.Label), "table.variable.label");

						IEnumerable<Variable> variablesround = sqliteconnectionround.Table<Variable>();

						variablesupdate = variablesround.Where(_ => variables.Any(__ => __.Id == _.Id) is true);
						variablesinsert = variablesround.Where(_ => variables.Any(__ => __.Id == _.Id) is false);

						sqliteconnectionround
							.UpdateAll(variablesupdate, out int _)
							.InsertAll(variablesinsert, out int _)
							.Commit();
					}
					void _Questions()
					{
						streamwriters.PathBase ??= directoryoutputpath;

						if (codebooktempfilepath is null)
							File.Create(Path.Combine(streamwriters.PathBase, "codebookpdf.notfound.txt"));

						if (codebooktempfilepath is not null)
						{
							streamwriters.TryAdd("table.question.text", "table.question.text.txt", true);
							streamwriters.TryAdd("table.question.variablelabel", "table.question.variablelabel.txt", true);

							CodebookPDF codebookpdf = new(codebooktempfilepath);
							ProcessArgs processargs = new(sqliteconnection, inputcontainer.LanguageCode, streamwriters)
							{
								OnQuestion = question =>
								{
									question.List_PkSurvey = _TableGeneral.AddPKIfUnique(question.List_PkSurvey, survey.Pk);
								}
							};

							questions = ProcessCodebookPDF(codebookpdf, processargs, out Dictionary<string, Variable> _variables)
								.Select(_ =>
								{
									if (_.Text is not null && _variables.TryGetValue(_.Text, out Variable? variable) && variable is not null)
										_.PkVariable ??= variable.Pk;

									return _;

								}).ToList();

							questionsupdate = questions.Where(_ => _.Pk != 0);
							questionsinsert = questions.Where(_ => _.Pk == 0);

							sqliteconnection
								.UpdateAll(variablesupdate, out int _)
								.InsertAll(variablesinsert, out int _)
								.UpdateAll(questionsupdate, out int _)
								.InsertAll(questionsinsert, out int _)
								.Commit();

							questions.RemoveAll(_ => _.Pk == 0);
							questions.AddRange(sqliteconnection
								.Table<Question>()
								.TakeLast(questionsinsert.Count())
								.ToList());

							foreach (Question _question in questions)
								survey.List_PkQuestion = _TableGeneral.AddPKIfUnique(survey.List_PkQuestion, _question.Pk);

							foreach (Question question in questions.OrderBy(_ => _.Text))
								streamwriters.PerformAction(_ => _.WriteLine("{0} [{1}]: {2}", question.Id, question.Pk, question.VariableLabel), "table.question.text");

							foreach (Question question in questions.OrderBy(_ => _.VariableLabel))
								streamwriters.PerformAction(_ => _.WriteLine("{0} [{1}]: {2}", question.Id, question.Pk, question.VariableLabel), "table.question.variablelabel");

							IEnumerable<Question> questionsround = sqliteconnectionround.Table<Question>();

							questionsupdate = questionsround.Where(_ => questions.Any(__ => __.Id == _.Id) is true);
							questionsinsert = questionsround.Where(_ => questions.Any(__ => __.Id == _.Id) is false);

							sqliteconnectionround
								.UpdateAll(questionsupdate, out int _)
								.InsertAll(questionsinsert, out int _)
								.Commit();
						}
						else File.Create(Path.Combine(streamwriters.PathBase, "codebookpdf.notfound.txt"));
					}
					void _Interviews()
					{
						streamwriters.PathBase ??= directoryoutputpath;
						streamwriters.TryAdd("surveysav.records", "surveysav.records.txt", true);

						if (surveysav.Variables.Select(surveysavvariable =>
						{
							string? id = surveysav.LanguageCode is null ? null : surveysavvariable.GetId(surveysav.LanguageCode);
							Variable? variable = id is null ? null : variables.FirstOrDefault(_ => _.Id == id);

							if (id is null) streamwriters["surveysav.records"].WriteLine("{0}: id not found", surveysavvariable.Name);
							if (variable is null) streamwriters["surveysav.records"].WriteLine("{0}: variable not found", surveysavvariable.Name);

							return (surveysavvariable, variable);

						}).ToList() is List<(SurveySAVVariable, Variable)> surveysavvariables)
							foreach ((ISpsslyRawRecord spsslyrawrecord, int index) in surveysav.Records.Select((_, __) => (_, __ + 1)))
							{
								streamwriters["surveysav.records"].WriteLine();

								Console.WriteLine("{0}: {1}", index, survey.InterviewCount);

								Language? language = sqliteconnection_languages?.Table<Language>().FirstOrDefault(_ => _.Code == surveysav.LanguageCode);
								Interview interview = new()
								{
									PkSurvey = survey.Pk,
									PkLanguage = language?.Pk,
									Round = survey.Round,
								};

								foreach ((SurveySAVVariable, Variable?) pair in surveysavvariables)
									if (spsslyrawrecord.GetValue(pair.Item1)?.ToString()?.Replace(":", ";").Replace(",c", "|") is not string value)
										streamwriters["surveysav.records"].WriteLine("{0}: value not found", pair.Item1.Name);
									else if (pair.Item2 is not null)
										interview.List_PkVariable_Record = _TableGeneral.AddPKPair(interview.List_PkVariable_Record, pair.Item2.Pk, value);

								interviews.Add(interview);
							}

						streamwriters.Dispose(true, "surveysav.records");

						sqliteconnection
							.Update(survey, out int _)
							.InsertAll(interviews, out int _)
							.Commit();

						interviews = [.. sqliteconnection.Table<Interview>().TakeLast(interviews.Count)];

						sqliteconnectionround
							.Update(survey, out int _)
							.InsertAll(interviews, out int _)
							.Commit();
					}

					_Variables();
					_Questions();
					_Interviews();

					string sqliteconnectionindividualpath = Path.Combine(
						streamwriters.PathBase,
						string.Format("afrobarometer.{0}.{1}.db", inputcontainer.Round.AsString(), inputcontainer.CountryCode));

					_SQLiteConnection(sqliteconnectionindividualpath, true)
						.Insert(survey, out int _)
						.InsertAll(variables, out int _)
						.InsertAll(questions, out int _)
						.InsertAll(interviews, out int _)
						.CommitAndClose();

					FileInfo sqliteconnectionindividualfileinfo = new (sqliteconnectionindividualpath);
					string sqliteconnectionindividualzipname = string.Join('/', sqliteconnectionindividualfileinfo.ZipFile().Split('\\')[^2..^0]);
					string sqliteconnectionindividualgzipname = string.Join('/', sqliteconnectionindividualfileinfo.GZipFile().Split('\\')[^2..^0]);

					apifiles.Add(sqliteconnectionindividualzipname, inputcontainer.Round, inputcontainer.CountryCode);
					apifiles.Add(sqliteconnectionindividualgzipname, inputcontainer.Round, inputcontainer.CountryCode);
					
					surveysav.Dispose();

					File.Delete(sqliteconnectionindividualpath);
					File.Delete(surveytempfilepath);

					Console.WriteLine();
				}

				streamwritersround.TryAdd("table.variable.label", "table.variable.label.txt", true);
				streamwritersround.TryAdd("table.question.text", "table.question.text.txt", true);
				streamwritersround.TryAdd("table.question.variablelabel", "table.question.variablelabel.txt", true);

				List<Variable> variablesround = [.. sqliteconnectionround.Table<Variable>()];
				List<Question> questionsround = [.. sqliteconnectionround.Table<Question>()];

				foreach (Variable variable in variablesround.OrderBy(_ => _.Label))
					streamwritersround["table.variable.label"].WriteLine("{0} {1}\n{2} {3}\n", variable.Pk.ToString(variablesround.Count), variable.Id, variablesround.Count.ToEmptyCharacters(), variable.Label);

				foreach (Question question in questionsround.OrderBy(_ => _.Text))
					streamwritersround["table.question.text"].WriteLine("{0} [{1}]: {2}", question.Pk.ToString(questionsround.Count), question.Id, question.Text);

				foreach (Question question in questionsround.OrderBy(_ => _.VariableLabel))
					streamwritersround["table.question.variablelabel"].WriteLine("{0} [{1}]: {2}", question.Pk.ToString(questionsround.Count), question.Id, question.VariableLabel);

				streamwritersround.Dispose();
				sqliteconnectionround.Close();

				FileInfo sqliteconnectionroundfileinfo = new(sqliteconnectionroundpath);
				string sqliteconnectionroundzipname = string.Join('/', sqliteconnectionroundfileinfo.ZipFile().Split('\\')[^2..^0]);
				string sqliteconnectionroundgzipname = string.Join('/', sqliteconnectionroundfileinfo.GZipFile().Split('\\')[^2..^0]);

				apifiles.Add(sqliteconnectionroundzipname, surveysziparchivecontainer.Round, null);
				apifiles.Add(sqliteconnectionroundgzipname, surveysziparchivecontainer.Round, null);

				Console.WriteLine();
			}

			sqliteconnection.CommitAndClose();

			FileInfo sqlconnectionfileinfo = new(sqlconnectionpath);
			string sqliteconnectionzipname = sqlconnectionfileinfo.ZipFile().Split('\\').Last();
			string sqliteconnectiongzipname = sqlconnectionfileinfo.GZipFile().Split('\\').Last();

			apifiles.Add(sqliteconnectionzipname, null, null);
			apifiles.Add(sqliteconnectiongzipname, null, null);

			string apifilesjson = apifiles.ToString();
			string apifilespath = Path.Combine(DirectoryOutput, "index.json");

			using FileStream apifilesfilestream = File.OpenWrite(apifilespath);
			using StreamWriter apifilesstreamwriter = new (apifilesfilestream);

			apifilesstreamwriter.Write(apifilesjson);
			apifilesstreamwriter.Close();
			apifilesfilestream.Close();

			File.Delete(sqlconnectionpath);

			_CleaningPost();
		}

		static void _CleaningPre()
		{
			Console.WriteLine("Pre Cleaning...");

			if (Directory.Exists(DirectoryTemp)) Directory.Delete(DirectoryTemp, true);
			if (Directory.Exists(DirectoryOutput)) Directory.Delete(DirectoryOutput, true);

			Console.WriteLine("Creating Directories...");

			Directory.CreateDirectory(DirectoryTemp);
			Directory.CreateDirectory(DirectoryOutput);
		}
		static void _CleaningPost()
		{
			Console.WriteLine("Cleaning Up...");

			Directory.Delete(DirectoryTemp, true);
		}
		static SQLiteConnection _SQLiteConnection(string path, bool individual)
		{
			SQLiteConnection sqliteconnection = new(path);

			sqliteconnection.CreateTable<Interview>();
			sqliteconnection.CreateTable<Question>();
			sqliteconnection.CreateTable<Survey>();
			sqliteconnection.CreateTable<Variable>();

			return sqliteconnection;
		}
	}

	public static class Extensions
	{
		public static void Add(this JArray jarray, string filename, Rounds? round, string? country)
		{
			string description = filename.Split('.').Last() switch
			{
				"zip" => "zipped ",
				"gz" => "g-zipped ",

				_ => string.Empty
			};

			jarray.Add(new JObject
			{
				{ "DateCreated", DateTime.Now.ToString("dd-MM-yyyy") },
				{ "DateEdited", DateTime.Now.ToString("dd-MM-yyyy") },
				{ "Name", filename.Split('/').Last() },
				{ "Url", string.Format("https://raw.githubusercontent.com/xyclone-designs/database.afrobarometer/refs/heads/main/.output/{0}", filename) },
				{ "Description", true switch
					{
						true when round.HasValue && country is not null
							=> string.Format("individual {0}database for {1} from round {2}", description, country, round.Value.AsString()),
						true when round.HasValue => string.Format("individual {0}database for round {1}", description, round.Value.AsString()),
						true when country is not null => string.Format("individual {0}database for {1}", description, country),

						_ => string.Format("{0}database", description)
					}
				}
			});
		}
	}
}