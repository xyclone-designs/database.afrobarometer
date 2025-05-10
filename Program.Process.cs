using Database.Afrobarometer.Enums;
using Database.Afrobarometer.Inputs;
using Database.Afrobarometer.Tables;

using SQLite;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using SurveySAVVariable = Spssly.SpssDataset.Variable;

namespace Database.Afrobarometer
{
	internal partial class Program
	{
		public class ProcessArgs
		{
			public ProcessArgs(SQLiteConnection sqliteconnection, Languages language, StreamWriters streamwriters)
			{
				Sqliteconnection = sqliteconnection;
				Language = language;
				Streamwriters = streamwriters;
			}

			public Action<Question>? OnQuestion { get; set; }
			public Action<Variable>? OnVariable { get; set; }

			public Languages Language { get; set; }
			public StreamWriters Streamwriters { get; set; }
			public SQLiteConnection Sqliteconnection { get; set; }
		}

		public static List<Variable> ProcessSurveySAV(SurveySAV surveysav, ProcessArgs args)
		{
			ProcessArgs processargs = new(args.Sqliteconnection, args.Language, args.Streamwriters);
			List<Variable> variables = [];

			foreach (SurveySAVVariable surveysavvariable in surveysav.Variables.DistinctBy(_ =>
			{
				return _.GetId(args.Language);

			})) if (ProcessSurveySAVVariable(surveysavvariable, processargs) is Variable variable)
					variables.Add(variable);

			return variables;
		}
		public static List<Question> ProcessCodebookPDF(CodebookPDF codebookpdf, ProcessArgs args, out Dictionary<string, Variable> variables)
		{
			List<Question> questions = []; variables = [];

			foreach (CodebookPDF.Question codebookpdfquestion in codebookpdf.Questions)
				if (ProcessCodebookPDFQuestion(codebookpdfquestion, args, out Variable? variable) is Question question)
				{
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
				}

			foreach (Question _question in questions)
				args.Streamwriters["_questions"].Log(_question);

			return questions;
		}

		public static Variable? ProcessSurveySAVVariable(SurveySAVVariable surveysavvariable, ProcessArgs args) 
		{
			Console.WriteLine("Variable Label: {0}", surveysavvariable?.Label);

			if (surveysavvariable is null || string.IsNullOrWhiteSpace(surveysavvariable.Label))
			{
				args.Streamwriters[StreamWriters.Key_Log].WriteLine("Variable Label: {0} 'No label found'", surveysavvariable?.Label);
				args.Streamwriters[StreamWriters.Key_Error].WriteLine("Variable Label: {0} 'No label found'", surveysavvariable?.Label);

				return null;
			}

			Variable variable = Utils.Inputs.SurveySAV.Variable
				.ToTableVariable(surveysavvariable, args.Language, args.Streamwriters[StreamWriters.Key_Operations]);

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

			if (variable.Pk == 0 && args.Sqliteconnection.Table<Variable>().AsEnumerable().FirstOrDefault(_ =>
			{
				if (string.IsNullOrWhiteSpace(variable.Label) is false && string.IsNullOrWhiteSpace(_.Label) is false)
					if (Utils.Likeness.Variable.Distance(variable.Label, _.Label, out double _) ||
						Utils.Likeness.Variable.Similarity(variable.Label, _.Label, out double _))
						return true;

				return false;

			}) is Variable __variable) variable = __variable;

			args.Streamwriters[StreamWriters.Key_Log].Log(variable);
			args.Streamwriters[StreamWriters.Key_Error].LogError(variable);

			return variable;
		}
		public static Question? ProcessCodebookPDFQuestion(CodebookPDF.Question codebookpdfquestion, ProcessArgs processargs, out Variable? variable)
		{
			variable = null;

			if (codebookpdfquestion is null || string.IsNullOrWhiteSpace(codebookpdfquestion.VariableLabel))
			{
				Console.WriteLine("CodebookPDF: 'No variable label found'");

				processargs.Streamwriters[StreamWriters.Key_Log].WriteLine("CodebookPDF: 'No variable label found'");
				processargs.Streamwriters[StreamWriters.Key_Error].WriteLine("CodebookPDF: 'No variable label found'");

				return null;
			}

			Question question = Utils.Inputs.CodebookPDF.Question
				.ToTablesQuestion(codebookpdfquestion, processargs.Language, processargs.Streamwriters[StreamWriters.Key_Operations], out Variable _variable);

			variable = processargs.Sqliteconnection
				.Table<Variable>()
				.FirstOrDefault(_ => question.VariableLabel == _.Label) ?? _variable;

			Console.WriteLine("Question ID: {0}", question.Id);

			if (string.IsNullOrWhiteSpace(question.Text))
				return question;

			Question? _question = processargs.Sqliteconnection
				.Table<Question>()
				.FirstOrDefault(_ => _.VariableLabel == question.VariableLabel || _.Text == question.Text);

			return _question ?? processargs.Sqliteconnection
				.Table<Question>()
				.AsEnumerable()
				.FirstOrDefault(_ =>
				{
					if (string.IsNullOrWhiteSpace(question.VariableLabel) is false && string.IsNullOrWhiteSpace(_.VariableLabel) is false)
						if (Utils.Likeness.Variable.Distance(question.VariableLabel, _.VariableLabel, out double _) ||
							Utils.Likeness.Variable.Similarity(question.VariableLabel, _.VariableLabel, out double _))
							return true;

					if (string.IsNullOrWhiteSpace(question.Text) is false && string.IsNullOrWhiteSpace(_.Text) is false)
						if (Utils.Likeness.Question.Distance(question.Text, _.Text) ||
							Utils.Likeness.Question.Similarity(question.Text, _.Text))
							return true;

					return false;
				
				}) ?? question;
		}
	}
}