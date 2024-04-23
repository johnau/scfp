namespace ExcelCableGeneratorApp.Dxf.Drawing.Element;

internal class PanelSection : LabelledDrawingObject
{
    private List<SocketGroup> _socketGroups;
    public List<SocketGroup> SocketGroups { get => new List<SocketGroup>(_socketGroups); }

    public PanelSection(string nameTag, float margin_Top = 0, float margin_Bottom = 0, float margin_Left = 0, float margin_Right = 0) 
        : base(nameTag, margin_Top, margin_Bottom, margin_Left, margin_Right)
    {
        _socketGroups = [];
    }

    public bool TryAddSocketGroup(SocketGroup socketGroup)
    {
        // check socket group fits in section

        // at this point we may need to reconfigure the dimensions of the socket group

        _socketGroups.Add(socketGroup);
        return true;
    }
}
