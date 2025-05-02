
namespace Database.Afrobarometer
{
	public static partial class Utils
	{
		public static partial class Inputs
		{
			public static readonly string[][] ReplacementsEnglish = new string[][]
			{
				[ "&", "and" ],
				[ "vs.", "vs" ],
				[ "don't", "do not" ],
				[ "Don't", "Do not" ],
				[ "canellation", "cacnellation" ],
				[ "demonstration", "demostartion" ],
				[ "dififculty", "difficulty" ],
				[ "health care", "healthcare" ],
				[ "govt", "government" ],
				[ "govt.", "government" ],
				[ "government.", "government" ],
				[ "groups", "group" ],
				[ "group's", "group" ],
				[ "programme", "programme" ],
				[ "regards to 1999", "regards to the 1999" ],
				[ "responsibilty", "responsibility" ],
				[ "resonsibility", "responsibility" ],
				[ "storey", "story" ],
			};
			public static readonly string[] ReplacementsEnglish_Country = new string[]
			{
				"[country]",
				"botswana",
				"ghana",
				"lesotho",
				"mali",
				"malawi",
				"namibia",
				"nigeria",
				"south africa",
				"tanzania",
				"uganda",
				"zambia",
				"zimbabwe",
			};
			public static readonly string[] ReplacementsEnglish_Nationality = new string[]
			{
				"[nationality]",
				"basotho",
				"batswana",
				"ghanaian",
				"ghanaians",
				"ghanians",
				"ghanian",
				"malians",
				"malian",
				"malawians",
				"malawian",
				"mosotho",
				"namibians",
				"namibian",
				"nigerians",
				"nigerian",
				"south african",
				"south africans",
				"south african people",
				"tanzanians",
				"tanzanian",
				"ugandans",
				"ugandan",
				"zambians",
				"zambian",
				"zimabaweans",
				"zimbabweans",
				"zimbabwean",
			};
		}
	}
}
