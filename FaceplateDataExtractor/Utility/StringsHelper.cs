using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FaceplateDataExtractor.Utility
{
    /// <summary>
    /// Helper class for serializing to and from json strings
    /// </summary>
    internal class StringsHelper
    {
        public static readonly string Delimiter = ", ";

        /// <summary>
        /// Strips a string of unwanted characters, spaces, etc
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Sanitize(string value)
        {
            value = value.Replace(',', ' '); // remove commas as we are using them for delimiter
            value = Regex.Replace(value, @"\s{2,}", " "); // ensure all spaces are single
            value = value.Replace("{", "").Replace("}", ""); // remove curly brackets
            value = value.Replace(" /", "/").Replace("/ ", "/"); // remove spaces near slashes
            value = value.Replace("( ", "(").Replace(") ", ")"); // remove spaces inside normal brackets
            value = value.Replace("[ ", "[").Replace("] ", "]"); // remove spaces inside square brackets

            return value;
        }

        /// <summary>
        /// The Damereau-Levenshein Distance algorithm calculates the number of letter additions, subtractions, 
        /// substitutions, and transpositions (swaps) necessary to convert one string to another. The lower the 
        /// score, the more similar they are.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int ComputeLevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (string.IsNullOrEmpty(t))
                    return 0;
                return t.Length;
            }

            if (string.IsNullOrEmpty(t))
            {
                return s.Length;
            }

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // initialize the top and right of the table to 0, 1, 2, ...
            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 1; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    int min1 = d[i - 1, j] + 1;
                    int min2 = d[i, j - 1] + 1;
                    int min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return d[n, m];
        }

        public static string ListToString(List<string> strings)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < strings.Count; i++)
            {
                var _str = strings[i];
                if (_str == "") 
                    continue;

                sb.Append(Sanitize(strings[i]));
                if (i != strings.Count - 1) sb.Append(Delimiter);
            }

            return sb.ToString();
            //return JsonSerializer.Serialize(strings);
        }

        /// <summary>
        /// This method should be called only on a string that was generated from
        /// the <see cref="ListToString(List{string})"/> method.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<string> StringToList(string str)
        {
            return [.. str.Split(Delimiter)];

            //var list = JsonSerializer.Deserialize<List<string>>(str);

            //if (list == null)
            //    return [];
            //else
            //    return list;
        }

    }
}
