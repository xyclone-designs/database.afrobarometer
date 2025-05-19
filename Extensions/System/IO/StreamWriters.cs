using System.Collections.Generic;
using System.Linq;
using static Database.Afrobarometer.Program;

namespace System.IO
{
	public class StreamWriters : Dictionary<string, StreamWriter>, IDisposable
	{
		Dictionary<string, FileStream> FileStreams { get; set; } = [];

		public string? PathBase { get; set; }

		public bool TryAdd(string key, string path, bool relative = false)
		{
			if (ContainsKey(key))
				return false;

			if (relative)
				path = Path.Combine(PathBase ?? throw new ArgumentException("If relative = true, 'StreamWriters.PathBase' must be set"), path);

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
		public void Add(string key, string path, bool relative = false)
		{
			if (relative)
				path = Path.Combine(PathBase ?? throw new ArgumentException("If relative = true, 'StreamWriters.PathBase' must be set"), path);

			if (TryAdd(key, path) is false)
				throw new ArgumentException(key);
		}
		public void Dispose(bool remove = true, params string[] keys)
		{
			foreach (string key in keys)
			{
				this[key].Close();
				FileStreams[key].Close();

				this[key].Dispose();
				FileStreams[key].Dispose();

				if (remove)
				{
					Remove(key);
					FileStreams.Remove(key);
				}
			}
		}
		public void Dispose()
		{
			foreach (string key in Keys)
			{
				this[key].Close();
				this[key].Dispose();
				this.Remove(key);
			}

			foreach (string key in FileStreams.Keys)
			{
				FileStreams[key].Close();
				FileStreams[key].Dispose();
				FileStreams.Remove(key);
			}
		}
	}

	public static class StreamWritersExtensions
	{
		public static StreamWriter[] Get(this StreamWriters[] streamwriters, string key)
		{
			return streamwriters
				.Select(_ => _[key])
				.ToArray();
		}
		public static void TryAdd(this StreamWriters[] streamwriters, params string[][] keys)
		{
			foreach (StreamWriters streamwriter in streamwriters)
				foreach (string[] key in keys)
					streamwriter.TryAdd(key[0], key[1], true);
		}
		public static void For(this StreamWriters[] streamwriters, string key, Action<StreamWriter> act)
		{
			foreach (StreamWriters streamwriter in streamwriters)
				if (streamwriter.TryGetValue(key, out StreamWriter? _streamwriter) && _streamwriter is not null)
					act.Invoke(_streamwriter);
		}
		public static void ForEach(this StreamWriters[] streamwriters, Action<StreamWriter> act, params string[] keys)
		{
			foreach (StreamWriters streamwriter in streamwriters)
				foreach (string key in keys)
					if (streamwriter.TryGetValue(key, out StreamWriter? _streamwriter) && _streamwriter is not null)
						act.Invoke(_streamwriter);
		}
		public static void Dispose(this StreamWriters[] streamwriters, bool remove, params string[] keys)
		{
			foreach (StreamWriters streamwriter in streamwriters)
				foreach (string key in keys)
					streamwriter.Dispose(remove, key);
		}
	}
}
