namespace ExcelCableGeneratorApp.Dxf.Drawing.Element;

internal class GroupingBracket : LabelledDrawingObject
{
    public GroupingBracket(string nameTag, float margin_Top = 0, float margin_Bottom = 0, float margin_Left = 0, float margin_Right = 0) 
        : base(nameTag, margin_Top, margin_Bottom, margin_Left, margin_Right)
    {
        SetLabelNameTag("Label for Group");
    }

    public void SetupLabel(string labelText, float textHeight)
    {
        SetLabelText(labelText);
        SetLabelTextHeight(textHeight);
        SetLabelVisible(true);
    }
}
