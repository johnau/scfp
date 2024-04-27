namespace ExcelCableGeneratorApp.Dxf.Drawing.Element;

internal class Label : DrawingObject
{
    public static Label BlankLabel => new Label("Label", "", 10.0f);
    /// <summary>
    /// Small Label
    /// </summary>
    /// <param name="nameTag"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static Label SocketLabel(string text) => new Label("Label for Socket", text, 4.0f);
    /// <summary>
    /// Medium Label
    /// </summary>
    /// <param name="nameTag"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static Label SubheaderLabel(string text) => new Label("Label for Group", text, 5.0f);
    /// <summary>
    /// Large Label
    /// </summary>
    /// <param name="nameTag"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static Label HeaderLabel(string text) => new Label("Label for Panel", text, 6.0f);
    public override ElementType Type => ElementType.LABEL;
    public string Text { get; private set; }
    public float Height { get; private set; }
    public float MaxWidth { get; private set; }

    public Label(string nameTag, string text, float textHeight, float margin_Top = 0, float margin_Bottom = 0, float margin_Left = 0, float margin_Right = 0)
    : base(nameTag, margin_Top, margin_Bottom, margin_Left, margin_Right)
    {
        Text = text;
        Height = textHeight;
        MaxWidth = -1.0f;
    }

    public void SetLabelText(string value)
    {
        // calculate text width and compare aginst MaxWidth
        Text = value;
    }

    public void SetTextHeight(float value)
    {
        if (Height < 0.1f) 
            throw new ArgumentException("Height cannot be less than 0.1mm");

        Height = value;
    }

    public void SetMaxWidth(float value)
    {
        MaxWidth = value;
    }
}
