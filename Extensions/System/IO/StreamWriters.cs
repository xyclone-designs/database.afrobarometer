using System.Collections.Generic;

namespace System.IO
{
	public class StreamWriters : Dictionary<string, StreamWriter>, IDisposable
	{
		public const string Key_Log = "log";
		public const string Key_Error = "error";
		public const string Key_Operations = "operations";

		Dictionary<string, FileStream> FileStreams { get; set; } = [];

		public bool TryAdd(string key, string path)
		{
			if (ContainsKey(key))
				return false;

			FileStreams.Add(key, File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite));
			
			Add(key, new StreamWriter(FileStreams[key]));

			return true;
		}

		public void PerformAction(Action<StreamWriter> act, params string[] keys)
		{
			foreach (string key in keys)
				if (TryGetValue(key, out StreamWriter? value) && value is not null)
					act.Invoke(value);
		}
		public void Add(string key, string path)
		{
			if (TryAdd(key, path) is false)
				throw new ArgumentException(key);
		}
		public void Dispose(string key, bool remove = true)
		{
			this[key].Dispose();
			FileStreams[key].Dispose();

			if (remove)
			{
				Remove(key);
				FileStreams.Remove(key);
			}
		}
		public void Dispose()
		{
			foreach (string key in Keys)
			{
				this[key].Dispose();
				this.Remove(key);
			}

			foreach (string key in FileStreams.Keys)
			{
				FileStreams[key].Dispose();
				FileStreams.Remove(key);
			}
		}
	}
}
