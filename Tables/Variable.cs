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

		private string? _valuelabels;
		private IDictionary<double, string>? _valuelabelsdictionary;

		[SQLite.Column(nameof(Id))] public string? Id { get; set; }
		[SQLite.Column(nameof(Name))] public string? Name { get; set; }
		[SQLite.Column(nameof(Label))] public string? Label { get; set; }
		[SQLite.Column(nameof(ValueLabels))] public string? ValueLabels
		{
			get => _valuelabels ??= _valuelabelsdictionary is null 
				? null 
				: string.Join(",", _valuelabelsdictionary.Select(_ => string.Format("{0};{1}", _.Key, _.Value).Replace(',', '|')));
			set
			{
				_valuelabels = value;
				_valuelabelsdictionary = _valuelabels?
					.Split(',')
					.Select(_ => _.Split(';'))
					.ToDictionary(_ => double.Parse(_[0]), _ => _[1].Replace('|', ','));
			}
		}

		[SQLite.Ignore] public IDictionary<double, string> ValueLabelsDictionary
		{
			get => _valuelabelsdictionary ??= _valuelabels?
				.Split(',')
				.Select(_ => _.Split(';'))
				.ToDictionary(_ => double.Parse(_[0]), _ => _[1].Replace('|', ',')) ?? new Dictionary<double, string>();
			set
			{
				_valuelabelsdictionary = value;
				_valuelabels = _valuelabelsdictionary is null 
					? null 
					: string.Join(",", _valuelabelsdictionary.Select(_ => string.Format("{0};{1}", _.Key, _.Value).Replace(',', '|')));
			}
		}

		public override void Log(StreamWriter streamwriter)
		{
			base.Log(streamwriter);

			streamwriter.WriteLine("Id: {0}", Id);
			streamwriter.WriteLine("Name: {0}", Name);
			streamwriter.WriteLine("Label: {0}", Label);
			streamwriter.WriteLine("ValueLabels: {0}", ValueLabels);
			streamwriter.WriteLine();
		}
	}
}