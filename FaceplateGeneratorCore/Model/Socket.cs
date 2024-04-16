namespace FaceplateGeneratorCore.Model
{
    /// <summary>
    /// Immutable representation of a socket that is typically
    /// part of an <see cref="ItemGroup">
    /// </summary>
    /// <remarks>
    /// Sockets can include XLR, DMX, Ethernet, GPO, etc
    /// </remarks>
    public class Socket : DrawingObject
    {
        public List<FixingHole> Fixings { get; }
        public List<Obstruction> Protrusions { get; }

        public Socket(string id, string h_Id, List<FixingHole> fixings)
        {
            Id = id;
            H_Id = h_Id;
            MarginLeft = 0f;
            MarginRight = 0f;
            MarginTop = 0f;
            MarginBottom = 0f;
            Fixings = [];
            Protrusions = [];
        }
    }
}
