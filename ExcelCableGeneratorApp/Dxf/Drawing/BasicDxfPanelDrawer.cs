using ExcelCableGeneratorApp.Dxf.Aggregates;
using ExcelCableGeneratorApp.Dxf.Drawing.Element;
using ExcelCableGeneratorApp.Utility;
using netDxf;
namespace ExcelCableGeneratorApp.Dxf.Drawing;

internal class BasicDxfPanelDrawer : IDxfPanelDrawer
{
    private Dictionary<string, PanelData> _panelSpecs;
    private List<Panel> _panelsCreated;

    public BasicDxfPanelDrawer()
    {
        _panelSpecs = [];
        _panelsCreated = [];

        _panelSpecs["3U_FullWidth"] = PanelData.Standard_UHeight_FullWidth(3);
    }

    public void AddPanelSpec(string name, PanelData panelData)
    {
        _panelSpecs.Add(name , panelData);
        var panel = new Panel("Rack Panel");
        panel.SetSize(panelData.PanelWidthInMm, panelData.PanelHeightInMm);
        _panelsCreated.Add(panel);
    }

    public string DrawPanels(List<SocketGroup> socketGroups)
    {
        DxfDocument doc = new DxfDocument();
        //var entities = doc.Entities;
        var nowMillis = UnixTimestamp();
        var filePath = $"./test_out_{nowMillis}.dxf";

        foreach (var group in socketGroups)
        {
            //CreateSocketGroup(group);
        }

        //doc.Entities.Add(entity);
        
        doc.Save(filePath);

        return filePath;
    }

    public List<PanelSection> MapGroupsToPanels(List<SocketGroup> groupsToMap)
    {
        foreach (var group in groupsToMap)
        {
            Panel? nextAvailablePanel = null;
            foreach (var panel in _panelsCreated)
            {
                if (panel.IsFull) 
                    continue;
                nextAvailablePanel = panel;
                break;
            }
            if (nextAvailablePanel == null)
            {
                // create new panel and use
                nextAvailablePanel = BuildPanel(0);
            }

            nextAvailablePanel.TryAddSocketGroup(group);

        }

        return [];
    }

    /// <summary>
    /// Mapping PanelData to panel, move the method to a convert/dto mapper object
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public Panel BuildPanel(int type)
    {
        Panel p;
        if (type == 0) // default panel: 3U full width
        {
            var panelData = _panelSpecs["3U_FullWidth"];

            p = new Panel("3U Full Width");
            p.SetEdgeOffset(panelData.PanelEdgeOffsetMm);
            p.SetSize(panelData.PanelWidthInMm, panelData.PanelHeightInMm);
            foreach (var hole in panelData.FixingHoles)
            {
                var position = hole.Item1;
                var radius = hole.Item2;
                var shape = hole.Item3;
                var _hole = new Hole($"M{((int)radius)} Fixing Hole", radius*2);
                _hole.SetPosition(position.ToDxfVector2(), true);
                if (shape == HoleShape.CIRCLE)
                    _hole.MakeCircular();

                _ = p.TryAddHole(_hole);
            }
            foreach (var obstruction in panelData.Obstructions)
            {
                var boundTopLeft = obstruction.Item1;
                var boundBottomRight = obstruction.Item2;
                var z = obstruction.Item3;
                var rot = obstruction.Item4;

                var zPosition = z == 0 ? Obstruction.ObstructionPosition.BACK : Obstruction.ObstructionPosition.FRONT;
                var _obstruction = new Obstruction("Manifold Obstruction", zPosition);
                var width = boundBottomRight.X - boundTopLeft.X;
                var height = boundBottomRight.Y - boundTopLeft.Y;
                _obstruction.SetSize(width, height);
                _obstruction.SetPosition(boundTopLeft.ToDxfVector2());
                _obstruction.SetRotation(rot);

                _ = p.TryAddObstruction(_obstruction);
            }
            foreach (var section in panelData.Sections)
            {
                var size = section.Value.Size;
                var position = section.Value.Position;

                var _section = new PanelSection("Panel Section " + section.Key);
                _section.SetSize(size.ToDxfVector2());
                _section.SetPosition(position.ToDxfVector2());

                _ = p.TryAddSection(section.Key, _section);
            }
        } 
        else
        {
            p = new Panel("Not implemented");
        }

        return p;
    }

    /// <summary>
    /// Convert SocketGroupData to SocketGroup : DrawingObject with Socket : DrawingObjects
    /// This is a flexible element that can be placed inside fixed width element: Section.
    /// A group must be kept together? (if possible)
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    public SocketGroup CreateSocketGroup(SocketGroupData group)
    {
        var cleanGroupName = StringHelper.StripAllNonAlphanumericChars(group.SourceId);
        var socketGroup = new SocketGroup($"Socket group for {cleanGroupName}");
        socketGroup.SetLabelAndBracketProperties(group.SourceId);

        foreach (var socketData in group.Sockets)
        {
            socketData.Deconstruct(out var id, out var type);

            var socket = new Socket($"{type} Socket", type);
            socket.SetLabelText(id);

            socketGroup.TryAddSocket(socket);
        }

        return socketGroup;
    }

    /// <summary>
    /// Produces a Unix style timestamp of current datetime.
    /// </summary>
    /// <returns></returns>
    private double UnixTimestamp()
    {
        return DateTime.Now.ToUniversalTime().Subtract(
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            ).TotalMilliseconds;
    }

}
