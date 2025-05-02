using Database.Afrobarometer.Tables;

using SQLite;

using System.Linq;

namespace Database.Afrobarometer
{
	public static partial class Utils
	{
		public static partial class Tables
		{
			public static class Variable { }
		}

		public static Variable InesrtIfNew(this SQLiteConnection sqliteconnection, Variable variable)
		{
			variable.Id ??= variable.Label is null ? null : Tables.ToId(variable.Label);

			if (variable.Id is not null)
				foreach (Variable _variable in sqliteconnection.Table<Variable>())
					if (_variable.Id is not null)
					{
						bool distance = Likeness.Variable.Distance(_variable.Id, variable.Id, out double _);
						bool similarity = distance || Likeness.Variable.Similarity(_variable.Id, variable.Id, out double _);

						if (distance || similarity)
							return _variable;
					}

			sqliteconnection.Insert(variable);

			return sqliteconnection.Table<Variable>().Last();
		}
	}
}
