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
}
