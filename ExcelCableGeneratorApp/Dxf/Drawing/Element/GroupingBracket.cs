using netDxf;
using netDxf.Collections;
using netDxf.Entities;

namespace ExcelCableGeneratorApp.Dxf.Drawing.Element;

internal class GroupingBracket : LabelledDrawingObject
{
    public override ElementType Type => ElementType.GROUPING_BRACKET;

    //public double LabelVerticalDistanceFromBracket { get; private set; }

    public GroupingBracket(string nameTag, float margin_Top = 0, float margin_Bottom = 0, float margin_Left = 0, float margin_Right = 0) 
        : base(nameTag, margin_Top, margin_Bottom, margin_Left, margin_Right)
    {
        SetLabelNameTag("Label for Group");
        //LabelVerticalDistanceFromBracket = 1.0d;
    }

    public void SetupLabel(string labelText, float textHeight)
    {
        SetLabelText(labelText);
        SetLabelTextHeight(textHeight);
        SetLabelVisible(true);
    }

    public override bool Draw(DrawingEntities drawing)
    {
        Label.WidthFactor = 0.85;
        DrawLabel(drawing);

        DrawOutline(drawing, AciColor.DarkGray);

        var verts = GetDwgVertices();
        var topLeft = verts[0];
        var topRight = verts[1];
        var bottomRight = verts[2];
        var bottomLeft = verts[3];

        var polyline = new Polyline2D([bottomLeft, topLeft, topRight, bottomRight], false);
        polyline.Color = AciColor.Cyan;

        drawing.Add(polyline);

        return true;
    }
}
