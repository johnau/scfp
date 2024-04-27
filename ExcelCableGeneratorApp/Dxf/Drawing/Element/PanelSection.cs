using netDxf;
using netDxf.Collections;
using System.Diagnostics;

namespace ExcelCableGeneratorApp.Dxf.Drawing.Element;

internal class PanelSection : LabelledDrawingObject
{
    private List<SocketGroup> _socketGroups;
    private bool _isFull;

    public override ElementType Type => ElementType.PANEL_SECTION;
    public List<SocketGroup> SocketGroups { get => new List<SocketGroup>(_socketGroups); }

    public bool IsFull => _isFull;

    public PanelSection(string nameTag, float margin_Top = 0, float margin_Bottom = 0, float margin_Left = 0, float margin_Right = 0) 
        : base(nameTag, margin_Top, margin_Bottom, margin_Left, margin_Right)
    {
        _socketGroups = [];
        _isFull = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// If a group has contact with an obstruction, that obstruction is added to the group.
    /// </remarks>
    /// <param name="socketGroup"></param>
    /// <returns></returns>
    public bool TryAddSocketGroup(SocketGroup socketGroup)
    {
        Dictionary<DrawingObject, List<LwLine>> obstructions = UpdateObstructionsFromParent()
    .ToDictionary(t => (DrawingObject)t.Key, t => t.Value);

        // at this point we may need to reconfigure the dimensions of the socket group
        if (_socketGroups.Count == 0)
        {
            if (socketGroup.Size.X > Size.X || socketGroup.Size.Y > Size.Y)
            {
                Debug.WriteLine("This group is too big to fit in this section");
                return false;
            }

            socketGroup.SetPosition(Position.X, Position.Y);
            var hasCollisions = socketGroup.CheckCollisions(obstructions);
            if (hasCollisions)
            {
                Debug.WriteLine($"Collision detected while trying to add SocketGroup");
                
            }
            _socketGroups.Add(socketGroup);
            socketGroup.Parent = this;
            return true;
        }

        var lastGroup = _socketGroups[^1];
        var lgPos = lastGroup.Position;
        var lgSize = lastGroup.Size;
        if (lgPos.X + lgSize.X + socketGroup.Size.X <= Position.X + Size.X)
        {
            // it fits on this row in this section
            var newPosition = new Vector2(lgPos.X + lgSize.X, lgPos.Y);
            socketGroup.SetPosition(lgPos.X + lgSize.X, lgPos.Y);
            //var collisionList = CheckCollisions(socketGroup);

            var hasCollisions = socketGroup.CheckCollisions(obstructions);
            
            if (hasCollisions) {
                Debug.WriteLine($"Handle re-arrange group for obstruction");
            }

            _socketGroups.Add(socketGroup);
            socketGroup.Parent = this;
            return true;
        }
        else
        {
            var bottomOfLastRow = _socketGroups.Select(group => group.Position.Y + group.Size.Y)
                                    .Max();

            // check to see if can fit on another row
            if (bottomOfLastRow + socketGroup.Size.Y > Position.Y + Size.Y)
            {
                // can try to re-arrange the group here, but really we just need to try every combination and rank
                Debug.WriteLine("There is not enough space in this section for this group...");
                return false;
            }

            // it fits on the next row
            socketGroup.SetPosition(Position.X, bottomOfLastRow);
            _socketGroups.Add(socketGroup);
            socketGroup.Parent = this;
            return true;
        }
    }

    //private List<DrawingObject> CheckCollisions(SocketGroup socketGroup)
    //{
    //    var obstructions = UpdateObstructionsFromParent();

    //    // check socket group fits in section
    //    // generate list of lines for this group
    //    var points = socketGroup.GetDwgVertices();
    //    // loop each vertice of obstruction and create 
    //    List<(Vector2, Vector2)> lines = [];
    //    for (int i = 0; i < points.Count; i++)
    //    {
    //        var point = points[i];
    //        var prevIdx = i - 1 == -1 ? points.Count - 1 : i - 1;
    //        var previous = points[prevIdx];
    //        lines.Add((point, previous));
    //    }

    //    List<DrawingObject> collidedObjects = [];

    //    foreach (var o in obstructions)
    //    {
    //        var obstruction = o.Key;
    //        var oLines = o.Value;
    //        bool hasIntersection = VecHelper.CheckForIntersection(lines, oLines);
    //        if (hasIntersection)
    //        {
    //            Debug.WriteLine($"Found intersection between socket group and obstruction");
    //            collidedObjects.Add(obstruction);
    //        }
    //    }
    //    return collidedObjects;
    //}


    private Dictionary<Obstruction, List<LwLine>> UpdateObstructionsFromParent()
    {
        Dictionary<Obstruction, List<LwLine>> obstructionLines = [];
        if (Parent! != null && Parent.Type == ElementType.PANEL)
        {
            var panel = (Panel)Parent;

            // loop each obstruction
            foreach (var obstruction in panel.Obstructions)
            {
                var points = obstruction.GetDwgVertices();
                // loop each vertice of obstruction and create 
                List<LwLine> lines = [];
                for (int i = 0; i < points.Count; i++)
                {
                    var current = points[i];
                    var nextIdx = (i + 1) % points.Count;
                    var next = points[nextIdx];

                    LwLine line = new(current, next);
                    lines.Add(line);
                }

                obstructionLines.TryAdd(obstruction, lines);
            }
        }

        return obstructionLines;
    }

    public override bool Draw(DrawingEntities drawing)
    {
        DrawOutline(drawing, AciColor.Blue);

        foreach (var group in _socketGroups)
        {
            group.Draw(drawing);
        }

        return true;
    }
}
