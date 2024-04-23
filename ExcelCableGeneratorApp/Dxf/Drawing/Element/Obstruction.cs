namespace ExcelCableGeneratorApp.Dxf.Drawing.Element;

internal class Obstruction : DrawingObject
{
    public enum ObstructionPosition
    { 
        BACK, 
        FRONT
    }

    public ObstructionPosition _thirdDimensionPosition { get; }

    public Obstruction(string nameTag, ObstructionPosition depthPosition, float margin_Top = 0, float margin_Bottom = 0, float margin_Left = 0, float margin_Right = 0) 
        : base(nameTag, margin_Top, margin_Bottom, margin_Left, margin_Right)
    {
        _thirdDimensionPosition = depthPosition;
    }
}
