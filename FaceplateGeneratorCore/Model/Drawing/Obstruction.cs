namespace FaceplateGeneratorCore.Model.Drawing
{
    /// <summary>
    /// Immutable representation of a faceplate obstruction
    /// </summary>
    /// <remarks>
    /// An obstruction can exist on a plate or on a socket.
    /// Obstructions on sockets will be adopted by the parent
    /// panel.
    /// </remarks>
    public class Obstruction : DrawingObject
    {
        public Obstruction(string id, string name)
            : base(id, name)
        {
        }
    }
}
