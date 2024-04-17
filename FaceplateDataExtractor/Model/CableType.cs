using static FaceplateDataExtractor.Utility.EnumHelper;

namespace FaceplateDataExtractor.Model
{
    /// <summary>
    /// All supported CableTypes
    /// </summary>
    /// <remarks>
    /// The <see cref="StringArrayValueAttribute"/> attached to each Enum value defines possible 
    /// string values that if found in a column header means that column header is of that type.
    /// The entire string must be matched, and any match will mean successful identification.
    /// The string values provided can assume that the strings matched will be sanitized, so that 
    /// things like double spaces, or slight variations in formatting will not throw out the match.
    /// ie. A header with a value like 'Cat     6A      S / FTP` will still match, as the extra spaces
    /// will be ignored.
    /// Note: *See the FaceplateDataExtractor.Utility.StringsHelper.Sanitize(string) method to 
    /// help write these match strings.*
    /// </remarks>
    public enum CableType
    {
        /// <summary>
        /// No type
        /// </summary>
        /// <remarks>
        /// The StringArrayValue provided for NONE should ensure it does not match any values that 
        /// might be in the spreadsheet headers.
        /// NONE must be located at the top to be assigned value of 0 to be used as default
        /// </remarks>
        [StringArrayValue(["<-None->"])] 
        NONE, 
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["Cat 6A S/FTP", "R305649"])] ETH_CAT6A_SFTP_R305649,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["Cat 6A", "U/FTP", "R308247"])] ETH_CAT6A_UFTP_R308247,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["M3", "4pair", "LC"])] FIBER_OM3_4PAIR_LC_LC,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["RG59", "3G", "SDI", "HD"])] COAX_RG59_3G_SDI_HD,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["RG6", "3G", "SDI", "HD"])] COAX_RG6_3G_SDI_HD,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["Belden", "1801B"])] BELDEN_1801B,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["Belden", "1802B"])] BELDEN_1802B,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["Belden", "1803F"])] BELDEN_1803F,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["Belden", "7880A"])] BELDEN_7880A,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["18", "AWG", "0.75mm2"])] WIRE_18_AWG_0_75mm2,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["11", "AWG", "4mm2", "with Overall Sheath"])] WIRE_11_AWG_4_00mm2_WITH_OVERALL_SHEATH,

    }
}
