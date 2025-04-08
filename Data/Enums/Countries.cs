
namespace Database.Afrobarometer.Data.Enums
{
	public enum Countries
	{
		_Undefined,
		_All,

		Benin,
		Botswana,
		CapeVerde,
		Ghana,
		Kenya,
		Lesotho,
		Madagascar,
		Mali,
		Malawi,
		Mozambique,
		Namibia,
		Niger,
		Nigeria,
		Senegal,
		SouthAfrica,
		Tanzania,
		Uganda,
		Zambia,
		Zimbabwe,
	}

	public static class CountriesExtensions
	{
		public static Countries? FromFilename(this Countries country, string filename)
		{
			string code = filename.Split('-', '_')[0];

			return code switch
			{
				"ben" => Countries.Benin,
				"bot" => Countries.Botswana,
				"cve" => Countries.CapeVerde,
				"gha" => Countries.Ghana,
				"ken" => Countries.Kenya,
				"les" => Countries.Lesotho,
				"mad" => Countries.Madagascar,
				"mli" or "mali" => Countries.Mali,
				"mlw" => Countries.Malawi,
				"moz" => Countries.Mozambique,
				"nam" => Countries.Namibia,
				"nig" => Countries.Nigeria,
				"saf" => Countries.SouthAfrica,
				"sen" => Countries.Senegal,
				"tan" => Countries.Tanzania,
				"uga" => Countries.Uganda,
				"zam" => Countries.Zambia,
				"zim" => Countries.Zimbabwe,

				_ => null
			};
		}
	}
}
