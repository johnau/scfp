using ExcelCableGeneratorApp.Identifier.Aggregates;
using ExcelCableGeneratorApp.Persistence.Entity;

namespace ExcelCableGeneratorApp.Persistence.Mapper;

internal class JobToEntityMapper
{
    public static Job MapToEntity(string jobName, List<IdentifiedCableGroup> modelSystemGroups)
    {
        var jobEntity = new Job
        {
            Name = jobName,
            Cables = []
        };

        foreach (var modelGroup in modelSystemGroups)
        {
            var systemType = modelGroup.Name;
            var cables = modelGroup.Cables;

            foreach (var modelCabel in cables)
            {
                var cableEntity = MapToEntity(modelCabel);
                cableEntity.Job = jobEntity;
                jobEntity.Cables.Add(cableEntity);
            }
        }

        return jobEntity;
    }

    public static Cable MapToEntity(IdentifiedCable model)
    {
        var cable = new Cable();
        //cable.CableId = 
        cable.StageCraftCableId = model.Id.IdFull;
        cable.SystemType = model.Cable.SystemType;
        cable.CableType = model.Cable.CableType;
        cable.PanelId = model.Cable.PanelId;
        cable.Description = model.Cable.Description;
        cable.Location = model.Cable.Location;
        cable.Room = model.Cable.Room;
        cable.AboveFinFloorLvl = model.Cable.AboveFinFloorLvl;
        cable.QuantityType = model.Cable.QuantityType;
        cable.Quantity = model.Cable.Quantity;
        cable.DestinationId = model.Cable.DestinationId;
        //cable.JobId = model.C
        //cable.Job
        return cable;
    }
}
