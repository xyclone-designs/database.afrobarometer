using XycloneDesigns.Apis.Afrobarometer.Tables;

namespace System.IO
{
	public static class StreamWriterExtensions 
	{
		public static void Log(this StreamWriter streamwriter, Interview interview) { }
		public static void Log(this StreamWriter streamwriter, Question question) { }
		public static void Log(this StreamWriter streamwriter, Survey survey) { }
		public static void Log(this StreamWriter streamwriter, Variable variable) { }

		public static void LogError(this StreamWriter streamwriter, Interview interview) { }
		public static void LogError(this StreamWriter streamwriter, Question question) { }
		public static void LogError(this StreamWriter streamwriter, Survey survey) { }
		public static void LogError(this StreamWriter streamwriter, Variable variable) { }
	}
}
