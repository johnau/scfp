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

        public Socket(string id, string name, List<FixingHole> fixings)
            : base(id, name)
        {
            Fixings = fixings;
            Protrusions = [];
        }
    }
}
