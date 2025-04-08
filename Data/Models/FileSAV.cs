using Database.Afrobarometer.Data.Enums;

using Spss;

using System;
using System.IO;

namespace Database.Afrobarometer.Data.Models
{
	public class FileSAV : SpssReader
	{
		public FileSAV(Stream fileStream) : base(fileStream) { }
		public FileSAV(Stream fileStream, string filename) : this(fileStream) 
		{
			Country = default(Countries).FromFilename(filename) ?? throw new ArgumentException();
			Round = default(Rounds).FromFilename(filename) ?? throw new ArgumentException();
		}

		public Countries Country { get; set; }
		public Rounds Round { get; set; }
	}
}
