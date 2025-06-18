using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

using XycloneDesigns.Apis.General.Tables;

namespace Database.Afrobarometer
{
	public static partial class Utils
	{
		public static partial class Inputs
		{
			public static partial class CodebookPDF
			{
				public static class Replacements
				{
					public static string _General(string input, string language)
					{
						input = _Base.Replacements._General(input, language);

						foreach (string[] _General in language switch
						{
							Language.Codes.French => French.General,
							Language.Codes.Portuguese => Portuguese.General,
							Language.Codes.English or _ => English.General,

						}) input = input.Replace(_General[0], _General[1]);

						return input;
					}
					public static string _GeneralRegex(string input, string language)
					{
						input = _Base.Replacements._GeneralRegex(input, language);

						foreach (string[] _GeneralRegex in language switch
						{
							Language.Codes.French => French.GeneralRegex,
							Language.Codes.Portuguese => Portuguese.GeneralRegex,
							Language.Codes.English or _ => English.GeneralRegex,

						}) input = Regex.Replace(input, _GeneralRegex[0], _GeneralRegex[1]);

						return input;
					}
					public static string _GeneralRegexExt(string input, string language, PdfDocument pdfdocument)
					{
						foreach (string[] _GeneralRegex in language switch
						{
							Language.Codes.French => French.GeneralRegexExt(pdfdocument),
							Language.Codes.Portuguese => Portuguese.GeneralRegexExt(pdfdocument),
							Language.Codes.English or _ => English.GeneralRegexExt(pdfdocument),

						}) input = Regex.Replace(input, _GeneralRegex[0], _GeneralRegex[1]);

						return input;
					}

					public static class French
					{
						public static IEnumerable<string[]> General => _Base.Replacements.French.General;
						public static IEnumerable<string[]> GeneralRegex => _Base.Replacements.French.GeneralRegex;
						public static IEnumerable<string[]> GeneralRegexExt(PdfDocument pdfdocument)
						{
							return GeneralRegex;
						}

					}
					public static class English
					{
						public static IEnumerable<string[]> General => _Base.Replacements.English.General
							.Append(["\n", string.Empty])
							.Append(["\r", string.Empty])
							.Append(["”", string.Empty])
							.Append(["“", string.Empty])
							.Append(["Copyright Afrobarometer", string.Empty])
							.Append(["/ ", "/"])
							.Append(["/", " or "])
							.Append(["Value Label1=Strongly agree", "Value Label: 1=Strongly agree"])
							.Append(["Value Label: =Strongly agree,", "Value Label: 1=Strongly agree,"])
							.Append(["Pidgin=English", "Pidgin-English"])
							.Append(["Béri=béri", "Béri-béri"])
							.Append(["Mixed=-=English/Afrikaans", "Mixed(English/Afrikaans)"])
							.Append(["Mixed=-=English or Afrikaans", "Mixed(English/Afrikaans)"])
							.Append(["Mixed=race", "Mixed-race"])
							.Append(["A=1, B=2, C=3", "1=A, 2=B, 3=C"])
							.Append(["102 QuestionIn", "102 Question: In"])
							.Append(["Statemen2 4", "Statement 2, 4"])
							.Append(["9= Turkey,=Ghana", "9= Turkey, 10=Ghana"])
							.Append(["518 =Sonrhai, =Makua", "518 =Sonrhai, 540=Makua"])
							.Append(["non=permanent", "non-permanent"])
							.Append(["518 =Sonrhai=Makua", "518 =Sonrhai, 540=Makua"])
							.Append(["9= Turkey=Ghana", "9= Turkey, 10 =Ghana"])
							.Append(["North=Sotho", "North-Sotho"])
							.Append(["South=Sotho", "South-Sotho"])
							.Append(["Eat Asian", "East Asian"])
							.Append(["[Response to Q78A-LES=0 (no) or 9 (Don’t Know)]", "[Response to Q78A-LES is 0 (no) or 9 (Don’t Know)]"])
							.Append(["[Response to Q78A-LES =0 (no) or 9 (Don’t Know)]", "[Response to Q78A-LES is 0 (no) or 9 (Don’t Know)]"])
							.Append(["7=Not applicable (Answer to Q83A-LES=1 No or9=Don’t know/have not heard enough)", "7=Not applicable (Answer to Q83A-LES is 1 or 9)"])
							.Append(["(Answer to Q83A-LES=1", "Answer to Q83A-LES is 1"])
							.Append(["9=Don’t know/have not heard enough),", "is 9 Don’t know/have not heard enough),"])
							.Append(["Ga=182", "182=Ga,"])
							.Append(["420-=TIM", "420=TIM"])
							.Append(["823=Patriotic Front=PF", "823=Patriotic Front/PF"])
							.Append(["Other=95", "95=Other"])
							.Append(["865-=Manyika", "865=Manyika"])
							.Append(["=98=Refused", "98=Refused"])
							.Append(["=98=Refused", "Pidgin/English,"])
							.Append(["743==Mngoni", "743=Mngoni"])
							.Append(["7-=Lack of projects", "7=Lack of projects"])
							.Append(["Sesotho, Sotho, S. Sotho ", "Sesotho/Sotho/S-Sotho, "])
							.Append(["1=very happy, 2= ++, 3= +, 4= =, 5= -, 6= --,7=not at all happy", "1=very happy, 2=more happy, 3=+happy, 4=happy, 5=-happy, 6=mildly happy, 7=not at all happy"])
							.Append(["87=Not applicable, =Refused,", "7=Not applicable, 8=Refused,"])
							.Append(["7=Not applicable (Answer to Q83A-LES=1 No or 9=Don’t know/have not heard enough)", "7=Not applicable (Answer to Q83A-LES is 1 or 9)"])
							.Append(["2= Do only if they choose3, =Always so", "2= Do only if they choose, 3=Always so"])
							.Append(["9=National independence/people’s self-determination=9,", "9=National independence/people’s self-determination,"])
							.Append(["6=Eat Asian (Chinese, Korean, Indonesian, etc.), Other=95,", "6=East Asian (Chinese, Korean, Indonesian, etc.), 95=Other,"]);
						public static IEnumerable<string[]> GeneralRegex => _Base.Replacements.English.GeneralRegex
							//.Append(["Copyright Afrobarometer\\s*[0-9]*", string.Empty])
							.Append([",\\s*Post\\s*Code=Other\\s*[A-Za-z0-9]*\\s*\\[[Ss]pecify\\]", string.Empty]);
						public static IEnumerable<string[]> GeneralRegexExt(PdfDocument pdfdocument)
						{
							return Enumerable.Range(1, pdfdocument.NumberOfPages)
								.Select(_ => new string[]
								{
									string.Format("Copyright Afrobarometer\\s*{0}{1}", _, "{1}"), string.Empty

								}).Concat(GeneralRegex);
						}
					}
					public static class Portuguese
					{
						public static IEnumerable<string[]> General => _Base.Replacements.Portuguese.General;
						public static IEnumerable<string[]> GeneralRegex => _Base.Replacements.Portuguese.GeneralRegex;
						public static IEnumerable<string[]> GeneralRegexExt(PdfDocument pdfdocument)
						{
							return GeneralRegex;
						}
					}
				}

				public static readonly string[] ProcessCodebooksInputEnglishSplit_Id = ["Question Number:",];
				public static readonly string[] ProcessCodebooksInputEnglishSplit_Text = ["Question:",];
				public static readonly string[] ProcessCodebooksInputEnglishSplit_VariableLabel = ["Variable Label:", "Variable label:",];
				public static readonly string[] ProcessCodebooksInputEnglishSplit_Values = ["Values:", "Value:",];
				public static readonly string[] ProcessCodebooksInputEnglishSplit_ValueLabels = ["Value Label:", "Value Labels:", "Values Labels:",];
				public static readonly string[] ProcessCodebooksInputEnglishSplit_Source = ["Source:", "DataSource:",];
				public static readonly string[] ProcessCodebooksInputEnglishSplit_Note = ["Note:", "Notes:",];
				public static readonly string[] ProcessCodebooksInputEnglishSplit =
				[
					.. ProcessCodebooksInputEnglishSplit_Id,
					.. ProcessCodebooksInputEnglishSplit_Text,
					.. ProcessCodebooksInputEnglishSplit_VariableLabel,
					.. ProcessCodebooksInputEnglishSplit_Values,
					.. ProcessCodebooksInputEnglishSplit_ValueLabels,
					.. ProcessCodebooksInputEnglishSplit_Source,
					.. ProcessCodebooksInputEnglishSplit_Note,
				];

				public static string[] SplitText(PdfDocument pdfdocument, string language, out string rawtext)
				{
					StringBuilder stringbuilder = new();

					for (int pagenumber = 1; pagenumber <= pdfdocument.NumberOfPages; pagenumber++)
					{
						Page page = pdfdocument.GetPage(pagenumber);

						if (string.IsNullOrWhiteSpace(page.Text) is false)
						{
							string pattern = string.Format("Copyright Afrobarometer\\s*{0}{1}", pagenumber, "{1}");
							string result = Regex.Replace(page.Text, pattern, string.Empty);

							stringbuilder.Append(result);
						}
					}

					string text = rawtext = stringbuilder.ToString();

					text = Replacements._General(text, language);
					text = Replacements._GeneralRegexExt(text, language, pdfdocument);
					text = Replacements._GeneralRegex(text, language);

					string[] split = text.SplitWithoutRemoval(ProcessCodebooksInputEnglishSplit_Id);

					return split.Length > 2 ? split[1..^0] : split;
				}
			}
		}
	}
}
