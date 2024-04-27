namespace ExcelCableGeneratorApp.Dxf.Drawing.Element;

internal class Screw : DrawingObject
{
    public override ElementType Type => ElementType.SCREW;
    public double HeadDiameter { get => Size.X == Size.Y ? Size.X : throw new ArgumentException("Size X and Y are not equal but should be."); }
    public ScrewHeadType ScrewHead;

    public Screw(string nameTag, float margin_Top = 0, float margin_Bottom = 0, float margin_Left = 0, float margin_Right = 0) 
        : base(nameTag, margin_Top, margin_Bottom, margin_Left, margin_Right)
    {
        ScrewHead = ScrewHeadType.PHILLIPS;
    }
}
