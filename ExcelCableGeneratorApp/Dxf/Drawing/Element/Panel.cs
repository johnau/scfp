using netDxf;
using netDxf.Collections;
using netDxf.Entities;
using System.Diagnostics;

namespace ExcelCableGeneratorApp.Dxf.Drawing.Element;

internal class Panel : LabelledDrawingObject
{
    private Dictionary<int, PanelSection> _sections; // int key for order
    private List<Obstruction> _obstructions;
    private List<Screw> _screws;
    private List<Hole> _holes;

    public override ElementType Type => ElementType.PANEL;

    public float EdgeOffsetTop { get; private set; }
    public float EdgeOffsetBottom { get; private set; }
    public float EdgeOffsetLeft { get; private set; }
    public float EdgeOffsetRight { get; private set; }

    public List<Obstruction> Obstructions { get => new List<Obstruction>(_obstructions); }
    public List<Screw> Screws { get => new List<Screw>(_screws); }
    public List<Hole> Holes { get => new List<Hole>(_holes);  }
    public bool IsFull => _sections.Select(s => !s.Value.IsFull).Any();

    public Panel(string nameTag, float margin_Top = 0, float margin_Bottom = 0, float margin_Left = 0, float margin_Right = 0) 
        : base(nameTag, margin_Top, margin_Bottom, margin_Left, margin_Right)
    {
        _sections = [];
        _obstructions = [];
        _screws = [];
        _holes = [];

        EdgeOffsetTop = 0f;
        EdgeOffsetBottom = 0f;
        EdgeOffsetLeft = 0f;
        EdgeOffsetRight = 0f;
    }

    public bool TryAddSocketGroup(SocketGroup group)
    {
        // loop through sections, find first that is not full
        var minIndex = _sections.Keys.Min();
        var maxIndex = _sections.Keys.Max();

        for (int i = minIndex; i <= maxIndex; i++)
        {
            if (!_sections.TryGetValue(i, out var _section))
                continue;

            if (_section.IsFull && i != maxIndex)
                continue;

            if (_section.IsFull && i == maxIndex)
            {
                Debug.WriteLine("There is no free room on this panel in any section");
                return false;
            }

            if (_section.TryAddSocketGroup(group))
            {
                Debug.WriteLine("Added socket group to section");
            } else
            {
                Debug.WriteLine("Could not add socket group to this section, or this panel");
                return false;
            }
        }

        return false;
    }

    public bool TryAddSection(int order, PanelSection section)
    {
        Debug.WriteLine("Adding section with inadequate check (x-axis only)");

        _sections[order] = section;
        section.Parent = this;

        var minIndex = _sections.Keys.Min();
        var maxIndex = _sections.Keys.Max();

        // position sections
        int lastIndex = minIndex;
        for (int i = minIndex; i <= maxIndex; i++)
        {
            if (!_sections.TryGetValue(i, out var existing))
                continue;

            if (i == minIndex)
            {
                if (EdgeOffsetLeft + section.Size.X > Size.X - EdgeOffsetRight)
                {
                    Debug.WriteLine("Section did not fit on panel");
                    // need to handle checking the next row
                    _sections.Remove(order);
                    return false;
                }
                existing.SetPosition(EdgeOffsetLeft, EdgeOffsetTop);
                continue;
            }

            var lastPos = _sections[lastIndex].Position;
            var lastSize = _sections[lastIndex].Size;
            if (lastPos.X + lastSize.X + section.Size.X > Size.X - EdgeOffsetRight)
            {
                Debug.WriteLine("Section did not fit on panel");
                // need to handle checking the next row
                _sections.Remove(order);
                return false;
            }
            existing.SetPosition(lastPos.X + lastSize.X, lastPos.Y);

            lastIndex = i;
        }

        
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obstruction"></param>
    /// <returns></returns>
    public bool TryAddObstruction(Obstruction obstruction)
    {
        // check against other obstructions
        // re-run checks for all sections, socket groups, sockets, etc

        Debug.WriteLine("Adding obstruction without checking");

        _obstructions.Add(obstruction);
        return true;
    }

    /// <summary>
    /// Add a screw to the panel
    /// </summary>
    /// <remarks>
    /// A hole must be added before a screw can be added - A screw without a hole cannot be added.
    /// </remarks>
    /// <param name="screw"></param>
    /// <returns></returns>
    public bool TryAddScrew(Screw screw)
    {
        // check there is a hole to match the screw

        _screws.Add(screw);
        return true;
    }

    public bool TryAddHole(Hole hole)
    {
        // check against other obstructions

        // check against holes
        Debug.WriteLine($"Adding Hole to Panel without check");

        // !!! ADD PANEL OFFSET TO HOLE

        _holes.Add(hole);
        return true;
    }

    public void SetEdgeOffset(float value)
    {
        if (value < 0)
            throw new ArgumentException("Edge offset must be positive");
        EdgeOffsetTop = value;
        EdgeOffsetBottom = value;
        EdgeOffsetLeft = value;
        EdgeOffsetRight = value;
    }

    public void SetEdgeOffsetTop(float value)
    {
        if (value < 0)
            throw new ArgumentException("Edge offset must be positive");
        EdgeOffsetTop = value;
    }

    public void SetEdgeOffsetBottom(float value)
    {
        if (value < 0)
            throw new ArgumentException("Edge offset must be positive");
        EdgeOffsetBottom = value;
    }

    public void SetEdgeOffsetLeft(float value)
    {
        if (value < 0)
            throw new ArgumentException("Edge offset must be positive");
        EdgeOffsetLeft = value;
    }

    public void SetEdgeOffsetRight(float value)
    {
        if (value < 0)
            throw new ArgumentException("Edge offset must be positive");
        EdgeOffsetRight = value;
    }

    public override bool Draw(DrawingEntities drawing)
    {
        DrawOutline(drawing, AciColor.FromHsl(0, 0, 0));
        DrawPanelEdgeOffsets(drawing);
        DrawHoles(drawing);
        DrawObstructions(drawing);
        DrawSections(drawing); // sections draw groups, groups draw sockets

        return true;
    }

    private void DrawPanelEdgeOffsets(DrawingEntities drawing)
    {
        List<Vector2> vertices = [
            new Vector2(0 + EdgeOffsetLeft, -EdgeOffsetBottom),   // bottom left
            new Vector2(0 + EdgeOffsetLeft, -Size.Y + EdgeOffsetTop),   // top left
            new Vector2(Size.X - EdgeOffsetRight, -Size.Y + EdgeOffsetTop),   // top right
            new Vector2(Size.X - EdgeOffsetRight, -EdgeOffsetBottom)    // bottom right
        ];

        var polyline = new Polyline2D(vertices, true);
        polyline.Color = AciColor.Yellow;

        drawing.Add(polyline);
    }

    private void DrawObstructions(DrawingEntities drawing)
    {
        foreach (var obstruction in _obstructions)
        {
            obstruction.Draw(drawing);
        }
    }

    private void DrawHoles(DrawingEntities drawing)
    {
        foreach (var hole in _holes)
        {
            hole.Draw(drawing);
        }
    }

    private void DrawSections(DrawingEntities drawing)
    {
        foreach (var section in _sections)
        {
            var sectionDwg = section.Value;
            sectionDwg.Draw(drawing);
        }
    }

}
