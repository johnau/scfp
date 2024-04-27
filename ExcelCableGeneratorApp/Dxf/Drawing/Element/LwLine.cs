using netDxf;

namespace ExcelCableGeneratorApp.Dxf.Drawing.Element;

internal struct LwLine
{
    public Vector2 Start { get; set; }
    public Vector2 End { get; set; }
    public Vector2 Direction { get => End - Start; }
    public LwLine(Vector2 start, Vector2 end)
    {
        Start = start;
        End = end;
    }
}
