using FaceplateDataExtractor;
using FaceplateDataExtractor.Excel;
using FaceplateDataExtractor.Model;
using FaceplateGeneratorCore.Model.Cable;
using FaceplateGeneratorCore.Model.Cable.Helper;
using FaceplateIdGenerator;
using FaceplateIdGenerator.Aggregates;
using System.Diagnostics;

namespace FaceplateGeneratorCore.Service;

public class DataExtractorService
{

    public DataExtractorService()
    {
        
    }

    public List<SystemCableData> ExtractFromMasterExcelTemplate(string filePath)
    {
        var extractor = DataExtractorFactory.CreateWithMasterTemplateFixedLayout(filePath);
        var idGenerator = new PrefixedIdentifierGenerator();
        idGenerator.StartAllSequences();

        var success = extractor.TryExtractData(0, out var data, out var rejectedData);
        if (!success) return [];

        //Dictionary<string, >
        // iterate the data and produce ids
        var cables = new List<CableData>();
        //var lastSourcePanel = "";
        foreach (var faceplateData in data)
        {
            foreach (var cableSystemData in faceplateData.CableSystemDatas)
            {
                //IdentifierType identifierType = MapToIdentifierType(cableSystemData.SystemType);
                for (int i = 0; i < cableSystemData.Quantity; i++)
                {
                    //string nextId = idGenerator.NextId(identifierType, faceplateData.PanelId);
                    var cableData = new CableData(
                        cableSystemData.SystemType,
                        "_", 
                        faceplateData.Description, 
                        faceplateData.Location, 
                        faceplateData.Room, 
                        faceplateData.AboveFinishedFloorLevel, 
                        "<cable type>", 
                        faceplateData.PanelId, 
                        cableSystemData.DestPanelId,
                        cableSystemData.Quantity);

                    cables.Add(cableData);
                }
            }
        }


        //var sortDestinationNumericFunc = (CableData cable) => {
        //    var match = Regex.Match(cable.DestinationPanelId, @"\d");
        //    return cable.DestinationPanelId.Substring(match.Index);
        //};

        //var sortedCables = cables.GroupBy(cable => cable.SystemType.ToString())
        //                            //.ThenBy(cable => cable.DestinationPanelId)
        //                            .Select(group => new {
        //                                Name = group.Key,
        //                                Cables = group.OrderBy(sortDestinationNumericFunc)
        //                                                .ThenBy(cable => cable.SourcePanelId)
        //                                                .ThenBy(cable => cable.Description)
        //                                                .ThenBy(cable => cable.Location)
        //                                                .ThenBy(cable => cable.Room)



        //                            })
        //                            //.OrderBy((cable) => { 

        //                            //    var match = Regex.Match(cable.DestinationPanelId, @"\d");
        //                            //    return cable.DestinationPanelId.Substring(match.Index);
        //                            //})
        //                            //.ThenBy(cable => cable.SourcePanelId)
        //                            //.ThenBy(cable => cable.Room)
        //                            //.ThenBy(cable => cable.Location)
        //                            .ToList();

        //var finalCableList = new List<CableData>();
        //sortedCables.ForEach(group => {
        //    var groupName = group.Name;
        //    var sortedGroupCables = group.Cables;

        //    foreach (var cable in sortedGroupCables)
        //    {
        //        IdentifierType identifierType = MapToIdentifierType(cable.SystemType);
        //        string nextId = idGenerator.NextId(identifierType, cable.DestinationPanelId);
        //        cable.AssignId(nextId);
        //        finalCableList.Add(cable);
        //    }
        //});

        //return finalCableList;

        var groups = cables.GroupBy(cable => cable.SystemType.ToString())
                            .Select(group => new {
                                Name = group.Key,
                                Cables = group.ToList()
                            })
                            .ToList();

        List<SystemCableData> systemGroups = [];
        foreach (var kvp in groups)
        {
            var sortedCables = SortingHelper.SortSystemGroup(kvp.Cables);
            var identifiedCablesOnly = TryAddIdsToCables(idGenerator, sortedCables, out var rejectedCables);

            Debug.WriteLine($"There are {rejectedCables.Count} rejected cables");
            foreach (var rc in rejectedCables)
            {
                Debug.WriteLine($"Rejected Cable: [ {rc} ]");
            }

            systemGroups.Add(new SystemCableData(kvp.Name, identifiedCablesOnly));
        }

        return systemGroups;
    }

    private List<CableData> TryAddIdsToCables(IIdentifierGenerator idGenerator, List<CableData> cables, out List<CableData> rejectedCables)
    {
        List<CableData> identifiedCables = [];
        rejectedCables = [];
        foreach (var cable in cables)
        {
            IdentifierType identifierType = MapToIdentifierType(cable.SystemType);
            if (identifierType == IdentifierType.NONE)
            {
                rejectedCables.Add(cable);
                continue;
            }

            string nextId = idGenerator.NextId(identifierType, cable.DestinationRackId); // provide the rack id used to break the batch (24)
            cable.AssignId(nextId);
            identifiedCables.Add(cable);
        }

        return identifiedCables;
    }

    public static IdentifierType MapToIdentifierType(SystemType systemType)
    {
        switch (systemType)
        {
            case SystemType.NONE:
                return IdentifierType.NONE;
            case SystemType.TECHNICAL_DATA:
                return IdentifierType.TECH_DATA;
            case SystemType.MULTIMODE_FIBER:
                return IdentifierType.MULTIMODE_FIBER;
            case SystemType.VIDEO_TIE_LINE_CV_RF_SDI_HDSDI:
                return IdentifierType.VIDEO_TIE_LINE;
            case SystemType.DIGITAL_MEDIA:
                return IdentifierType.DIGITAL_MEDIA;
            case SystemType.AV_CONTROL_DATA:
                return IdentifierType.AV_CONTROL;
            case SystemType.AUDIO_DIGITAL_ANALOG_SEND:
            case SystemType.AUDIO_DIGITAL_ANALOG_RECEIVE:
                return IdentifierType.AUDIO;
            case SystemType.DANTE_ETHERNET_AUDIO:
                return IdentifierType.DANTE_ETHERNET_AUDIO;
            case SystemType.TALKBACK:
                return IdentifierType.TALKBACK;
            case SystemType.PERFORMANCE_RELAY_INPUT:
                return IdentifierType.NONE;
            case SystemType.PAGING_STATION:
                return IdentifierType.PAGING_STATION;
            case SystemType.PAGING_VOLUME_CONTROL:
                return IdentifierType.NONE;
            case SystemType.PAGING_SPEAKER:
                return IdentifierType.PAGING_SPEAKER;
            case SystemType.PERFORMANCE_LOUDSPEAKER:
                return IdentifierType.PERFORMANCE_LOUDSPEAKER;
            case SystemType.DMX_STAGE_LIGHTING_CONTROL:
                return IdentifierType.DMX_LIGHTING_CONTROL;
            case SystemType.BLUE_WORK_LIGHT_CONTROL:
                return IdentifierType.NONE;
            case SystemType.HOIST_CONTROL:
                return IdentifierType.HOIST_CONTROL;
            case SystemType.HOUSE_CURTAIN_CONTROL:
                return IdentifierType.HOUSE_CURTAIN_CONTROL;
            case SystemType.PENDENT_CONTROL_WITH_ESTOP:
                return IdentifierType.NONE;
            case SystemType.ESTOP:
                return IdentifierType.ESTOP;
            case SystemType.STAGE_LIGHTING_OUTLETS_SINGLE_10A:
                return IdentifierType.NONE;
            case SystemType.BLUE_AND_WHITE_WORK_LIGHT_OUTLET:
                return IdentifierType.NONE;
            case SystemType.GPO_10A_DIRTY_DOUBLE_OUTLET:
                return IdentifierType.NONE;
            case SystemType.AUDIO_10A_POWER_DOUBLE_OUTLET:
                return IdentifierType.NONE;
            case SystemType.THREE_PHASE_OUTLET:
                return IdentifierType.NONE;
            default:
                throw new NotSupportedException($"Mapping for SystemType '{systemType}' not supported.");
        }
    }
}
