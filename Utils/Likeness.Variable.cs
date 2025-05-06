using F23.StringSimilarity;

namespace Database.Afrobarometer
{
	public static partial class Utils
	{
		public static partial class Likeness
		{
			public static class Variable
			{
				public static readonly double DistanceThreshold = 0.15;
				public static readonly double DistanceDamerauThreshold = 3;
				public static readonly double DistanceJaroWinklerThreshold = DistanceThreshold;
				public static readonly double DistanceNGramThreshold = DistanceThreshold;
				public static readonly double DistanceRatcliffObershelpThreshold = DistanceThreshold;

				public static readonly double SimilarityThreshold = 0.96;
				public static readonly double SimilarityJaroWinkler = SimilarityThreshold;
				public static readonly double SimilarityRatcliffObershelp = SimilarityThreshold;

				public static readonly Damerau _Damerau = new();
				public static readonly JaroWinkler _JaroWinkler = new();
				public static readonly NGram _NGram = new();
				public static readonly RatcliffObershelp _RatcliffObershelp = new();

				public static bool Distance(string q, string _q, out double percentage)
				{
					double distancedamerau = _Damerau.Distance(q, _q);
					double distancejarowinkler = _JaroWinkler.Distance(q, _q);
					double distancengram = _NGram.Distance(q, _q);
					double distanceratcliffobershelp = _RatcliffObershelp.Distance(q, _q);

					percentage = (double)((distancejarowinkler + distancengram + distanceratcliffobershelp) / 3D);
					//percentage = (double)((distancedamerau + distancejarowinkler + distancengram + distanceratcliffobershelp) / 4D);

					bool _damerau = distancedamerau < DistanceDamerauThreshold;
					bool _jarowinkler = distancejarowinkler < DistanceJaroWinklerThreshold;
					bool _ngram = distancengram < DistanceNGramThreshold;
					bool _ratcliffobershelp = distanceratcliffobershelp < DistanceRatcliffObershelpThreshold;

					bool _one = _damerau;
					bool _two = _jarowinkler && _ngram && _ratcliffobershelp;

					return _one || _two;
				}
				public static bool Similarity(string q, string _q, out double percentage)
				{
					double similarityjarowinkler = _JaroWinkler.Similarity(q, _q);
					double similarityratcliffobershelp = _RatcliffObershelp.Similarity(q, _q);

					percentage = (double)((similarityjarowinkler + similarityratcliffobershelp) / 2D);

					bool _similardamerau = similarityjarowinkler > SimilarityThreshold;
					bool _similarjarowinkler = similarityratcliffobershelp > SimilarityThreshold;

					bool similarone = _similardamerau && _similarjarowinkler;

					return similarone;
				}

				public static string ToKey(string str)
				{
					return Likeness.ToKey(str);
				}
			}
		}
	}
}
