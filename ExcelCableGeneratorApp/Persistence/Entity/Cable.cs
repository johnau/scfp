namespace ExcelCableGeneratorApp.Persistence.Entity;

internal class Cable
{
    public int CableId { get; set; }
    public string StageCraftCableId { get; set; }
    public string SystemType { get; set; }
    public string CableType { get; set; }
    public string PanelId { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public string Room { get; set; }
    public string AboveFinFloorLvl { get; set; }
    public string QuantityType { get; set; }
    public int Quantity { get; set; }
    public string DestinationId { get; set; }
    public int JobId { get; set; }
    public Job? Job { get; set; }

    public Cable()
    {
        StageCraftCableId = "";
        SystemType = "";
        CableType = "";
        PanelId = "";
        Description = "";
        Location = "";
        Room = "";
        AboveFinFloorLvl = "";
        QuantityType = "";
        DestinationId = "";
    }
}
