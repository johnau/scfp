using FaceplateDataExtractor.Utility;
using System.Diagnostics;

namespace FaceplateDataExtractor.Utility
{
    internal class EnumHelper
    {

        /// <summary>
        /// </summary>
        /// <remarks>
        /// Enum type used with this method should ensure that a default value is considered.
        /// If no matches are found, the default value is returned.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="type"></param>
        /// <returns>Finds the longest match and returns, or returns Enum default value</returns>
        /// <exception cref="Exception"></exception>
        public static bool ContainsMatchingType<T>(string s, out T type) where T : Enum
        {
            Debug.WriteLine($"Checking string: {s} to find match in type {typeof(T)}");
            s = StringsHelper.Sanitize(s);

            var matches = new Dictionary<T, string>();

            foreach (T _type in Enum.GetValues(typeof(T)))
            {
                var enumTypeValueOptions = _type.GetStringArrayValue();
                for (int i = 0; i < enumTypeValueOptions.Length; i++)
                {
                    var currentMatch = enumTypeValueOptions[i];
                    if (currentMatch.StartsWith(s, StringComparison.OrdinalIgnoreCase))
                    {
                        var success = matches.TryAdd(_type, currentMatch);
                        //matches.Add(_type, currentMatch);
                    }
                }
            }
            
            if (matches.Count == 0)
            {
                Debug.WriteLine($"!! Checking string: {s} could not match a type in: {typeof(T)}");
                type = default!; // ignoring nulls - enums should ensure NONE is first (= 0 = default)
                return false;
            }

            // sort the matches and find the one with the most matched characters.
            // This is a little janky since it's possible that two items match the same character
            // length and we can get the wrong one.
            // For now it should be ok, and most cases should be ok.
            // This can be reviewed when we shift the enums to the database.
            var longestMatch = "";
            T longestMatchValue = default!;
            foreach (var match in matches)
            {
                var e = match.Key;
                var str = match.Value;
                if (str.Length > longestMatch.Length)
                {
                    longestMatch = str;
                    longestMatchValue = e;
                }
                else if (str.Length == longestMatch.Length)
                {
                    throw new Exception($"The enum {typeof(T)} needs to be modified since there are two or more values that are colliding during detection. ({str} and {longestMatch})  This error was somewhat expected at some point...");
                }
            }

            type = longestMatchValue;
            Debug.WriteLine($"> Checked string: {s} matched {type}");
            return true;
        }

        /// <summary>
        /// Give a property a string value
        /// </summary>
        public class StringValueAttribute : Attribute
        {
            public string Value { get; }

            public StringValueAttribute(string value)
            {
                Value = value;
            }
        }

        /// <summary>
        /// Give a property a string[] value
        /// </summary>
        public class StringArrayValueAttribute : Attribute
        {
            public string[] Value { get; }

            public StringArrayValueAttribute(string[] value)
            {
                Value = value;
            }
        }

    }
}
