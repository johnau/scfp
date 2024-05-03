namespace ExcelCableGeneratorApp.Dxf.Aggregates.Data;

internal class SystemGroupContents
{
    public string SystemName { get; set; }
    public Dictionary<string, SocketFormat> Sockets { get; set; }

    public SystemGroupContents()
    {
        SystemName = "Not set";
        Sockets = [];
    }
}
