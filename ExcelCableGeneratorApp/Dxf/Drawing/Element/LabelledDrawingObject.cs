namespace ExcelCableGeneratorApp.Dxf.Drawing.Element;

internal class LabelledDrawingObject : DrawingObject
{
    protected readonly Label _label;
    private bool _labelVisible;

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

    public void SetLabelVisible(bool visible)
    {
        _labelVisible = visible;
    }
}
