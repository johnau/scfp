using static FaceplateDataExtractor.Utility.EnumHelper;

namespace FaceplateDataExtractor.Model
{
    /// <summary>
    /// All supported System Types
    /// </summary>
    public enum SystemType
    {
        [StringArrayValue(["None"])] NONE,
        [StringArrayValue(["Technical Data"])] TECHNICAL_DATA,
        [StringArrayValue(["Multimode Fiber"])] MULTIMODE_FIBER,
        [StringArrayValue(["Video Tie Line", "CV/RF/SDI/HDSI"])] VIDEO_TIE_LINE_CV_RF_SDI_HDSDI,
        [StringArrayValue(["Digital Media"])] DIGITAL_MEDIA,
        [StringArrayValue(["AV Control Data"])] AV_CONTROL_DATA,
        [StringArrayValue(["Audio Digital", "Analogue", "Send"])] AUDIO_DIGITAL_ANALOG_SEND,
        [StringArrayValue(["Audio Digital", "Analogue", "Return"])] AUDIO_DIGITAL_ANALOG_RECEIVE,
        [StringArrayValue(["Ethernet Audio", "Dante"])] DANTE_ETHERNET_AUDIO,
        [StringArrayValue(["Talkback"])] TALKBACK,
        [StringArrayValue(["Performance Relay Input"])] PERFORMANCE_RELAY_INPUT,
        [StringArrayValue(["Paging Station"])] PAGING_STATION,
        [StringArrayValue(["Paging Volume Control"])] PAGING_VOLUME_CONTROL,
        [StringArrayValue(["Paging Speaker"])] PAGING_SPEAKER,
        [StringArrayValue(["Performance Loudspeaker"])] PERFORMANCE_LOUDSPEAKER,
        [StringArrayValue(["Stage Lighting Control", "DMX"])] DMX_STAGE_LIGHTING_CONTROL,
        [StringArrayValue(["Blue", "Work Light Control"])] BLUE_WORK_LIGHT_CONTROL,
        [StringArrayValue(["Hoist Control"])] HOIST_CONTROL,
        [StringArrayValue(["House Curtain Control"])] HOUSE_CURTAIN_CONTROL,
        [StringArrayValue(["Pendent Control", "Estop"])] PENDENT_CONTROL_WITH_ESTOP,
        [StringArrayValue(["Estop"])] ESTOP,
        [StringArrayValue(["Stage Lighting Outlets", "Single", "10A"])] STAGE_LIGHTING_OUTLETS_SINGLE_10A,
        [StringArrayValue(["Blue", "Work Light Outlet"])] BLUE_AND_WHITE_WORK_LIGHT_CONTROL,
        [StringArrayValue(["10A", "Dirty", "GPO", "Double Outlet"])] GPO_10A_DIRTY_DOUBLE_OUTLET,
        [StringArrayValue(["10A", "Audio Power", "Double Outlet"])] AUDIO_10A_POWER_DOUBLE_OUTLET,
        [StringArrayValue(["3 Phase", "Outlet"])] THREE_PHASE_OUTLET,
    }

}
