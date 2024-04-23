using System.Text.RegularExpressions;

namespace ExcelCableGeneratorApp.Utility;
internal partial class StringHelper
{
    [GeneratedRegex(@"\s{2,}")]
    private static partial Regex DoubleSpaceRegex();

    public static readonly string Delimiter = ", ";

    /// <summary>
    /// Strips a string of unwanted characters, spaces, etc
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string Sanitize(string value)
    {
        value = value.Replace(',', ' '); // remove commas as we are using them for delimiter
        value = DoubleSpaceRegex().Replace(value, " "); // ensure all spaces are single
        value = value.Replace("{", "").Replace("}", ""); // remove curly brackets
        value = value.Replace(" /", "/").Replace("/ ", "/"); // remove spaces near slashes
        value = value.Replace("( ", "(").Replace(") ", ")"); // remove spaces inside normal brackets
        value = value.Replace("[ ", "[").Replace("] ", "]"); // remove spaces inside square brackets

        return value;
    }

    public static string StripAllNonAlphanumericChars(string input)
    {
        string pattern = "[^a-zA-Z0-9]";
        string result = Regex.Replace(input, pattern, "");
        return result.Length > 0 ? (result.Length > 20 ? result.Substring(0, 20) : result) : "";
    }
}
