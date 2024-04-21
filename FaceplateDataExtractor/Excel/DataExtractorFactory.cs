using FaceplateDataExtractor.Model.Mapper;
using FaceplateDataExtractor.Model;
using static FaceplateDataExtractor.Excel.UserControlledXlFaceplateDataExtractor;

namespace FaceplateDataExtractor.Excel
{
    public class DataExtractorFactory
    {
        public static UserControlledXlFaceplateDataExtractor CreateWithMasterTemplateFixedLayout(string filePath)
        {
            var dataStartRow = 6;
            var dataEndRow = 317; // TODO: WARNING: NEED TO FIX THIS
            var panelIdLayout = new ColumnLayout(PanelDescriptorDataType.PANEL_ID.ToString(), 1, 4, 4);
            var descriptionLayout = new ColumnLayout(PanelDescriptorDataType.DESCRIPTION.ToString(), 2, 4, 4);
            var locationLayout = new ColumnLayout(PanelDescriptorDataType.LOCATION.ToString(), 3, 4, 4);
            var roomLayout = new ColumnLayout(PanelDescriptorDataType.ROOM.ToString(), 4, 4, 4);
            var afflLayout = new ColumnLayout(PanelDescriptorDataType.AFFL.ToString(), 5, 4, 4);

            // grouped columns (system data)
            var technicalDataQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 6, 2, 4);
            var technicalDataDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 7, 2, 4);
            var technicalDataGroup = new ColumnSet(
                "TECHNICAL DATA",
                [technicalDataQtyLayout, technicalDataDestLayout]
            );

            var mmFiberQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 8, 2, 4);
            var mmFiberDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 9, 2, 4);
            var mmDataGroup = new ColumnSet(
                "MULTIMODE FIBER",
                [mmFiberQtyLayout, mmFiberDestLayout]
            );

            var vtlQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 10, 2, 4);
            var vtlDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 11, 2, 4);
            var vtlDataGroup = new ColumnSet(
                "VIDEO TIE LINE",
                [vtlQtyLayout, vtlDestLayout]
            );

            var digMediaQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 12, 2, 4);
            var digMediaDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 13, 2, 4);
            var digMediaGroup = new ColumnSet(
                "DIGITAL MEDIA",
                [digMediaQtyLayout, digMediaDestLayout]
            );

            var avControlQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 14, 2, 4);
            var avControlDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 15, 2, 4);
            var avControlGroup = new ColumnSet(
                "AV CONTROL",
                [avControlQtyLayout, avControlDestLayout]
            );

            var audioDigiAnlgSendQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY_MALE.ToString(), 16, 2, 4);
            var audioDigiAnlgSendDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 18, 2, 4);
            var audioSendGroup = new ColumnSet(
                "AUDIO DIGITAL/ANALOG SEND",
                [audioDigiAnlgSendQtyLayout, audioDigiAnlgSendDestLayout]
            );

            var audioDigiAnlgReturnQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY_FEMALE.ToString(), 17, 2, 4);
            var audioDigiAnlgReturnDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 18, 2, 4);
            var audioReturnGroup = new ColumnSet(
                "AUDIO DIGITAL/ANALOG RETURN",
                [audioDigiAnlgReturnQtyLayout, audioDigiAnlgReturnDestLayout]
            );

            var danteEthernetAudioQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 19, 2, 4);
            var danteEthernetAudioDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 20, 2, 4);
            var danteEthernetGroup = new ColumnSet(
                "DANTE ETHERNET AUDIO",
                [danteEthernetAudioQtyLayout, danteEthernetAudioDestLayout]
            );

            var talkbackQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 21, 2, 4);
            var talkbackDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 22, 2, 4);
            var talkbackGroup = new ColumnSet(
                "TALKBACK",
                [talkbackQtyLayout, talkbackDestLayout]
            );

            var performanceRelayInputQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 23, 2, 4);
            var performanceRelayInputDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 24, 2, 4);
            var performanceRelayGroup = new ColumnSet(
                "PERFORMANCE RELAY INPUT",
                [performanceRelayInputQtyLayout, performanceRelayInputDestLayout]
            );

            var pagingStationQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 25, 2, 4);
            var pagingStationDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 26, 2, 4);
            var pagingStationGroup = new ColumnSet(
                "PAGING STATION",
                [pagingStationQtyLayout, pagingStationDestLayout]
            );

            var pagingSpeakerQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 27, 2, 4);
            var pagingSpeakerDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 28, 2, 4);
            var pagingSpeakerGroup = new ColumnSet(
                "PAGING SPEAKER",
                [pagingSpeakerQtyLayout, pagingSpeakerDestLayout]
            );

            var performanceLoudspeakerQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 29, 2, 4);
            var performanceLoudspeakerDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 30, 2, 4);
            var performanceLoudspeakerGroup = new ColumnSet(
                "PERFORMANCE LOUDSPEAKER",
                [performanceLoudspeakerQtyLayout, performanceLoudspeakerDestLayout]
            );

            var dmxLightingControlQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 31, 2, 4);
            var dmxLightingControlDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 32, 2, 4);
            var dmxLightingControlGroup = new ColumnSet(
                "STAGE LIGHTING CONTROL DMX",
                [dmxLightingControlQtyLayout, dmxLightingControlDestLayout]
            );

            var blueWorkLightControlQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 33, 2, 4);
            var blueWorkLightControlDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 34, 2, 4);
            var blueWorkLightControlGroup = new ColumnSet(
                "WORK LIGHT CONTROL",
                [blueWorkLightControlQtyLayout, blueWorkLightControlDestLayout]
            );

            var hoistControlQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 35, 2, 4);
            var hoistControlDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 36, 2, 4);
            var hoistControlGroup = new ColumnSet(
                "HOIST CONTROL",
                [hoistControlQtyLayout, hoistControlDestLayout]
            );

            var houseCurtainControlQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 37, 2, 4);
            var houseCurtainControlDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 38, 2, 4);
            var houseCurtainControlGroup = new ColumnSet(
                "HOUSE CURTAIN CONTROL",
                [houseCurtainControlQtyLayout, houseCurtainControlDestLayout]
            );

            var pendentControlQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 39, 2, 4);
            var pendentControlDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 40, 2, 4);
            var pendentControlGroup = new ColumnSet(
                "PENDENT CONTROL",
                [pendentControlQtyLayout, pendentControlDestLayout]
            );

            var estopQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 41, 2, 4);
            var estopDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 42, 2, 4);
            var estopGroup = new ColumnSet(
                "ESTOP",
                [estopQtyLayout, estopDestLayout]
            );

            var stageLightingOutletsQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 43, 2, 4);
            var stageLightingOutletsDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 44, 2, 4);
            var stageLightingOutletsGroup = new ColumnSet(
                "STAGE LIGHTING OUTLETS SINGLE 10A",
                [stageLightingOutletsQtyLayout, stageLightingOutletsDestLayout]
            );

            var blueWorkOutletsQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 45, 2, 4);
            var blueWorkOutletsDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 46, 2, 4);
            var blueWorkOutletsGroup = new ColumnSet(
                "WORK LIGHT OUTLET",
                [blueWorkOutletsQtyLayout, blueWorkOutletsDestLayout]
            );

            var gpoDirtyDoubleOutletQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 47, 2, 4);
            var gpoDirtyDoubleOutletDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 48, 2, 4);
            var gpoDirtyDoubleOutletGroup = new ColumnSet(
                "10A 'DIRTY' GPO DOUBLE OUTLET",
                [gpoDirtyDoubleOutletQtyLayout, gpoDirtyDoubleOutletDestLayout]
            );

            var audioPowerQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 49, 2, 4);
            var audioPowerDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 50, 2, 4);
            var audioPowerGroup = new ColumnSet(
                "10A AUDIO POWER DOUBLE OUTLET",
                [audioPowerQtyLayout, audioPowerDestLayout]
            );

            var threePhaseOuletQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 51, 2, 4);
            var threePhaseOuletDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 52, 2, 4);
            var threePhaseOuletGroup = new ColumnSet(
                "3 PHASE OULET",
                [threePhaseOuletQtyLayout, threePhaseOuletDestLayout]
            );


            var columns = new Dictionary<string, ColumnLayout>();
            columns[panelIdLayout.Name] = panelIdLayout;
            columns[descriptionLayout.Name] = descriptionLayout;
            columns[locationLayout.Name] = locationLayout;
            columns[roomLayout.Name] = roomLayout;
            columns[afflLayout.Name] = afflLayout;

            var columnGroups = new Dictionary<string, ColumnSet>() {
                {technicalDataGroup.Name, technicalDataGroup },
                {mmDataGroup.Name, mmDataGroup },
                {vtlDataGroup.Name, vtlDataGroup },
                {digMediaGroup.Name, digMediaGroup },
                {avControlGroup.Name, avControlGroup },
                {audioSendGroup.Name, audioSendGroup },
                {audioReturnGroup.Name, audioReturnGroup },
                {danteEthernetGroup.Name, danteEthernetGroup },
                {talkbackGroup.Name, talkbackGroup },
                {performanceRelayGroup.Name, performanceRelayGroup },
                {pagingStationGroup.Name, pagingStationGroup },
                {pagingSpeakerGroup.Name, pagingSpeakerGroup },
                {performanceLoudspeakerGroup.Name, performanceLoudspeakerGroup },
                {dmxLightingControlGroup.Name, dmxLightingControlGroup },
                {blueWorkLightControlGroup.Name, blueWorkLightControlGroup },
                {hoistControlGroup.Name, hoistControlGroup },
                {houseCurtainControlGroup.Name, houseCurtainControlGroup },
                {pendentControlGroup.Name, pendentControlGroup },
                {estopGroup.Name, estopGroup },
                {stageLightingOutletsGroup.Name, stageLightingOutletsGroup },
                {blueWorkOutletsGroup.Name, blueWorkOutletsGroup },
                {gpoDirtyDoubleOutletGroup.Name, gpoDirtyDoubleOutletGroup },
                {audioPowerGroup.Name, audioPowerGroup },
                {threePhaseOuletGroup.Name, threePhaseOuletGroup },
            };

            var config = new Configuration(dataStartRow, dataEndRow, columns, columnGroups);
            var extractor = new UserControlledXlFaceplateDataExtractor(filePath, 1, config);
            
            return extractor;
        }
    }
}
