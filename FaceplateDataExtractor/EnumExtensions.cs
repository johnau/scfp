using static FaceplateDataExtractor.Utility.EnumHelper;

namespace FaceplateDataExtractor
{
    public static class EnumExtensions
    {
        public static string[] GetStringArrayValue(this Enum value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var stringArrayValues = fieldInfo!.GetCustomAttributes(typeof(StringArrayValueAttribute), false) as StringArrayValueAttribute[];

            var strs = stringArrayValues![0].Value; // this is a bad usage of this
            var strings = new string[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                strings[i] = strs[i];
            }

            return strings.Length > 0 ? strings : [];
        }

        public static string GetStringValue(this Enum value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var stringValueAttribute = fieldInfo!.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
            return stringValueAttribute!.Length > 0 ? stringValueAttribute[0].Value : "";
        }
    }
}
