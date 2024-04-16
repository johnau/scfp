namespace FaceplateGeneratorCore.Model
{
    public class GroupingBracket : DrawingObject
    {
        public string LabelText { get; }

        public GroupingBracket(string id, string name, string labelText)
            : base(id, name)
        {
            LabelText = labelText;
        }
    }
}
