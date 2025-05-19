using Database.Afrobarometer.Enums;
using Database.Afrobarometer.Tables;

using System;
using System.IO;

using Newtonsoft.Json.Linq;

using SystemPath = System.IO.Path;

namespace Database.Afrobarometer.Inputs
{
	public class CountryJSON : JObject
	{
		public static class KeysCountry
		{
			public const string Blurb = "blurb";
			public const string Languages = "languages";
			public const string Name = "name";
		}
		public static class KeysCountryBase
		{
			public const string Capital = "capital";
			public const string Population = "population";
			public const string SquareKMs = "squareKMs";
			public const string UrlFlag = "urlFlag";
			public const string UrlInsignia = "urlInsignia";
			public const string UrlPoster = "urlPoster";
			public const string UrlWebsite = "urlWebsite";
		}

		private static JObject JObjectFromFilepath(string filepath)
		{
			using FileStream filestream = File.OpenRead(filepath);
			using StreamReader streamreader = new(filestream);

			string json = streamreader.ReadToEnd();

			return Parse(json);
		}

		public CountryJSON(string filepath) : base(JObjectFromFilepath(filepath)) 
		{
			Filepath = filepath;
			Filename = Filepath.Split('\\')[^1];
		}

		public string Filepath { get; set; }
		public string Filename { get; set; }

		public CountryBase ToCountryBase()
		{
			CountryBase countrybase = new()
			{
				Code = Enum.Parse<Countries>(Filename.Split('.')[0], true).ToCode(),

				Capital = TryGetValue(KeysCountryBase.Capital, StringComparison.OrdinalIgnoreCase, out JToken? _capital) ? _capital.ToObject<string?>() : null,
				Population = TryGetValue(KeysCountryBase.Population, StringComparison.OrdinalIgnoreCase, out JToken? _population) ? _population.ToObject<int?>() : null,
				SquareKMs = TryGetValue(KeysCountryBase.SquareKMs, StringComparison.OrdinalIgnoreCase, out JToken? _squarekms) ? _squarekms.ToObject<decimal?>() : null,
				UrlFlag = TryGetValue(KeysCountryBase.UrlFlag, StringComparison.OrdinalIgnoreCase, out JToken? _urlflag) ? _urlflag.ToObject<string?>() : null,
				UrlInsignia = TryGetValue(KeysCountryBase.UrlInsignia, StringComparison.OrdinalIgnoreCase, out JToken? _urlinsignia) ? _urlinsignia.ToObject<string?>() : null,
				UrlPoster = TryGetValue(KeysCountryBase.UrlPoster, StringComparison.OrdinalIgnoreCase, out JToken? _urlposter) ? _urlposter.ToObject<string?>() : null,
				UrlWebsite = TryGetValue(KeysCountryBase.UrlWebsite, StringComparison.OrdinalIgnoreCase, out JToken? _urlwebsite) ? _urlwebsite.ToObject<string?>() : null,
			};

			return countrybase;
		}
		public Country ToCountry(Languages languages) 
		{
			string filepath = SystemPath.Combine(Directory.GetParent(Filepath)?.FullName ?? string.Join('\\', Filepath.Split('\\')[0..^2]), languages.ToCode(), Filename);

			CountryJSON countrylanguage = new(filepath);
			CountryBase countrybase = ToCountryBase();
			Country country = new(countrybase)
			{
				Blurb = countrylanguage.TryGetValue(KeysCountry.Blurb, StringComparison.OrdinalIgnoreCase, out JToken? _blurb) ? _blurb.ToObject<string?>() : null,
				Languages = countrylanguage.TryGetValue(KeysCountry.Languages, StringComparison.OrdinalIgnoreCase, out JToken? _languages) ? _languages.ToObject<string?>() : null,
				Name = countrylanguage.TryGetValue(KeysCountry.Name, StringComparison.OrdinalIgnoreCase, out JToken? _name) ? _name.ToObject<string?>() : null,
			};

			return country;
		}
	}
}
