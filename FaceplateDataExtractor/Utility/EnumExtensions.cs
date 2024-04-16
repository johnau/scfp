using static FaceplateDataExtractor.Utility.EnumHelper;

namespace FaceplateDataExtractor.Utility
{
    public static class EnumExtensions
    {
        //public static string GetStringValue(this Enum value)
        //{
        //    var type = value.GetType();
        //    var fieldInfo = type.GetField(value.ToString());
        //    var stringValueAttribute = fieldInfo!.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
        //    return stringValueAttribute!.Length > 0 ? stringValueAttribute[0].Value : "";
        //}

        public static string[] GetStringArrayValue(this Enum value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var stringArrayValues = fieldInfo!.GetCustomAttributes(typeof(StringArrayValueAttribute), false) as StringArrayValueAttribute[];

            var strings = new string[stringArrayValues!.Length];
            for (int i = 0; i < stringArrayValues!.Length; i++)
            {
                strings[i] = stringArrayValues[0].Value[i];
            }

            return strings.Length > 0 ? strings : [];
        }
    }
}
