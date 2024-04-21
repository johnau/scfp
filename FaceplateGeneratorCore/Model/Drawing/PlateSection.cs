namespace FaceplateGeneratorCore.Model.Drawing
{
    /// <summary>
    /// Immutable representation of a collection of SocketGroups
    /// </summary>
    /// <remarks>
    /// A section can be placed on any size plate (providing it fits)
    /// </remarks>
    public class PlateSection : DrawingObject
    {
        public PlateSection(string id, string name)
            : base(id, name)
        {
        }
    }
}
