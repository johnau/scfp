using netDxf;
using netDxf.Collections;
using netDxf.Entities;

namespace ExcelCableGeneratorApp.Dxf.Drawing.Element;

/// <summary>
/// Represents a hole in the drawing (cut through)
/// A hole by default is rectangular, a square hole can be specified with the MakeSquare() func, and a circle with MakeCircle()
/// An oval can be made with MakeHorizontalRoundedSlot() or MakeVerticalRoundedSlot(), depending which side should be a half circle.
/// Rounded corner rectangles and squares of other specifications can be created with the SetCornerFilletRadius() method.
/// </summary>
internal class Hole : DrawingObject
{
    public static Hole FixingHole_M3 => new Hole("M3 Fixing Hole", 3.1f).MakeCircular();
    public static Hole FixingHole_M4 => new Hole("M4 Fixing Hole", 4.1f).MakeCircular();

    private double _cornerFilletRadius;
    public override ElementType Type => ElementType.HOLE;

    public Hole(string nameTag, double width, double height = 0, float margin_Top = 0, float margin_Bottom = 0, float margin_Left = 0, float margin_Right = 0) 
        : base(nameTag, margin_Top, margin_Bottom, margin_Left, margin_Right)
    {
        _cornerFilletRadius = 0f;
        if (height == 0f)
            SetSize(width, width);
        else
            SetSize(width, height);
    }

    public Hole SetCornerFilletRadius(double radius)
    {
        if (radius < 0f) throw new ArgumentException("Radius cannot be less than zero");
        _cornerFilletRadius = radius;
        return this;
    }

    public Hole MakeHorizontalRoundedSlot()
    {
        _cornerFilletRadius = Size.Y / 2;
        return this;
    }

    public Hole MakeVerticalRoundedSlot()
    {
        _cornerFilletRadius = Size.X / 2;
        return this;
    }

    /// <summary>
    /// Discards Size.Y (Height) and use Size.X (Width) for width and height
    /// </summary>
    public Hole MakeSquare()
    {
        SetSize(Size.X, Size.X);
        return this;
    }

    /// <summary>
    /// Circle diameter based on X dimension of Size
    /// </summary>
    public Hole MakeCircular()
    {
        MakeSquare();
        MakeVerticalRoundedSlot();
        return this;
    }

    public override bool Draw(DrawingEntities drawing)
    {
        DrawOutline(drawing, AciColor.Magenta);
        return true;
    }

    protected override void DrawOutline(DrawingEntities drawing, AciColor color)
    {
        if (_cornerFilletRadius == Size.X / 2)
        {
            var centerForDxf = new Vector2(Center.X, -Center.Y);
            Circle circle = new Circle(centerForDxf, Size.X / 2);
            circle.Color = AciColor.Magenta;
            drawing.Add(circle);
        }

        base.DrawOutline(drawing, color);


    }
}
