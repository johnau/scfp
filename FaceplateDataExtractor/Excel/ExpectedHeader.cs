using static FaceplateDataExtractor.Utility.EnumHelper;

namespace FaceplateDataExtractor.Excel
{
    public enum ExpectedHeader
    {
        [StringValue("panel id")]
        PANEL_ID = 1,
        [StringValue("description")]
        DESCRIPTION = 2,
        [StringValue("location")]
        LOCATION = 4,
        [StringValue("room")]
        ROOM = 8,
        [StringValue("affl")]
        AFFL = 16
    }

    /// <summary>
    /// Want to move this with the other EnumExtensions - Need to change location due to internal limitations
    /// </summary>
    public static class EnumExtension
    {
        public static string GetStringValue(this Enum value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var stringValueAttribute = fieldInfo!.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
            return stringValueAttribute!.Length > 0 ? stringValueAttribute[0].Value : "";
        }
    }
}
