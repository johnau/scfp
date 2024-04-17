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
