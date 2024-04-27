
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using netDxf;
using netDxf.Collections;
using netDxf.Entities;
using System.Diagnostics;

namespace ExcelCableGeneratorApp.Dxf.Drawing.Element;

/// <summary>
/// Represents a group of related sockets.
/// This class does not extend <see cref="LabelledDrawingObject"/> like others that have 
/// a label, as the <see cref="GroupingBracket"/> object provides the label (Assumption is 
/// that there is always a bracket with a label on a Socket Group.
/// </summary>
internal class SocketGroup : DrawingObject
{
    private GroupingBracket _groupingBracket;
    private bool _groupingBracketVisible; // visiblity could be moved to the GroupingBracket object itself
    private List<Socket> _sockets;
    /// <summary>
    /// Local Obstructions can be Obstruction, Hole, etc
    /// DUring collision detection, they are added here for later repositioning
    /// </summary>
    private List<DrawingObject> _localObstructions; // obstructions that affects this group, used to affect socket placement and therefor group size
    public override ElementType Type => ElementType.SOCKET_GROUP;
    public List<Socket> Sockets { get => new List<Socket>(_sockets);  }

    public SocketGroup(string nameTag, float margin_Top = 0, float margin_Bottom = 0, float margin_Left = 0, float margin_Right = 0) 
        : base(nameTag, margin_Top, margin_Bottom, margin_Left, margin_Right)
    {
        _groupingBracket = new GroupingBracket("Grouping Bracket for Socket Group");
        _groupingBracketVisible = false;
        _sockets = [];
        _localObstructions = [];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// A group does not have any constraints on size (unless maxwidth set).  It can flex to all possible configrations
    /// A group will try to maintain in a single row if possible... ??? lay out rules need to be clearly defined, is fitting more on one panel more important than having single rows? not sure
    /// </remarks>
    /// <param name="socket"></param>
    /// <returns></returns>
    public bool TryAddSocket(Socket socket)
    {
        //// check socket is ok to add

        //if (_sockets.Count == 0)
        //{
        //    socket.SetPositionRelative(0, 0);
        //    _sockets.Add(socket);
        //    return true;
        //}

        // here we would need to factor in maxwidth which has not be implemented yet
        // Sockets always have a label, se we use the HeightWithLabel property instead of Size.X
        var groupHeight = Size.Y > socket.HeightWithLabel ? Size.Y : socket.HeightWithLabel;
        socket.SetPosition(new Vector2(Position.X + Size.X, Position.Y));

        // this will change with maxwidth, as we won't always just add on to Size.X
        // max width will come from the parent Section
        var newGroupSize = new Vector2(Size.X + socket.Size.X, groupHeight);
        
        SetSize(newGroupSize);

        _sockets.Add(socket);

        Debug.WriteLine($"Added Socket to Group, Group now {newGroupSize}");

        return true;
    }

    /// <summary>
    /// Repositions children based on eachother and on local obstructions
    /// </summary>
    /// <remarks>
    /// It is likely that this method will be one of the first targets for optimization, 
    /// as recalculating the positions every time is not required
    /// </remarks>
    protected override void RepositionChildren()
    {
        // lay out sockets horizontally in Group by default
        var lastPos = Position;
        for (int i = 0; i < _sockets.Count; i++)
        {
            var socket = _sockets[i];
            Vector2 pos;
            if (i == 0)
                pos = new Vector2(lastPos.X, lastPos.Y);
            else
                pos = new Vector2(lastPos.X + socket.Size.X, lastPos.Y);

            socket.SetPosition(pos);
            lastPos = pos;
        }


        // loop through any obstructions and shift children over.
        foreach (var x in _localObstructions)
        {
            var obstructionVerts = x.GetDwgVertices();
            for (int i = 0; i < _sockets.Count - 1; i++)
            {
                var socket = _sockets[i];
                var socketVerts = socket.GetDwgVertices();
                if (!VecHelper.CalculateMinTranslationVectorX(obstructionVerts, socketVerts, out var translate))
                    continue;

                // apply translate to all the vertices of the group
                //var newGroupVerts = VecHelper.TranslateAll(groupVerts, translate);
                
                socket.SetPosition(socket.Position + translate);

                //// position other children
                for (int i2 = i + 1; i2 < _sockets.Count; i2++)
                {
                    var _s = _sockets[i2];
                    _s.SetPosition(_s.Position + translate);

                    if (i2 == _sockets.Count - 1)
                    {
                        var end = _s.Position + translate + new Vector2(_s.Size.X, 0);
                        SetSize(end.X, Size.Y); // resize the group...
                    }
                }

                Debug.WriteLine($"This obstruction caused a socket and all subsequent sockets to move @ {x.Position}");
            }
        }
    }

    public bool TryAddSockets(List<Socket> sockets)
    {
        // check sockets are ok to add
        List<Socket> failed = [];
        foreach (var socket in sockets)
        {
            if (!TryAddSocket(socket))
            {
                failed.Add(socket);
            }
        }

        if (failed.Count > 0)
        {
            throw new Exception("Intention is to have group completely flexible, will not reject a socket addition.  There should never be a group that is too large to fit on a panel... right?");
        }

        return true;
    }

    public string GetBracketLabelText()
    {
        return _groupingBracket.GetLabelText();
    }

    public void SetLabelAndBracketProperties(string labelText, float maxWidth = 0f)
    {
        _groupingBracket.SetLabelText(labelText);
        _groupingBracket.SetLabelMaxWidth(maxWidth);
    }

    public void SetLabelAndBracketVisible(bool visible)
    {
        if (visible && _groupingBracket == null)
        {
            throw new Exception("Must setup the Grouping Bracket first, call SetLabelAndBracketProperties() before setting Label and Bracket Visibility to `true`");
        }

        _groupingBracketVisible = visible;
    }

    public override bool Draw(DrawingEntities drawing)
    {
        DrawOutline(drawing, AciColor.LightGray);

        foreach (var socket in _sockets)
        {
            socket.Draw(drawing);
        }

        return true;
    }

    //private void DrawGroupOutline(DrawingEntities drawing)
    //{
    //    var topLeft = Position;
    //    var topRight = new Vector2(Position.X + Size.X, -Position.Y);
    //    var bottomRight = new Vector2(Position.X + Size.X, -Position.Y - Size.Y);
    //    var bottomLeft = new Vector2(Position.X, -Position.Y - Size.Y);
        
    //    List<Vector2> vertices = [
    //        topLeft,
    //        topRight,
    //        bottomRight,
    //        bottomLeft
    //    ];

    //    var polyline = new Polyline2D(vertices, true);
    //    polyline.Color = AciColor.FromHsl(0, 0, 0);

    //    drawing.Add(polyline);
    //}

    internal bool CheckCollisions(Dictionary<DrawingObject, List<LwLine>> obstructions)
    {
        // check socket group fits in section
        // generate list of lines for this group
        var points = GetDwgVertices();
        // loop each vertice of obstruction and create 
        List<LwLine> lines = [];
        for (int i = 0; i < points.Count; i++)
        {
            var current = points[i];
            var nextIdx = (i + 1) % points.Count;
            var next = points[nextIdx];
            var line = new LwLine(current, next);
            lines.Add(line);
        }

        List<DrawingObject> collidedObjects = [];

        foreach (var o in obstructions)
        {
            var obstruction = o.Key;
            var oLines = o.Value;
            bool hasIntersection = VecHelper.CheckForIntersection(lines, oLines);
            if (hasIntersection)
            {
                Debug.WriteLine($"Found intersection between socket group and obstruction");
                collidedObjects.Add(obstruction);
            }
        }
        
        foreach (var co in collidedObjects)
        {
            if (!_localObstructions.Contains(co))
                _localObstructions.Add(co);
        }

        // re arrange elements based on collisions.
        RepositionChildren();

        return collidedObjects.Count > 0;
    }


}
