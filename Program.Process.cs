using Database.Afrobarometer.Enums;
using Database.Afrobarometer.Inputs;

using SQLite;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

using XycloneDesigns.Apis.Afrobarometer.Enums;
using XycloneDesigns.Apis.Afrobarometer.Tables;
using XycloneDesigns.Apis.General.Tables;
using SurveySAVVariable = Spssly.SpssDataset.Variable; 

namespace Database.Afrobarometer
{
	internal partial class Program
	{
		public class ProcessArgs
		{
			public ProcessArgs(SQLiteConnection sqliteconnection, string languagecode, StreamWriters streamwriters)
			{
				Sqliteconnection = sqliteconnection;
				LanguageCode = languagecode;
				Streamwriters = streamwriters;
			}

			public Action<Question>? OnQuestion { get; set; }
			public Action<Variable>? OnVariable { get; set; }

			public Rounds Round { get; set; }
			public string? CountryCode { get; set; }
			public string LanguageCode { get; set; }
			public List<Question>? Questions { get; set; }
			public List<Variable>? Variables { get; set; }
			public StreamWriters Streamwriters { get; set; }
			public SQLiteConnection Sqliteconnection { get; set; }
			public SQLiteConnection? SqliteconnectionCountries { get; set; }
			public SQLiteConnection? SqliteconnectionLanguages { get; set; }
			public SQLiteConnection? SqliteconnectionMunicipalities { get; set; }
			public SQLiteConnection? SqliteconnectionProvinces { get; set; }
		}

		public static string 
			Key_SurveySAV_Error = "surveysav.error", 
			Key_SurveySAV_Log = "surveysav.log", 
			Key_SurveySAV_Operations = "surveysav.operations",
			Key_SurveySAV_Labels = "surveysav.labels";
		
		public static string 
			Key_CodebookPDF_Error = "codebookpdf.error", 
			Key_CodebookPDF_Log = "codebookpdf.log", 
			Key_CodebookPDF_Operations = "codebookpdf.operations", 
			Key_CodebookPDF_Texts = "codebookpdf.texts", 
			Key_CodebookPDF_VariableLabels = "codebookpdf.variablelabels";

		public static List<Variable> ProcessSurveySAV(SurveySAV surveysav, ProcessArgs args)
		{
			List<Variable> variables = [];

			args.Streamwriters.TryAdd(Key_SurveySAV_Log, string.Format("{0}.txt", Key_SurveySAV_Log), true);
			args.Streamwriters.TryAdd(Key_SurveySAV_Error, string.Format("{0}.txt", Key_SurveySAV_Error), true);
			args.Streamwriters.TryAdd(Key_SurveySAV_Operations, string.Format("{0}.txt", Key_SurveySAV_Operations), true);

			foreach (SurveySAVVariable surveysavvariable in surveysav.Variables)
				if (ProcessSurveySAVVariable(surveysavvariable, args) is Variable variable)
				{
					args.Variables ??= [];
					args.Variables.Add(variable);

					if (variables.Index(_ => _.Id == variable.Id) is int index)
						variables[index] = variable;
					else variables.Add(variable);

					args.Streamwriters.PerformAction(_ => _.Log(variable), Key_SurveySAV_Log);
					args.Streamwriters.PerformAction(_ => _.LogError(variable), Key_SurveySAV_Error);
				}

			foreach (Variable _variable in variables.OrderBy(_ => _.Label))
				args.Streamwriters.PerformAction(_ => _.WriteLine("{0}: {1}", _variable.Id, _variable.Label ?? "\"null\""), Key_SurveySAV_Labels);

			args.Variables?.Clear();
			args.Streamwriters.Dispose(true, Key_SurveySAV_Log, Key_SurveySAV_Error, Key_SurveySAV_Operations);

			return variables;
		}
		public static List<Question> ProcessCodebookPDF(CodebookPDF codebookpdf, ProcessArgs args, out Dictionary<string, Variable> variables)
		{
			List<Question> questions = []; variables = [];

			args.Streamwriters.Add(Key_CodebookPDF_Error, string.Format("{0}.txt", Key_CodebookPDF_Error), true);
			args.Streamwriters.Add(Key_CodebookPDF_Log, string.Format("{0}.txt", Key_CodebookPDF_Log), true);
			args.Streamwriters.Add(Key_CodebookPDF_Operations, string.Format("{0}.txt", Key_CodebookPDF_Operations), true);
			args.Streamwriters.Add(Key_CodebookPDF_Texts, string.Format("{0}.txt", Key_CodebookPDF_Texts), true);
			args.Streamwriters.Add(Key_CodebookPDF_VariableLabels, string.Format("{0}.txt", Key_CodebookPDF_VariableLabels), true);

			foreach (CodebookPDF.Question codebookpdfquestion in codebookpdf.Questions)
			{
				if (ProcessCodebookPDFQuestion(codebookpdfquestion, args, out Variable? variable) is Question question)
				{
					args.Questions ??= [];
					args.Questions.Add(question);

					if (variable is not null)
					{
						if (args.SqliteconnectionLanguages?
							.Table<Language>()
							.FirstOrDefault(_ => _.Code == args.LanguageCode) is Language language)
							question.PkLanguage = language.Pk;

						if (question.Text is not null)
							variables.TryAdd(question.Text, variable);

						if (variable.Pk != 0)
							question.PkVariable ??= variable.Pk;
					}

					args.OnQuestion?.Invoke(question);

					if (questions.Index(_ => _.Id == question.Id) is int index)
						questions[index] = question;
					else questions.Add(question);

					args.Streamwriters.PerformAction(_ => _.Log(question), Key_CodebookPDF_Log);
					args.Streamwriters.PerformAction(_ => _.LogError(question), Key_CodebookPDF_Error);
				}
			}

			foreach (Question _question in questions.OrderBy(_ => _.Text))
				args.Streamwriters.PerformAction(_ => _.WriteLine("{0}: {1}", _question.Id, _question.Text ?? "\"null\""), Key_CodebookPDF_Texts);

			foreach (Question _question in questions.OrderBy(_ => _.VariableLabel))
				args.Streamwriters.PerformAction(_ => _.WriteLine("{0}: {1}", _question.Id, _question.VariableLabel ?? "\"null\""), Key_CodebookPDF_VariableLabels);

			args.Questions?.Clear();
			args.Streamwriters.Dispose(true, Key_CodebookPDF_Error, Key_CodebookPDF_Log, Key_CodebookPDF_Operations, Key_CodebookPDF_Texts, Key_CodebookPDF_VariableLabels);

			return questions;
		}

		public static Variable? ProcessSurveySAVVariable(SurveySAVVariable surveysavvariable, ProcessArgs args) 
		{
			Variable variable = Utils.Inputs.SurveySAV.Variable.ToTableVariable(
				language: args.LanguageCode,
				spsslyvariable: surveysavvariable, 
				loggers: args.Streamwriters[Key_SurveySAV_Operations]);
			Dictionary<double, string> valuelablesditionary = variable.GetValueLabelsDictionary();

			Console.WriteLine("SurveySAVVariable.Label: {0}", surveysavvariable.Label ?? "null");

			TableQuery<Variable> variables = args.Sqliteconnection
				.Table<Variable>()
				.Where(_ => variable.Id == _.Id);

			foreach (Variable _variable in variables)
			{
				Dictionary<double, string> _valuelablesditionary = variable.GetValueLabelsDictionary();

				switch (true)
				{
					case true when variable.ValueLabels?.EqualsOrdinalIgnoreCase(_variable.ValueLabels) ?? false:
						variable = _variable;
						break;
					
					case true when valuelablesditionary.All(_ =>
					{
						return _valuelablesditionary.TryGetValue(_.Key, out string? _value) && _.Value.EqualsOrdinalIgnoreCase(_value);
					}): 
						variable.ValueLabels = _valuelablesditionary.ToValueLabels();
						variable = _variable; 
						break;

					case true when _valuelablesditionary.All(_ =>
					{
						return valuelablesditionary.TryGetValue(_.Key, out string? _value) && _.Value.EqualsOrdinalIgnoreCase(_value);
					}):
						_variable.ValueLabels = valuelablesditionary.ToValueLabels();
						args.Sqliteconnection.Update(_variable);
						variable = _variable;
						break;

					case true when valuelablesditionary.Keys
						.Where(_ => _valuelablesditionary.ContainsKey(_))
						.All(_ => _valuelablesditionary[_].EqualsOrdinalIgnoreCase(valuelablesditionary[_])):
						foreach (double variablekey in valuelablesditionary.Keys)
							_valuelablesditionary.TryAdd(variablekey, valuelablesditionary[variablekey]);

						variable.ValueLabels = _valuelablesditionary.ToValueLabels();
						args.Sqliteconnection.Update(_variable);
						variable = _variable;
						break;

					default: break;
				}
			}

			if (variable.Pk is not 0)
				return variable;

			Expression<Func<Variable, bool>> equalityexpression = _ => _.Label == variable.Label;
			bool equalityfunc(Variable _)
			{
				return _.Label == variable.Label;
			}
			bool similarityanddistance(Variable _)
			{
				if (string.IsNullOrWhiteSpace(variable.Label) is false && string.IsNullOrWhiteSpace(_.Label) is false)
					if (Utils.Likeness.Variable.Distance(variable.Label, _.Label, out double _) ||
						Utils.Likeness.Variable.Similarity(variable.Label, _.Label, out double _))
						return true;

				return false;
			};

			variable =
				args.Variables?.FirstOrDefault(_ => equalityfunc(_) || similarityanddistance(_)) ??
				args.Sqliteconnection
					.Table<Variable>()
					.FirstOrDefault(equalityexpression) ??
				args.Sqliteconnection
					.Table<Variable>()
					.AsEnumerable()
					.FirstOrDefault(similarityanddistance) ??
				variable;

			return variable;
		}
		public static Question? ProcessCodebookPDFQuestion(CodebookPDF.Question codebookpdfquestion, ProcessArgs args, out Variable? variable)
		{
			Console.WriteLine("CodebookPDF.Question.Id: {0}", codebookpdfquestion.Id); variable = null;

			Question question = Utils.Inputs.CodebookPDF.Question.ToTablesQuestion(
				language: args.LanguageCode,
				question: codebookpdfquestion,
				tablesvariable: out Variable _variable,
				loggers: args.Streamwriters[Key_CodebookPDF_Operations]);

			bool nulltexts = string.IsNullOrWhiteSpace(question.Text), nullvariblelabels = string.IsNullOrWhiteSpace(question.VariableLabel);
			
			variable = args.Sqliteconnection
				.Table<Variable>()
				.FirstOrDefault(_ => _.Id == _variable.Id);
 
			Func<Question, bool> equalityfunc;			
			Expression<Func<Question, bool>> equalityexpresion;			
			Func<Question, bool> similarityanddistance;

			switch ((nulltexts, nullvariblelabels))
			{
				case (false, false):
					equalityfunc = _ => _.VariableLabel == question.VariableLabel || _.Text == question.Text;
					equalityexpresion = _ => _.VariableLabel == question.VariableLabel || _.Text == question.Text;
					similarityanddistance = _ =>
					{
						if (string.IsNullOrWhiteSpace(_.VariableLabel) is false)
							if (Utils.Likeness.Variable.Distance(question.VariableLabel!, _.VariableLabel, out double _) ||
								Utils.Likeness.Variable.Similarity(question.VariableLabel!, _.VariableLabel, out double _))
								return true;

						if (string.IsNullOrWhiteSpace(_.Text) is false)
							if (Utils.Likeness.Question.Distance(question.Text!, _.Text) ||
								Utils.Likeness.Question.Similarity(question.Text!, _.Text))
								return true;

						return false;
					};
					break;
				
				case (false, true):
					equalityfunc = _ => _.Text == question.Text;
					equalityexpresion = _ => _.Text == question.Text;
					similarityanddistance = _ =>
					{
						if (string.IsNullOrWhiteSpace(_.Text) is false)
							if (Utils.Likeness.Question.Distance(question.Text!, _.Text) ||
								Utils.Likeness.Question.Similarity(question.Text!, _.Text))
								return true;

						return false;
					};
					break;

				case (true, false):
					equalityfunc = _ => _.VariableLabel == question.VariableLabel;
					equalityexpresion = _ => _.VariableLabel == question.VariableLabel;
					similarityanddistance = _ =>
					{
						if (string.IsNullOrWhiteSpace(_.VariableLabel) is false)
							if (Utils.Likeness.Variable.Distance(question.VariableLabel!, _.VariableLabel, out double _) ||
								Utils.Likeness.Variable.Similarity(question.VariableLabel!, _.VariableLabel, out double _))
								return true;

						return false;
					};
					break;

				case (true, true): return question;
			}

			question = 
				args.Questions?.FirstOrDefault(_ => equalityfunc.Invoke(_) || similarityanddistance.Invoke(_)) ?? 
				args.Sqliteconnection
					.Table<Question>()
					.FirstOrDefault(equalityexpresion) ?? 
				args.Sqliteconnection
					.Table<Question>()
					.AsEnumerable()
					.FirstOrDefault(similarityanddistance) ??
				question;

			return question;
		}
	}
}