using System;
using System.Collections.Generic;
using System.Linq;

using SpsslyVariable = Spssly.SpssDataset.Variable;

namespace XycloneDesigns.Apis.Afrobarometer.Tables
{
    public static class VariableExtensions
	{
		private static readonly char[] _valuelabelsdictionaryrefreshsplit = [';', '='];

		public static string ToValueLabels(this IDictionary<double, string> valuelabels)
		{
			return string.Join(",", valuelabels.Select(_ =>
			{
				return string.Format("{0};{1}", _.Key, _.Value).Replace(',', '|');
			}));
		}
		public static Dictionary<double, string> GetValueLabelsDictionary(this Variable variable)
		{
			if (variable.ValueLabels is null)
				return [];

			string[]? valuelabels = variable.ValueLabels.Trim(':').SplitRemoveTrim(',');

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
		public static Variable FromSpsslyVariable(this Variable variable, SpsslyVariable spsslyvariable)
		{
			variable.Label = spsslyvariable.Label;
			variable.ValueLabels = spsslyvariable.ValueLabels?.ToValueLabels();

			return variable;
		}
	}
}