using ExcelCableGeneratorApp.Utility;

namespace ExcelCableGeneratorApp.Extract.Aggregates;

public class SystemCableData
{
    public string PanelId { get; }
    public string Description { get; }
    public string Location { get; }
    public string Room { get; }
    public string AboveFinFloorLvl { get; }
    public string SystemType { get; }
    public string CableType { get; }
    public string QuantityType { get; }
    public int Quantity { get; }
    public string DestinationId { get; }

    public SystemCableData(string panelId, string description, string location, string room, string aboveFinFloorLvl, string systemType, string cableType, string quantityType, int quantity, string destinationId)
    {
        PanelId = panelId;
        Description = description;
        Location = location;
        Room = room;
        AboveFinFloorLvl = aboveFinFloorLvl;
        SystemType = systemType;
        CableType = cableType;
        QuantityType = quantityType;
        Quantity = quantity;
        DestinationId = destinationId;
    }

    public override string ToString()
    {
        return $"PanelId='{PanelId}', Desc='{Description}', Loc='{Location}', Rm='{Room}', AFFL='{AboveFinFloorLvl}', SysType='{SystemType}', CableType='{CableType}', QtyType='{QuantityType}', Qty='{Quantity}', RackId='{DestinationId}'";
    }

    public SystemCableData CreateSanitizedObject()
    {
        return new SystemCableData(
            StringHelper.Sanitize(PanelId),
            StringHelper.Sanitize(Description),
            StringHelper.Sanitize(Location),
            StringHelper.Sanitize(Room),
            StringHelper.Sanitize(AboveFinFloorLvl),
            StringHelper.Sanitize(SystemType),
            StringHelper.Sanitize(CableType),
            StringHelper.Sanitize(QuantityType),
            Quantity,
            StringHelper.Sanitize(DestinationId)
        );
    }
}
