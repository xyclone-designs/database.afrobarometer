using Database.Afrobarometer.Enums;

namespace Database.Afrobarometer.Tables
{
	[SQLite.Table("interviews")]
    public class Interview : _AfrobarometerModel
    {
		[SQLite.Column(nameof(Language))] public Languages? Language { get; set; }
		[SQLite.Column(nameof(Round))] public Rounds? Round { get; set; }
		[SQLite.Column(nameof(Pk_Survey))] public int Pk_Survey { get; set; }
		[SQLite.Column(nameof(List_PkVariables))] public string? List_PkVariables { get; set; }
		[SQLite.Column(nameof(List_PkQuestions_Record))] public string? List_PkQuestions_Record { get; set; }
	}
}