namespace FaceplateGeneratorCore.Model
{
    public class DrawingObject
    {
        public string Id { get; }
        public string Name { get; }
        public float MarginLeft { get; }
        public float MarginRight { get; }
        public float MarginTop { get; }
        public float MarginBottom { get; }

        public DrawingObject(string id, string name)
        {
            Id = id;
            Name = name;
            MarginLeft = 0;
            MarginRight = 0;
            MarginTop = 0;
            MarginBottom = 0;
        }
    }
}
