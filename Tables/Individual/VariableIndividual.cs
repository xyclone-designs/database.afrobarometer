
namespace Database.Afrobarometer.Tables.Individual
{
	[SQLite.Table("variables")]
    public class VariableIndividual : Variable
	{
		public VariableIndividual(Variable variable)
		{
			Name = variable.Name;
			Label = variable.Label;
			ValueLabels = variable.ValueLabels;
		}

		[SQLite.Column(nameof(Pk))] public new int Pk { get; set; }
	}
}