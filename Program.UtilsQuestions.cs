using Database.Afrobarometer.Tables;

using F23.StringSimilarity;

using System.Collections.Generic;
using System.Linq;

namespace Database.Afrobarometer
{
	internal partial class Program
	{
		public static readonly double UtilQuestionDistanceThreshold = 0.15;
		public static readonly double UtilQuestionDistanceDamerauThreshold = 3;
		public static readonly double UtilQuestionDistanceJaroWinklerThreshold = UtilQuestionDistanceThreshold;
		public static readonly double UtilQuestionDistanceNGramThreshold = UtilQuestionDistanceThreshold;
		public static readonly double UtilQuestionDistanceRatcliffObershelpThreshold = UtilQuestionDistanceThreshold;

		public static readonly double UtilQuestionSimilarityThreshold = 0.96;
		public static readonly double SimilarityJaroWinkler = UtilQuestionSimilarityThreshold;
		public static readonly double SimilarityRatcliffObershelp = UtilQuestionSimilarityThreshold;

		public static readonly Damerau UtilAddQuestionDamerau = new();
		public static readonly JaroWinkler UtilAddQuestionJaroWinkler = new();
		public static readonly NGram UtilAddQuestionNGram = new();
		public static readonly RatcliffObershelp UtilAddQuestionRatcliffObershelp = new();

		public static bool UtilQuestionDistance(string q, string _q)
		{
			double distancedamerau = UtilAddQuestionDamerau.Distance(q, _q);
			double distancejarowinkler = UtilAddQuestionJaroWinkler.Distance(q, _q);
			double distancengram = UtilAddQuestionNGram.Distance(q, _q);
			double distanceratcliffobershelp = UtilAddQuestionRatcliffObershelp.Distance(q, _q);

			bool _damerau = distancedamerau < UtilQuestionDistanceDamerauThreshold;
			bool _jarowinkler = distancejarowinkler < UtilQuestionDistanceJaroWinklerThreshold;
			bool _ngram = distancengram < UtilQuestionDistanceNGramThreshold;
			bool _ratcliffobershelp = distanceratcliffobershelp < UtilQuestionDistanceRatcliffObershelpThreshold;

			bool _one = _damerau;
			bool _two = _jarowinkler && _ngram && _ratcliffobershelp;
			
			return _one || _two; 
		}
		public static bool UtilQuestionSimilarity(string q, string _q)
		{
			double similarityjarowinkler = UtilAddQuestionJaroWinkler.Similarity(q, _q);
			double similarityratcliffobershelp = UtilAddQuestionRatcliffObershelp.Similarity(q, _q);

			bool _similardamerau = similarityjarowinkler > UtilQuestionSimilarityThreshold;
			bool _similarjarowinkler = similarityratcliffobershelp > UtilQuestionSimilarityThreshold;

			bool similarone = _similardamerau && _similarjarowinkler;

			return similarone;
		}
		public static bool UtilAddQuestion(Question question, IDictionary<string, IList<string>> questions, IDictionary<string, IList<string>> keys, out string? key, out string? value)
		{
			key = null;
			value = null;

			if (question.QuestionText is null)
				return false;

			key = value = string.Empty;

			IEnumerator<KeyValuePair<string, IList<string>>> keysenumerator = keys.GetEnumerator();
			IEnumerator<KeyValuePair<string, IList<string>>> questionsenumerator = questions.GetEnumerator();
			string wouldbekey = (question.VariableLabel ?? question.QuestionText)
				.Replace(" ", string.Empty)
				.Replace(":", string.Empty)
				.Replace(",", string.Empty)
				.Replace("'", string.Empty)
				.ToLower();

			while (keysenumerator.MoveNext())
			{
				if (keysenumerator.Current.Key == wouldbekey && questions.TryGetValue(wouldbekey, out IList<string>? _wouldbekeyquestions) && _wouldbekeyquestions is not null)
				{
					key = wouldbekey;
					value = _wouldbekeyquestions[0];

					return false;
				}

				foreach (string _key in keysenumerator.Current.Value)
				{
					bool ismatch = UtilQuestionDistance(wouldbekey, _key) || UtilQuestionSimilarity(wouldbekey, _key);

					if (ismatch)
					{
						keysenumerator.Current.Value.Add(wouldbekey);
						wouldbekey = keysenumerator.Current.Key;

						if (questions.TryGetValue(wouldbekey, out IList<string>? _questions) && _questions is not null)
						{
							key = wouldbekey;
							value = _questions[0];

							return false;
						}

						break;
					}
				}
			}

			key = key == string.Empty ? wouldbekey : key;

			while (questionsenumerator.MoveNext())
			{
				foreach (string _question in questionsenumerator.Current.Value)
				{
					bool ismatch = UtilQuestionDistance(question.QuestionText, _question) || UtilQuestionSimilarity(question.QuestionText, _question);

					if (ismatch)
					{
						questionsenumerator.Current.Value.Add(question.QuestionText);
						key = questionsenumerator.Current.Key;
						value = questionsenumerator.Current.Value.First();

						return false;
					}
				}

				if (questions.TryGetValue(key, out IList<string>? _value) && _value is not null)
				{
					value = _value[0];
					_value.Add(question.QuestionText);
					return false;
				}
			}

			questions.Add(key, [ value = question.QuestionText]);

			return true;
		}
	}
}