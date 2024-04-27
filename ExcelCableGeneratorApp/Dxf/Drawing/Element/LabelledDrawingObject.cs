using netDxf;
using netDxf.Collections;
using netDxf.Entities;

namespace ExcelCableGeneratorApp.Dxf.Drawing.Element;

internal class LabelledDrawingObject : DrawingObject
{
    protected readonly Label _label;
    private bool _labelVisible;
    public bool LabelVisible => _labelVisible;
    public double HeightWithLabel => Size.Y + _label.Height;
    public LabelledDrawingObject(string nameTag, float margin_Top = 0, float margin_Bottom = 0, float margin_Left = 0, float margin_Right = 0) 
        : base(nameTag, margin_Top, margin_Bottom, margin_Left, margin_Right)
    {
        _label = Label.BlankLabel;
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

        _label.SetPosition(position.X, position.Y, centerPoint);
    }

    public string GetLabelText()
    {
        return _label.Text;
    }
    public void SetLabelText(string text)
    {
        _label.SetLabelText(text);
    }

    public void SetLabelTextHeight(float textHeight)
    {
        _label.SetTextHeight(textHeight);
    }

    public void SetLabelNameTag(string nameTag)
    {
        _label.SetNameTag(nameTag);
    }

    public void SetLabelMaxWidth(float maxWidth)
    {
        _label.SetMaxWidth(maxWidth);
    }

    public virtual void SetLabelVisible(bool visible)
    {
        _labelVisible = visible;
    }

    protected override void DrawOutline(DrawingEntities drawing, AciColor color)
    {
        var vertices = GetDwgVertices();

        if (_labelVisible)
        {
            vertices = VecHelper.TranslateAll(vertices, new Vector2(0, -_label.Height));
        }

        var polyline = new Polyline2D(vertices, true);
        polyline.Color = color;

        drawing.Add(polyline);
    }

    public void DrawLabel(DrawingEntities drawing)
    {
        var posDxf = VecHelper.FlipYAxis(_label.Position);
        var yShiftText = new Vector2(0, -_label.Height);
        posDxf = posDxf + yShiftText;
        Text text = new Text(_label.Text, posDxf, _label.Height);
        drawing.Add(text);
    }
}
