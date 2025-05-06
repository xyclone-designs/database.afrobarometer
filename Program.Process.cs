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
		public static Variable? ProcessSurveySAVVariable(SurveySAVVariable surveysavvariable, SQLiteConnection sqliteconnection, Languages languages, StreamWriters streamwriters) 
		{
			Console.WriteLine("Variable Label: {0}", surveysavvariable.Label);

			if (string.IsNullOrWhiteSpace(surveysavvariable.Label))
			{
				streamwriters[StreamWriters.Key_Log].WriteLine("Variable Label: {0} 'No label found'", surveysavvariable.Label);
				streamwriters[StreamWriters.Key_Error].WriteLine("Variable Label: {0} 'No label found'", surveysavvariable.Label);

				return null;
			}

			Variable variable = Utils.Inputs.SurveySAV.Variable
				.ToTableVariable(surveysavvariable, languages, streamwriters[StreamWriters.Key_Operations]);

			TableQuery<Variable> variables = sqliteconnection
				.Table<Variable>()
				.Where(_ => variable.Label == _.Label);

			foreach (Variable _variable in variables)
				switch (true)
				{
					case true when variable.ValueLabels?.EqualsOrdinalIgnoreCase(_variable.ValueLabels) ?? false: break;
					
					case true when variable.ValueLabelsDictionary.All(_ =>
					{
						return _variable.ValueLabelsDictionary.TryGetValue(_.Key, out string? _value) && _.Value.EqualsOrdinalIgnoreCase(_value);
					
					}): variable.ValueLabelsDictionary = _variable.ValueLabelsDictionary; break;

					case true when _variable.ValueLabelsDictionary.All(_ =>
					{
						return variable.ValueLabelsDictionary.TryGetValue(_.Key, out string? _value) && _.Value.EqualsOrdinalIgnoreCase(_value);
					}):
						_variable.ValueLabelsDictionary = variable.ValueLabelsDictionary;
						sqliteconnection.Update(_variable);
						variable = _variable;
						break;

					case true when variable.ValueLabelsDictionary.Keys
						.Where(_ => _variable.ValueLabelsDictionary.ContainsKey(_))
						.All(_ => _variable.ValueLabelsDictionary[_].EqualsOrdinalIgnoreCase(variable.ValueLabelsDictionary[_])):
						foreach (double variablekey in variable.ValueLabelsDictionary.Keys)
							_variable.ValueLabelsDictionary.TryAdd(variablekey, variable.ValueLabelsDictionary[variablekey]);

						variable.ValueLabelsDictionary = _variable.ValueLabelsDictionary;
						sqliteconnection.Update(_variable);
						variable = _variable;
						break;

					default: break;
				}

			streamwriters[StreamWriters.Key_Log].Log(variable);
			streamwriters[StreamWriters.Key_Error].LogError(variable);

			return variable;
		}
		public static Question? ProcessCodebookPDFQuestion(CodebookPDF.Question codebookpdfquestion, SQLiteConnection sqliteconnection, Languages languages, StreamWriters streamwriters)
		{
			if (string.IsNullOrWhiteSpace(codebookpdfquestion.Id))
			{
				Console.WriteLine("CodebookPDF.QuestionID 'No id found'", codebookpdfquestion.Id);

				streamwriters[StreamWriters.Key_Log].WriteLine("CodebookPDF.QuestionID 'No id found'", codebookpdfquestion.Id);
				streamwriters[StreamWriters.Key_Error].WriteLine("CodebookPDF.QuestionID 'No id found'", codebookpdfquestion.Id);

				return null;
			}

			Question question = Utils.Inputs.CodebookPDF.Question
				.ToTablesQuestion(codebookpdfquestion, languages, streamwriters[StreamWriters.Key_Operations], out Variable variable);

			Console.WriteLine("Question ID: {0}", question.Id);

			if (string.IsNullOrWhiteSpace(question.Text))
				return question;

			IDictionary<int, string?> questions = sqliteconnection
				.Table<Question>()
				.ToDictionary(_ => _.Pk, _ => _.Text);

			int pk = questions.FirstOrDefault(_ => question.Text == _.Value).Key;
			pk = pk is not 0 ? pk : questions.FirstOrDefault(_ =>
			{
				if (string.IsNullOrWhiteSpace(_.Value))
					return false;

				return Utils.Likeness.Question.Distance(question.Text, _.Value) || Utils.Likeness.Question.Similarity(question.Text, _.Value);

			}).Key;

			return pk is 0 ? question : sqliteconnection.Get<Question>(pk);
		}
	}
}