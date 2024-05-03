namespace ExcelCableGeneratorApp.Dxf.Aggregates.Data;

internal class SourcePanelContents
{
    public string SourcePanelId { get; set; }
    public List<SystemGroupContents> SystemGroups { get; set; }

    public SourcePanelContents()
    {
        SourcePanelId = "Not set";
        SystemGroups = [];
    }
}
