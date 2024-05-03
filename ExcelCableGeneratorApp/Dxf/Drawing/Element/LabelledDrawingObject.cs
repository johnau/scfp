using netDxf;
using netDxf.Collections;
using netDxf.Entities;
using netDxf.Tables;

namespace ExcelCableGeneratorApp.Dxf.Drawing.Element;

internal class LabelledDrawingObject : DrawingObject
{
    protected readonly Label Label;
    private bool _labelVisible;
    public bool LabelVisible => _labelVisible;
    public double HeightWithLabel => Size.Y + Label.Height;
    public LabelledDrawingObject(string nameTag, float margin_Top = 0, float margin_Bottom = 0, float margin_Left = 0, float margin_Right = 0) 
        : base(nameTag, margin_Top, margin_Bottom, margin_Left, margin_Right)
    {
        Label = Label.BlankLabel;
        _labelVisible = false;
    }

    //public void SetLabelProperties(string nameTag, string labelText, float textHeight, float maxWidth = 0f)
    //{
    //    _label = new Label(nameTag, labelText, textHeight, maxWidth);
    //}

    public override void SetPosition(double x, double y, bool centerPoint = false)
    {
        SetPosition(new Vector2(x, y), centerPoint);
    }

    public override void SetPosition(Vector2 position, bool centerPoint = false)
    {
        base.SetPosition(position, centerPoint);

        Label.SetPosition(position.X, position.Y - Size.Y/2 - Label.Height, centerPoint);
    }

    public string GetLabelText()
    {
        return Label.Text;
    }
    public void SetLabelText(string text)
    {
        Label.SetLabelText(text);
    }

    public void SetLabelTextAlignCenter(bool alignedCenter)
    {
        Label.AlignTextCenter = alignedCenter;
    }
    public void SetLabelTextHeight(float textHeight)
    {
        Label.SetTextHeight(textHeight);
    }

    public void SetLabelNameTag(string nameTag)
    {
        Label.SetNameTag(nameTag);
    }

    // Should this method alwasy be considered as relative position to the parent
    // ie. If we set the position here relative to the parent panel (for example)
    // then we will adjust the position with parent position offset
    public void SetLabelPosition(Vector2 position, bool centerPoint = true)
    {
        Label.SetPosition(Position + position, centerPoint);
    }

    public void SetLabelMaxWidth(float maxWidth)
    {
        Label.SetMaxWidth(maxWidth);
    }

    public virtual void SetLabelVisible(bool visible)
    {
        _labelVisible = visible;
    }

    protected override void DrawOutline(DrawingEntities drawing, AciColor color)
    {
        var vertices = GetDwgVertices();

        //if (_labelVisible)
        //{
        //    vertices = VecHelper.TranslateAll(vertices, new Vector2(0, -_label.Height));
        //}

        var polyline = new Polyline2D(vertices, true);
        polyline.Color = color;

        drawing.Add(polyline);
    }

    public void DrawLabel(DrawingEntities drawing)
    {
        var posDxf = VecHelper.FlipYAxis(Label.Position);
        var yShiftText = new Vector2(0, -Label.Height);
        posDxf = posDxf + yShiftText;
        var text = new Text(Label.Text, posDxf, Label.Height);
        var style = new TextStyle("SubHeading", "dim.shx");
        style.WidthFactor = Label.WidthFactor;

        if (Label.IsBold && !Label.IsItalic)
        {
            style.FontStyle = FontStyle.Bold;
        } 
        else if (Label.IsItalic)
        {
            style.FontStyle = FontStyle.Italic;
        }

        text.Alignment = TextAlignment.BaselineCenter;
        text.Style = style;
        drawing.Add(text);
    }
}
