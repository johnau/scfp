namespace FaceplateGeneratorCore.Model.Drawing;

/// <summary>
/// Immutable representation of a socket
/// </summary>
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
