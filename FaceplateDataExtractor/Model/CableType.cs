using static FaceplateDataExtractor.Utility.EnumHelper;

namespace FaceplateDataExtractor.Model
{
    /// <summary>
    /// All supported CableTypes
    /// </summary>
    public enum CableType
    {
        [StringArrayValue(["None"])] NONE,
        [StringArrayValue(["Cat 6A", "S/FTP", "R305649"])] ETH_CAT6A_SFTP_R305649,
        [StringArrayValue(["Cat 6A", "U/FTP", "R308247"])] ETH_CAT6A_UFTP_R308247,
        [StringArrayValue(["M3", "4pair", "LC"])] FIBER_OM3_4PAIR_LC_LC,
        [StringArrayValue(["RG59", "3G", "SDI", "HD"])] COAX_RG59_3G_SDI_HD,
        [StringArrayValue(["RG6", "3G", "SDI", "HD"])] COAX_RG6_3G_SDI_HD,
        [StringArrayValue(["Belden", "1801B"])] BELDEN_1801B,
        [StringArrayValue(["Belden", "1802B"])] BELDEN_1802B,
        [StringArrayValue(["Belden", "1803F"])] BELDEN_1803F,
        [StringArrayValue(["Belden", "7880A"])] BELDEN_7880A,
        [StringArrayValue(["18", "AWG", "0.75mm2"])] WIRE_18_AWG_0_75mm2,
        [StringArrayValue(["11", "AWG", "4mm2", "with Overall Sheath"])] WIRE_11_AWG_4_00mm2_WITH_OVERALL_SHEATH,
    }
}
