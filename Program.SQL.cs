using Database.Afrobarometer.Enums;
using Database.Afrobarometer.Inputs;
using Database.Afrobarometer.Tables;
using Database.Afrobarometer.Tables.Individual;

using SQLite;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

using SpsslyVariable = Spssly.SpssDataset.Variable;
using ISpsslyRawRecord = Spssly.SpssDataset.IRawRecord;

namespace Database.Afrobarometer
{
	internal partial class Program
	{
		public static void SQL(Args args)
		{
			string alldatabase = Path.Combine(DirectoryOutputDatabase, "all.db");

			SQLiteConnection sqliteconnectionall = SQLiteConnection(alldatabase, true);

			foreach (IGrouping<(Rounds Round, string ZipPath), InputContainer> ziparchive in args.Inputs.Where(_ =>
			{
				return
					_.InputType == InputTypes.SurveySAV &&
					_.Round == _.Round &&
					_.Country == _.Country &&
					_.Language == _.Language;

			}).GroupBy(_ => (_.Round, _.ZipPath)))
			{
				using FileStream datastream = File.OpenRead(ziparchive.Key.ZipPath);
				using ZipArchive datazip = new(datastream);

				foreach (string tempfilepath in datazip.Entries.OrderMergeLast().ExtractToDirectoryIterable(DirectoryTemp))
				{
					string directory = Path.Combine(
						DirectoryOutputDatabase,
						string.Format("round-{0:00}", ziparchive.Key.Round.ToInt()),
						ziparchive.Key.ZipPath.Split('\\')[^1].Replace(".zip", ""));

					if (Directory.Exists(directory) is false) Directory.CreateDirectory(directory);

					string surveysavname = tempfilepath.Split('\\')[^1];
					string surveydatabase = Path.Combine(directory, string.Format("{0}.db", surveysavname));

					SurveySAV surveysav = new(tempfilepath);
					SQLiteConnection sqliteconnectionindividual = SQLiteConnection(surveydatabase, true);

					SQLFromSurveys(surveysav, directory, sqliteconnectionall, sqliteconnectionindividual);

					surveysav.Dispose();
					sqliteconnectionindividual.Close();

					File.Delete(tempfilepath);
					Console.WriteLine();
				}

				Console.WriteLine();
			}

			sqliteconnectionall.Close();
		}

		public static void SQLFromSurveys(SurveySAV surveysav, string logpath, params SQLiteConnection[] sqliteconnections)
		{
			Console.WriteLine(surveysav);

			SQLiteConnection? 
				sqliteconnectionall = sqliteconnections.ElementAtOrDefault(0),
				sqliteconnectionindividual = sqliteconnections.ElementAtOrDefault(1);

			if (sqliteconnectionall is null)
				return;

			string surveysavlogfilename = surveysav.Filename.Replace(".sav", ".log.txt");
			string surveysaverrorfilename = surveysav.Filename.Replace(".sav", ".error.txt");
			string surveysavoperationsfilename = surveysav.Filename.Replace(".sav", ".operations.txt");

			string surveysavlogfilepath = Path.Combine(logpath, surveysavlogfilename);
			string surveysaverrorfilepath = Path.Combine(logpath, surveysaverrorfilename);
			string surveysavoperationsfilepath = Path.Combine(logpath, surveysavoperationsfilename);

			using FileStream surveysavlogfilestream = File.Create(surveysavlogfilepath);
			using FileStream surveysaverrorfilestream = File.Create(surveysaverrorfilepath);
			using FileStream surveysavoperationsfilestream = File.Create(surveysavoperationsfilepath);

			using StreamWriter surveysavlogstreamwriter = new(surveysavlogfilestream);
			using StreamWriter surveysaverrorstreamwriter = new(surveysaverrorfilestream);
			using StreamWriter surveysavoperationsstreamwriter = new(surveysavoperationsfilestream);

			List<Variable?> variables = [];
			
			foreach (SpsslyVariable spsslyvariable in surveysav.Variables)
			{
				Console.WriteLine("Variable Name: {0}", spsslyvariable.Name);

				if (string.IsNullOrWhiteSpace(spsslyvariable.Label))
				{
					variables.Add(null);
					continue;
				}

				Variable variable = Utils.Inputs.Variable.CleanNew(spsslyvariable, surveysavoperationsstreamwriter);

				if (variable.Label is not null)
					foreach (Variable _variable in sqliteconnectionall.Table<Variable>().Where(_ => _.Label == variable.Label))
					{
						if (variable.ValueLabels?.EqualsOrdinalIgnoreCase(_variable.ValueLabels) ?? false)
						{
							variable = _variable;
							break;
						}
						else if (variable.ValueLabelsDictionary.All(_ => _variable.ValueLabelsDictionary.TryGetValue(_.Key, out string? _value) && _.Value.EqualsOrdinalIgnoreCase(_value)))
						{
							variable.ValueLabelsDictionary = _variable.ValueLabelsDictionary;
							break;
						}
						else if (_variable.ValueLabelsDictionary.All(_ => variable.ValueLabelsDictionary.TryGetValue(_.Key, out string? _value) && _.Value.EqualsOrdinalIgnoreCase(_value)))
						{
							_variable.ValueLabelsDictionary = variable.ValueLabelsDictionary;
							variable = _variable;
							break;
						}
						else if (variable.ValueLabelsDictionary.Keys
							.Where(_ => _variable.ValueLabelsDictionary.ContainsKey(_))
							.All(_ => _variable.ValueLabelsDictionary[_].EqualsOrdinalIgnoreCase(variable.ValueLabelsDictionary[_])))
						{
							foreach (double variablekey in variable.ValueLabelsDictionary.Keys)
								_variable.ValueLabelsDictionary.TryAdd(variablekey, variable.ValueLabelsDictionary[variablekey]);

							variable.ValueLabelsDictionary = _variable.ValueLabelsDictionary;
							break;
						}
					}

				if (variable.Pk != 0)
					sqliteconnectionall.Update(variable);
				else
				{
					sqliteconnectionall.Insert(variable);
					variables.Add(variable = sqliteconnectionall.Table<Variable>().Last());
				}
				
				if (sqliteconnectionindividual is not null)
				{
					VariableIndividual variableindividual = new(variable);
					sqliteconnectionindividual.Insert(variableindividual);
				}

				variable.Log(surveysavlogstreamwriter);
				variable.LogError(surveysaverrorstreamwriter);
			}

			StringBuilder stringbuilder = new ();

			foreach (ISpsslyRawRecord spsslyrawrecord in surveysav.Records)
			{
				for (int index = 0; variables.ElementAtOrDefault(index) is not null && spsslyrawrecord.Data.ElementAtOrDefault(index)?.ToString() is string value; index++)
				{
					switch (true)
					{
						case true when int.TryParse(value, out int valueint):
							break;

						default: break;
					}
				}
			}

			Console.WriteLine();
		}
	}
}
