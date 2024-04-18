using FaceplateDataExtractor;
using FaceplateDataExtractor.Excel;
using FaceplateDataExtractor.Model;
using FaceplateGeneratorCore.Data;
using FaceplateIdGenerator;
using FaceplateIdGenerator.Aggregates;
using System.Diagnostics;
using System.Reflection;

namespace FaceplateGeneratorCore.Service;

public class DataExtractorService
{

    public DataExtractorService()
    {
        
    }

    public List<CableData> ExtractFromMasterExcelTemplate(string filePath)
    {
        var extractor = DataExtractorFactory.CreateWithMasterTemplateFixedLayout(filePath);
        var idGenerator = new PrefixedIdentifierGenerator();
        idGenerator.StartAllSequences();

        var success = extractor.TryExtractData(0, out var data, out var rejectedData);
        if (!success) return [];

        //Dictionary<string, >
        // iterate the data and produce ids
        var cables = new List<CableData>();
        foreach (var faceplateData in data)
        {
            foreach (var cableSystemData in faceplateData.CableSystemDatas)
            {
                IdentifierType identifierType = MapToIdentifierType(cableSystemData.SystemType);

                for (int i = 0; i < cableSystemData.Quantity; i++)
                {
                    string nextId = idGenerator.NextId(identifierType);
                    var cableData = new CableData(
                        nextId, 
                        faceplateData.Description, 
                        faceplateData.Location, 
                        faceplateData.Room, 
                        faceplateData.AboveFinishedFloorLevel, 
                        "<cable type>", 
                        faceplateData.PanelId, 
                        cableSystemData.Destination);

                    cables.Add(cableData);
                }
            }
        }

        return cables;
    }

    public static IdentifierType MapToIdentifierType(SystemType systemType)
    {
        switch (systemType)
        {
            case SystemType.TECHNICAL_DATA:
                return IdentifierType.TECH_PANEL;
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
            default:
                throw new NotSupportedException($"Mapping for SystemType '{systemType}' not supported.");
        }
    }
}
