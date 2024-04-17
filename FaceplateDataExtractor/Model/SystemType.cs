using static FaceplateDataExtractor.Utility.EnumHelper;

namespace FaceplateDataExtractor.Model
{
    /// <summary>
    /// All supported System Types
    /// </summary>
    /// <remarks>
    /// The <see cref="StringArrayValueAttribute"/> attached to each Enum value defines possible 
    /// string values that if found in a column header means that column header is of that type.
    /// The entire string must be matched, and any match will mean successful identification.
    /// The string values provided can assume that the strings matched will be sanitized, so that 
    /// things like double spaces, or slight variations in formatting will not throw out the match.
    /// ie. A header with a value like 'Cat     6A      S / FTP` will still match, as the extra spaces
    /// will be ignored.
    /// * Note: for some of the matches we may need regex (ie. for audio digital/analogue send/return)
    /// Note: *See the FaceplateDataExtractor.Utility.StringsHelper.Sanitize(string) method to 
    /// help write these match strings.*
    /// </remarks>
    public enum SystemType
    {
        /// <summary>
        /// No type
        /// </summary>
        /// <remarks>
        /// The StringArrayValue provided for NONE should ensure it does not match any values that 
        /// might be in the spreadsheet headers.
        /// NONE must be located at the top to be assigned value of 0 to be used as default
        /// </remarks>
        [StringArrayValue(["<-None->"])] NONE,
        /// <summary>
        /// Technical Data Panel Column
        /// </summary>
        [StringArrayValue(["Technical Data"])] 
        TECHNICAL_DATA,
        /// <summary>
        /// Multimode Fiber column
        /// </summary>
        [StringArrayValue(["Multimode Fiber"])] 
        MULTIMODE_FIBER,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["Video Tie Line", "CV/RF/SDI/HDSI"])] 
        VIDEO_TIE_LINE_CV_RF_SDI_HDSDI,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["Digital Media"])] 
        DIGITAL_MEDIA,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["AV Control Data"])] 
        AV_CONTROL_DATA,
        /// <summary>
        /// There is an issue detecting these columns still
        /// </summary>
        [StringArrayValue(["Audio Digital/Analogue", "Send"])] 
        AUDIO_DIGITAL_ANALOG_SEND,
        /// <summary>
        /// There is an issue detecting these columns still
        /// </summary>
        [StringArrayValue(["Audio Digital/Analogue", "Return"])] 
        AUDIO_DIGITAL_ANALOG_RECEIVE,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["Ethernet Audio (Dante)"])] 
        DANTE_ETHERNET_AUDIO,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["Talkback"])] 
        TALKBACK,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["Performance Relay Input"])] 
        PERFORMANCE_RELAY_INPUT,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["Paging Station"])] 
        PAGING_STATION,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["Paging Volume Control"])] 
        PAGING_VOLUME_CONTROL,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["Paging Speaker"])] 
        PAGING_SPEAKER,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["Performance Loudspeaker"])] 
        PERFORMANCE_LOUDSPEAKER,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["Stage Lighting Control (DMX)", "Stage Lighting Control DMX"])] 
        DMX_STAGE_LIGHTING_CONTROL,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["Blue/Work Light Control"])] 
        BLUE_WORK_LIGHT_CONTROL,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["Hoist Control"])] 
        HOIST_CONTROL,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["House Curtain Control"])] 
        HOUSE_CURTAIN_CONTROL,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["Pendent Control (with ESTOP)"])] 
        PENDENT_CONTROL_WITH_ESTOP,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["ESTOP"])] 
        ESTOP,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["Stage Lighting Outlets Single 10A"])] 
        STAGE_LIGHTING_OUTLETS_SINGLE_10A,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["Blue/Work Light Outlet"])] 
        BLUE_AND_WHITE_WORK_LIGHT_CONTROL,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["10A 'Dirty' GPO Double Outlet"])] 
        GPO_10A_DIRTY_DOUBLE_OUTLET,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["10A Audio Power Double Outlet"])] 
        AUDIO_10A_POWER_DOUBLE_OUTLET,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["3 Phase Outlet"])] 
        THREE_PHASE_OUTLET,
    }

}
