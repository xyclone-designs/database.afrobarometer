using System.IO;
using System.Linq;

namespace Database.Afrobarometer.Tables
{
    public class _AfrobarometerModel
    {
		[SQLite.AutoIncrement]
		[SQLite.NotNull]
		[SQLite.PrimaryKey]
        [SQLite.Unique]
        public int Pk { get; set; }

		public static string AddPKPair(string? pkPairs, int pk, string value)
		{
			if (string.IsNullOrWhiteSpace(pkPairs))
				return string.Format("{0}:{1}", pk, value);

			pkPairs += string.Format(",{0}:{1}", pk, value);
			
			return pkPairs;
		}
		public static string AddPKIfUnique(string? pks, int? pk)
		{
			if (pk is null)
				return string.Empty;
			if (pks is null)
				pks = pk.Value.ToString();
			else if (pks.Split(",").Contains(pk.ToString()) is false)
				pks += string.Format(",{0}", pk);

			return pks;
		}
		public static string AddPKPairIfUnique(string? pkPairs, int pk, string value)
		{
			if (pkPairs is null)
				pkPairs = string.Format("{0}:{1}", pk, value);
			else if (pkPairs.Split(",").FirstOrDefault(_pkPair => long.Parse(_pkPair.Split(":")[0]) == pk) is not string pkPair)
				pkPairs += string.Format(",{0}:{1}", pk, value);
			
			return pkPairs;
		}
		public static string AddPKPairIfUnique(string? pkPairs, int pk, int value, bool addtoifpresent = false)
		{
			if (pkPairs is null)
				pkPairs = string.Format("{0}:{1}", pk, value);
			else if (pkPairs.Split(",").FirstOrDefault(_pkPair => long.Parse(_pkPair.Split(":")[0]) == pk) is not string pkPair)
				pkPairs += string.Format(",{0}:{1}", pk, value);
			else if (addtoifpresent)
				pkPairs = pkPairs.Replace(pkPair, string.Format("{0}:{1}", pk, long.Parse(pkPair.Split(":")[1]) + value));

			return pkPairs;
		}
	}

	public static partial class StreamWriterExtensions
	{
		public static void Log(this StreamWriter streamwriter, _AfrobarometerModel _afrobarometermodel)
		{
			streamwriter.WriteLine("Pk: {0}", _afrobarometermodel.Pk);
		}
		public static void LogError(this StreamWriter streamwriter, _AfrobarometerModel _afrobarometermodel)
		{ }
	}
}