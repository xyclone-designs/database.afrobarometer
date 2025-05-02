using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace Database.Afrobarometer
{
	public static partial class Utils
	{
		public static partial class Inputs
		{
			public static class Codebook
			{
				[StringSyntax("Regex")]
				public static readonly string Removals_Regex_Copyright = "Copyright Afrobarometer\\s*[0-9]*";
				[StringSyntax("Regex")]
				public static readonly string Removals_Regex_PostCode = ",\\s*Post\\s*Code=Other\\s*[A-Za-z0-9]*\\s*\\[[Ss]pecify\\]";

				public static readonly string[][] ProcessCodebooksInputEnglishReplacements = new string[][]
				{
					["\n", string.Empty],
					["\r", string.Empty],
					[ "/ ", "/" ],
					[ "/", " or " ],
					[ "Pidgin=English", "Pidgin-English" ],
					[ "Béri=béri", "Béri-béri" ],
					[ "Mixed=-=English/Afrikaans", "Mixed(English/Afrikaans)" ],
					[ "Mixed=-=English or Afrikaans", "Mixed(English/Afrikaans)" ],
					[ "Mixed=race", "Mixed-race" ],
					["A=1, B=2, C=3", "1=A, 2=B, 3=C"],
					["102 QuestionIn", "102 Question: In"],
					["Statemen2 4", "Statement 2, 4"],
					["9= Turkey,=Ghana", "9= Turkey, 10=Ghana"],
					["518 =Sonrhai, =Makua", "518 =Sonrhai, 540=Makua"],
					["non=permanent", "non-permanent"],
					["518 =Sonrhai=Makua", "518 =Sonrhai, 540=Makua"],
					["9= Turkey=Ghana", "9= Turkey, 10 =Ghana"],
					["North=Sotho", "North-Sotho"],
					["South=Sotho", "South-Sotho"],
					["Eat Asian", "East Asian"],
					["[Response to Q78A-LES=0 (no) or 9 (Don’t Know)]", "[Response to Q78A-LES is 0 (no) or 9 (Don’t Know)]"],
					["[Response to Q78A-LES =0 (no) or 9 (Don’t Know)]", "[Response to Q78A-LES is 0 (no) or 9 (Don’t Know)]"],
					["7=Not applicable (Answer to Q83A-LES=1 No or9=Don’t know/have not heard enough)", "7=Not applicable (Answer to Q83A-LES is 1 or 9)"],
					["(Answer to Q83A-LES=1", "Answer to Q83A-LES is 1"],
					["9=Don’t know/have not heard enough),", "is 9 Don’t know/have not heard enough),"],
					["Ga=182", "182=Ga,"],
					["420-=TIM", "420=TIM"],
					["823=Patriotic Front=PF", "823=Patriotic Front/PF"],
					["Other=95", "95=Other"],
					["865-=Manyika", "865=Manyika"],
					["=98=Refused", "98=Refused"],
					["=98=Refused", "Pidgin/English,"],
					["743==Mngoni", "743=Mngoni"],
					["7-=Lack of projects", "7=Lack of projects"],
					["Sesotho, Sotho, S. Sotho ", "Sesotho/Sotho/S-Sotho, "],
					["1=very happy, 2= ++, 3= +, 4= =, 5= -, 6= --,7=not at all happy", "1=very happy, 2=more happy, 3=+happy, 4=happy, 5=-happy, 6=mildly happy, 7=not at all happy"],
					["87=Not applicable, =Refused,", "7=Not applicable, 8=Refused,"],
					["7=Not applicable (Answer to Q83A-LES=1 No or 9=Don’t know/have not heard enough)", "7=Not applicable (Answer to Q83A-LES is 1 or 9)"],
					["2= Do only if they choose3, =Always so", "2= Do only if they choose, 3=Always so"],
					["9=National independence/people’s self-determination=9,", "9=National independence/people’s self-determination,"],
					["6=Eat Asian (Chinese, Korean, Indonesian, etc.), Other=95,", "6=East Asian (Chinese, Korean, Indonesian, etc.), 95=Other,"],
				};
				public static readonly string[] ProcessCodebooksInputEnglishSplit = new string[]
				{
					"Question Number:",
					"Question:",
					"Variable Label:", "Variable label:",
					"Values:",
					"Value Label:", "Value Labels:", "Values Labels:",
					"Source:", "DataSource:",
					"Note:", "Notes:"
				};

				public static string[] SplitText(PdfDocument pdfdocument, out string rawtext)
				{
					bool copyrightnumbered = false;
					StringBuilder stringbuilder = new();

					foreach (Page page in pdfdocument.GetPages())
					{
						stringbuilder.Append(page.Text);

						if (page.Number == 1)
							copyrightnumbered = Regex.IsMatch(page.Text, "Copyright Afrobarometer\\s*1{1}", RegexOptions.Singleline);
						if (page.Number == 2 && copyrightnumbered is false)
							copyrightnumbered = Regex.IsMatch(page.Text, "Copyright Afrobarometer\\s*2{1}", RegexOptions.Singleline);
						if (page.Number == 3 && copyrightnumbered is false)
							copyrightnumbered = Regex.IsMatch(page.Text, "Copyright Afrobarometer\\s*3{1}", RegexOptions.Singleline);
						if (page.Number == 4 && copyrightnumbered is false)
							copyrightnumbered = Regex.IsMatch(page.Text, "Copyright Afrobarometer\\s*4{1}", RegexOptions.Singleline);
						if (page.Number == 5 && copyrightnumbered is false)
							copyrightnumbered = Regex.IsMatch(page.Text, "Copyright Afrobarometer\\s*5{1}", RegexOptions.Singleline);
					}

					string text = rawtext = stringbuilder.ToString();

					if (copyrightnumbered is false)
						text = Regex.Replace(text, "Copyright Afrobarometer", string.Empty);
					else for (int pagenumber = 1; pagenumber < pdfdocument.NumberOfPages; pagenumber++)
							text = Regex.Replace(text, string.Format("Copyright Afrobarometer\\s*{0}{1}", pagenumber, "{1}"), string.Empty, RegexOptions.Singleline);

					text = Regex.Replace(text, Removals_Regex_Copyright, string.Empty);
					text = Regex.Replace(text, Removals_Regex_PostCode, string.Empty);

					foreach (string[] _ in ReplacementsEnglish)
						text = text.Replace(_[0], _[1]);

					foreach (string[] _ in ProcessCodebooksInputEnglishReplacements)
						text = text.Replace(_[0], _[1]);

					string[] split = text.Split(ProcessCodebooksInputEnglishSplit[0], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

					return split.Length > 2 ? split[1..^1] : split;
				}
			}
		}
	}
}
