using System.Reflection.Emit;

namespace ExcelCableGeneratorApp.Dxf.Drawing.Element;

/// <summary>
/// Represents a group of related sockets.
/// This class does not extend <see cref="LabelledDrawingObject"/> like others that have 
/// a label, as the <see cref="GroupingBracket"/> object provides the label (Assumption is 
/// that there is always a bracket with a label on a Socket Group.
/// </summary>
internal class SocketGroup : DrawingObject
{
    private GroupingBracket? _groupingBracket;
    private bool _groupingBracketVisible; // visiblity could be moved to the GroupingBracket object itself
    private List<Socket> _sockets;
    public List<Socket> Sockets { get => new List<Socket>(_sockets);  }

    public SocketGroup(string nameTag, float margin_Top = 0, float margin_Bottom = 0, float margin_Left = 0, float margin_Right = 0) 
        : base(nameTag, margin_Top, margin_Bottom, margin_Left, margin_Right)
    {
        _groupingBracket = null;
        _groupingBracketVisible = false;
        _sockets = [];
    }

    public bool TryAddSocket(Socket socket)
    {
        // check socket is ok to add

        _sockets.Add(socket);
        return true;
    }

    public bool TryAddSockets(List<Socket> sockets)
    {
        // check sockets are ok to add

        _sockets.AddRange(sockets);
        return true;
    }

    public void SetLabelAndBracketProperties(string labelText, float maxWidth = 0f)
    {
        var nameTag = "Grouping Bracket for SocketGroup";
        _groupingBracket = new GroupingBracket(nameTag);
        
    }

    public void SetLabelAndBracketVisible(bool visible)
    {
        if (visible && _groupingBracket == null)
        {
            throw new Exception("Must setup the Grouping Bracket first, call SetLabelAndBracketProperties() before setting Label and Bracket Visibility to `true`");
        }

        _groupingBracketVisible = visible;
    }

}
