using Database.Afrobarometer.Enums;
using Database.Afrobarometer.Inputs;
using Database.Afrobarometer.Tables;

using SQLite;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

using SurveySAVVariable = Spssly.SpssDataset.Variable;

namespace Database.Afrobarometer
{
	internal partial class Program
	{
		public class ProcessArgs
		{
			public ProcessArgs(SQLiteConnection sqliteconnection, Languages language, params StreamWriters[] streamwriters)
			{
				Sqliteconnection = sqliteconnection;
				Language = language;
				Streamwriters = streamwriters;
			}

			public Action<Question>? OnQuestion { get; set; }
			public Action<Variable>? OnVariable { get; set; }

			public Languages Round { get; set; }
			public Languages Country { get; set; }
			public Languages Language { get; set; }
			public List<Question>? Questions { get; set; }
			public List<Variable>? Variables { get; set; }
			public StreamWriters[] Streamwriters { get; set; }
			public SQLiteConnection Sqliteconnection { get; set; }
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

			args.Streamwriters.TryAdd(
				[Key_SurveySAV_Log, string.Format("{0}.txt", Key_SurveySAV_Log)],
				[Key_SurveySAV_Error, string.Format("{0}.txt", Key_SurveySAV_Error)],
				[Key_SurveySAV_Operations, string.Format("{0}.txt", Key_SurveySAV_Operations)]);

			foreach (SurveySAVVariable surveysavvariable in surveysav.Variables)
				if (ProcessSurveySAVVariable(surveysavvariable, args) is Variable variable)
				{
					args.Variables ??= [];
					args.Variables.Add(variable);

					if (variables.Index(_ => _.Id == variable.Id) is int index)
						variables[index] = variable;
					else variables.Add(variable);

					args.Streamwriters.For(Key_SurveySAV_Log, _ => _.Log(variable));
					args.Streamwriters.For(Key_SurveySAV_Error, _ => _.LogError(variable));
				}

			foreach (Variable _variable in variables.OrderBy(_ => _.Label))
				args.Streamwriters.For(Key_SurveySAV_Labels, _ => _.WriteLine("{0}: {1}", _variable.Id, _variable.Label ?? "\"null\""));

			args.Variables?.Clear();
			args.Streamwriters.Dispose(true, Key_SurveySAV_Log, Key_SurveySAV_Error, Key_SurveySAV_Operations);

			return variables;
		}
		public static List<Question> ProcessCodebookPDF(CodebookPDF codebookpdf, ProcessArgs args, out Dictionary<string, Variable> variables)
		{
			List<Question> questions = []; variables = [];

			args.Streamwriters.TryAdd(
				[Key_CodebookPDF_Error, string.Format("{0}.txt", Key_CodebookPDF_Error)],
				[Key_CodebookPDF_Log, string.Format("{0}.txt", Key_CodebookPDF_Log)],
				[Key_CodebookPDF_Operations, string.Format("{0}.txt", Key_CodebookPDF_Operations)],
				[Key_CodebookPDF_Texts, string.Format("{0}.txt", Key_CodebookPDF_Texts)],
				[Key_CodebookPDF_VariableLabels, string.Format("{0}.txt", Key_CodebookPDF_VariableLabels)]);

			foreach (CodebookPDF.Question codebookpdfquestion in codebookpdf.Questions)
			{
				if (ProcessCodebookPDFQuestion(codebookpdfquestion, args, out Variable? variable) is Question question)
				{
					args.Questions ??= [];
					args.Questions.Add(question);

					if (variable is not null)
					{
						question._Language = args.Language;

						if (question.Text is not null)
							variables.TryAdd(question.Text, variable);

						if (variable.Pk != 0)
							question.PkVariable ??= variable.Pk;
					}

					args.OnQuestion?.Invoke(question);

					if (questions.Index(_ => _.Id == question.Id) is int index)
						questions[index] = question;
					else questions.Add(question);

					args.Streamwriters.For(Key_CodebookPDF_Log, _ => _.Log(question));
					args.Streamwriters.For(Key_CodebookPDF_Error, _ => _.LogError(question));
				}
			}

			foreach (Question _question in questions.OrderBy(_ => _.Text))
				args.Streamwriters.For(Key_CodebookPDF_Texts, _ => _.WriteLine("{0}: {1}", _question.Id, _question.Text ?? "\"null\""));

			foreach (Question _question in questions.OrderBy(_ => _.VariableLabel))
				args.Streamwriters.For(Key_CodebookPDF_VariableLabels, _ => _.WriteLine("{0}: {1}", _question.Id, _question.VariableLabel ?? "\"null\""));

			args.Questions?.Clear();
			args.Streamwriters.Dispose(true, Key_CodebookPDF_Error, Key_CodebookPDF_Log, Key_CodebookPDF_Operations, Key_CodebookPDF_Texts, Key_CodebookPDF_VariableLabels);

			return questions;
		}

		public static Variable? ProcessSurveySAVVariable(SurveySAVVariable surveysavvariable, ProcessArgs args) 
		{
			Variable variable = Utils.Inputs.SurveySAV.Variable.ToTableVariable(
				language: args.Language,
				spsslyvariable: surveysavvariable, 
				loggers: args.Streamwriters.Get(Key_SurveySAV_Operations));

			Console.WriteLine("SurveySAVVariable.Label: {0}", surveysavvariable.Label ?? "null");

			TableQuery<Variable> variables = args.Sqliteconnection
				.Table<Variable>()
				.Where(_ => variable.Id == _.Id);

			foreach (Variable _variable in variables)
				switch (true)
				{
					case true when variable.ValueLabels?.EqualsOrdinalIgnoreCase(_variable.ValueLabels) ?? false:
						variable = _variable;
						break;
					
					case true when variable.ValueLabelsDictionary.All(_ =>
					{
						return _variable.ValueLabelsDictionary.TryGetValue(_.Key, out string? _value) && _.Value.EqualsOrdinalIgnoreCase(_value);
					
					}): 
						variable.ValueLabelsDictionary = _variable.ValueLabelsDictionary;
						variable = _variable; 
						break;

					case true when _variable.ValueLabelsDictionary.All(_ =>
					{
						return variable.ValueLabelsDictionary.TryGetValue(_.Key, out string? _value) && _.Value.EqualsOrdinalIgnoreCase(_value);
					}):
						_variable.ValueLabelsDictionary = variable.ValueLabelsDictionary;
						args.Sqliteconnection.Update(_variable);
						variable = _variable;
						break;

					case true when variable.ValueLabelsDictionary.Keys
						.Where(_ => _variable.ValueLabelsDictionary.ContainsKey(_))
						.All(_ => _variable.ValueLabelsDictionary[_].EqualsOrdinalIgnoreCase(variable.ValueLabelsDictionary[_])):
						foreach (double variablekey in variable.ValueLabelsDictionary.Keys)
							_variable.ValueLabelsDictionary.TryAdd(variablekey, variable.ValueLabelsDictionary[variablekey]);

						variable.ValueLabelsDictionary = _variable.ValueLabelsDictionary;
						args.Sqliteconnection.Update(_variable);
						variable = _variable;
						break;

					default: break;
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
				language: args.Language,
				question: codebookpdfquestion,
				tablesvariable: out Variable _variable,
				loggers: args.Streamwriters.Get(Key_CodebookPDF_Operations));

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