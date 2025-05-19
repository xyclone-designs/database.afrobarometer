using Database.Afrobarometer.Enums;
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
			Label = variable.Label;
			ValueLabelsDictionary = variable.ValueLabels;
		}

		private readonly char[] _valuelabelsdictionaryrefreshsplit = [';', '='];

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
			if (_valuelabels is null)
				return new Dictionary<double, string> { };

			string[]? valuelabels = _valuelabels.Trim(':').SplitRemoveTrim(',');

			for (int index = 0; index < valuelabels.Length; index++)
			{
				string[] split = valuelabels[index].SplitRemoveTrim('=', '"');

				if (index > 0 && int.TryParse(split.ElementAtOrDefault(0), out int _) is false)
				{
					valuelabels[index - 1] = string.Format("{0}, {1}", valuelabels[index - 1], valuelabels[index]);
					valuelabels = valuelabels
						.Where((_, __) => index != __)
						.ToArray();
					index--;
				}
			}

			IEnumerable<string[]>? _split = valuelabels
				.Select(_ => _.SplitTrim(_valuelabelsdictionaryrefreshsplit, 2))
				.Where(_ => _.Length == 2)
				.Select(_ =>
				{
					_[0] = _[0].Replace('|', ',');
					_[1] = _[1].Replace('|', ',');

					return _;

				}).DistinctBy(_ => _[0]);
			
			return _split.ToDictionary(_ => double.Parse(_[0]), _ => _[1]) ?? [];
		}

		[SQLite.Column(nameof(Id))] public string? Id { get; set; }
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

		[SQLite.Ignore] public Languages Language { get; set; }
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
				Id = variable.Id;
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

			streamwriter.WriteLine("Id: {0}", variable.Id);
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