namespace FaceplateGeneratorCore.Model.Drawing
{
    public class TextLabel : DrawingObject
    {
        public string Text { get; }
        public List<string> MoreText { get; }

        public TextLabel(string id, string name, string text, List<string> moreText)
            : base(id, name)
        {
            Text = text;
            MoreText = moreText;
        }
    }
}
