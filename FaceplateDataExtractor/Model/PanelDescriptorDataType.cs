using static FaceplateDataExtractor.Utility.EnumHelper;

namespace FaceplateDataExtractor.Model
{
    /// <summary>
    /// The simple headers at the table that provide metadata about the cable or
    /// group of cables in the various systems.
    /// </summary>
    /// <remarks>
    /// This will eventually move to database.
    /// *Note: String values for Enum values must be unique like the enum value itself.
    /// </remarks>
    public enum PanelDescriptorDataType
    {
        [StringArrayValue(["None"])]
        NONE,
        [StringArrayValue(["Panel Id"])]
        PANEL_ID,
        [StringArrayValue(["Description"])]
        DESCRIPTION = 2,
        [StringArrayValue(["Location"])]
        LOCATION = 4,
        [StringArrayValue(["Room"])]
        ROOM = 8,
        [StringArrayValue(["AFFL"])]
        AFFL = 16
    }
}
