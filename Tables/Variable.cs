using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using SpsslyVariable = Spssly.SpssDataset.Variable;

namespace Database.Afrobarometer.Tables
{
	[SQLite.Table("variables")]
    public class Variable : _AfrobarometerModel
    {
		public Variable() { }
		public Variable(SpsslyVariable variable) 
		{
			Name = variable.Name;
			Label = variable.Label;
			ValueLabelsDictionary = variable.ValueLabels;
		}

		private readonly char[] _valuelabelsdictionaryrefreshsplit = [ ';', '=' ];

		private string? _valuelabels;
		private IDictionary<double, string>? _valuelabelsdictionary;

		private string? _valuelabelsrefresh()
		{ 
			return _valuelabelsdictionary is null
				? null
				: string.Join(",", _valuelabelsdictionary.Select(_ => string.Format("{0};{1}", _.Key, _.Value).Replace(',', '|')));
		}
		private IDictionary<double, string> _valuelabelsdictionaryrefresh()
		{
			return _valuelabels?
				.SplitRemoveTrim(',')
				.Select(_ =>
				{
					string[] _split = _.SplitTrim(_valuelabelsdictionaryrefreshsplit, 2);
					
					_split[0] = _split[0].Replace('|', ',');
					_split[1] = _split[1].Replace('|', ',');

					return _split;
				
				}).ToDictionary(_ => double.Parse(_[0]), _ => _[1]) ?? [];
		}

		[SQLite.Column(nameof(Id))] public string? Id { get; set; }
		[SQLite.Column(nameof(Name))] public string? Name { get; set; }
		[SQLite.Column(nameof(Label))] public string? Label { get; set; }
		[SQLite.Column(nameof(ValueLabels))] public string? ValueLabels
		{
			get => _valuelabels ??= _valuelabelsrefresh();
			set
			{
				_valuelabels = value;
				_valuelabelsdictionary = _valuelabelsdictionaryrefresh();
			}
		}

		[SQLite.Ignore] public IDictionary<double, string> ValueLabelsDictionary
		{
			get => _valuelabelsdictionary ??= _valuelabelsdictionaryrefresh();
			set
			{
				_valuelabelsdictionary = value;
				_valuelabels = _valuelabelsrefresh();
			}
		}

		[SQLite.Table("variables")]
		public class Individual : Variable
		{
			public Individual(Variable variable)
			{
				Name = variable.Name;
				Label = variable.Label;
				ValueLabels = variable.ValueLabels;
			}

			[SQLite.AutoIncrement]
			[SQLite.NotNull]
			[SQLite.PrimaryKey]
			[SQLite.Unique]
			public new int Pk { get; set; }
		}
	}

	public static partial class StreamWriterExtensions
	{
		public static void Log(this StreamWriter streamwriter, Variable variable)
		{
			streamwriter.Log(variable as _AfrobarometerModel);

			streamwriter.WriteLine("Pk: {0}", variable.Pk);
			streamwriter.WriteLine("Id: {0}", variable.Id);
			streamwriter.WriteLine("Name: {0}", variable.Name);
			streamwriter.WriteLine("Label: {0}", variable.Label);
			streamwriter.WriteLine("ValueLabels: {0}", variable.ValueLabels);
			streamwriter.WriteLine();
		}
		public static void LogError(this StreamWriter streamwriter, Variable variable)
		{
			streamwriter.LogError(variable as _AfrobarometerModel);
		}
	}
}