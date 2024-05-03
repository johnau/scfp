using ExcelCableGeneratorApp.Dxf.Drawing.Element;
using netDxf;

namespace ExcelCableGeneratorApp.Dxf.Drawing.Factory;
/// <summary>
/// Creates instances of Panel objects (<see cref="FixedGridPanel"/>, <see cref="Panel"/>)
/// </summary>
internal class PanelFactory
{
    public const double TechPanelHeight = 128.0d;
    public const double TechPanelFirstRow = 48.5;
    public const double TechPanelSecondRow = 98.5;

    public PanelFactory()
    {
    }

    public static FixedGridPanel BuildTechPanel_1Gang()
    {
        var panel = new FixedGridPanel("Tech Panel - 1 Gang");
        var panelWidth = 85.0d;
        panel.SetSize(panelWidth, TechPanelHeight);

        panel.SetLabelText("TP 1 GANG");
        panel.SetLabelTextHeight(StandardTextHeight.Large);
        panel.SetLabelPosition(new Vector2(panelWidth/2, 10.0d), true);
        panel.SetLabelTextAlignCenter(true);
        panel.CornerFilletRadius = 2.0d;

        var sockets = new List<Vector2>()
        {
            new Vector2(25.5, TechPanelFirstRow),
            new Vector2(57.5, TechPanelFirstRow),
            new Vector2(25.5, TechPanelSecondRow),
            new Vector2(57.5, TechPanelSecondRow)
        };

        var fh1 = Hole.FixingHole_M3;
        fh1.SetPosition(11.0, 7.0, true);
        var fh2 = Hole.FixingHole_M3;
        fh2.SetPosition(74.0, 7.0, true);
        var fh3 = Hole.FixingHole_M3;
        fh3.SetPosition(11.0, 121.0, true);
        var fh4 = Hole.FixingHole_M3;
        fh4.SetPosition(74.0, 121.0, true);

        var fixingHoles = new List<Hole>() { fh1, fh2, fh3, fh4 };

        panel.SetSocketPoints(sockets);
        panel.SetFixingHoles(fixingHoles);

        return panel;
    }

    public static FixedGridPanel BuildTechPanel_2Gang()
    {
        var panel = new FixedGridPanel("Tech Panel - 2 Gang");
        var panelWidth = 170.0d;
        panel.SetSize(panelWidth, TechPanelHeight);

        panel.SetLabelText("TP 2 GANG");
        panel.SetLabelTextHeight(StandardTextHeight.Large);
        panel.SetLabelPosition(new Vector2(panelWidth / 2, 10.0d), true);
        panel.SetLabelTextAlignCenter(true);
        panel.CornerFilletRadius = 2.0d;

        var sockets = new List<Vector2>()
        {
            new Vector2(21.0, TechPanelFirstRow),
            new Vector2(53.0, TechPanelFirstRow),
            new Vector2(85.0, TechPanelFirstRow),
            new Vector2(117.0, TechPanelFirstRow),
            new Vector2(149.0, TechPanelFirstRow),
            new Vector2(21.0, TechPanelSecondRow),
            new Vector2(53.0, TechPanelSecondRow),
            new Vector2(85.0, TechPanelSecondRow),
            new Vector2(117.0, TechPanelSecondRow),
            new Vector2(149.0, TechPanelSecondRow)
        };

        var fh1 = Hole.FixingHole_M3;
        fh1.SetPosition(11.0, 7.0, true);
        var fh2 = Hole.FixingHole_M3;
        fh2.SetPosition(159.0, 7.0, true);
        var fh3 = Hole.FixingHole_M3;
        fh3.SetPosition(11.0, 121.0, true);
        var fh4 = Hole.FixingHole_M3;
        fh4.SetPosition(159.0, 121.0, true);

        var fixingHoles = new List<Hole>() { fh1, fh2, fh3, fh4 };

        panel.SetSocketPoints(sockets);
        panel.SetFixingHoles(fixingHoles);

        return panel;
    }

    public static FixedGridPanel BuildTechPanel_3Gang()
    {
        var panel = new FixedGridPanel("Tech Panel - 3 Gang");
        var panelWidth = 255.0d;
        panel.SetSize(panelWidth, TechPanelHeight);

        panel.SetLabelText("TP 3 GANG");
        panel.SetLabelTextHeight(StandardTextHeight.Large);
        panel.SetLabelPosition(new Vector2(panelWidth / 2, 10.0d), true);
        panel.SetLabelTextAlignCenter(true);
        panel.CornerFilletRadius = 2.0d;

        var sockets = new List<Vector2>()
        {
            new Vector2(15.5, TechPanelFirstRow),
            new Vector2(47.5, TechPanelFirstRow),
            new Vector2(79.5, TechPanelFirstRow),
            new Vector2(111.5, TechPanelFirstRow),
            new Vector2(143.5, TechPanelFirstRow),
            new Vector2(175.5, TechPanelFirstRow),
            new Vector2(207.5, TechPanelFirstRow),
            new Vector2(239.5, TechPanelFirstRow),

            new Vector2(15.5, TechPanelSecondRow),
            new Vector2(47.5, TechPanelSecondRow),
            new Vector2(79.5, TechPanelSecondRow),
            new Vector2(111.5, TechPanelSecondRow),
            new Vector2(143.5, TechPanelSecondRow),
            new Vector2(175.5, TechPanelSecondRow),
            new Vector2(207.5, TechPanelSecondRow),
            new Vector2(239.5, TechPanelSecondRow),
        };

        // top row
        var fh1 = Hole.FixingHole_M3;
        fh1.SetPosition(11.0, 7.0, true);
        var fh2 = Hole.FixingHole_M3;
        fh2.SetPosition(74.0, 7.0, true);        
        var fh3 = Hole.FixingHole_M3;
        fh3.SetPosition(181.0, 7.0, true);
        var fh4 = Hole.FixingHole_M3;
        fh4.SetPosition(244.0, 7.0, true);
        // bottom row
        var fh5 = Hole.FixingHole_M3;
        fh5.SetPosition(11.0, 121.0, true);
        var fh6 = Hole.FixingHole_M3;
        fh6.SetPosition(74.0, 121.0, true);
        var fh7 = Hole.FixingHole_M3;
        fh7.SetPosition(181.0, 121.0, true);
        var fh8 = Hole.FixingHole_M3;
        fh8.SetPosition(244.0, 121.0, true);

        var fixingHoles = new List<Hole>() { fh1, fh2, fh3, fh4, fh5, fh6, fh7, fh8 };

        panel.SetSocketPoints(sockets);
        panel.SetFixingHoles(fixingHoles);

        return panel;
    }

    public static FixedGridPanel BuildTechPanel_4Gang()
    {
        var panel = new FixedGridPanel("Tech Panel - 4 Gang");
        var panelWidth = 340.0d;
        panel.SetSize(panelWidth, TechPanelHeight);

        panel.SetLabelText("TP 4 GANG");
        panel.SetLabelTextHeight(StandardTextHeight.Large);
        panel.SetLabelPosition(new Vector2(panelWidth / 2, 10.0d), true);
        panel.SetLabelTextAlignCenter(true);
        panel.CornerFilletRadius = 2.0d;

        var sockets = new List<Vector2>()
        {
            new Vector2(26.0, TechPanelFirstRow),
            new Vector2(58.0, TechPanelFirstRow),
            new Vector2(90.0, TechPanelFirstRow),
            new Vector2(122.0, TechPanelFirstRow),
            new Vector2(154.0, TechPanelFirstRow),
            new Vector2(186.0, TechPanelFirstRow),
            new Vector2(218.0, TechPanelFirstRow),
            new Vector2(250.0, TechPanelFirstRow),
            new Vector2(282.0, TechPanelFirstRow),
            new Vector2(314.0, TechPanelFirstRow),

            new Vector2(26.0, TechPanelSecondRow),
            new Vector2(58.0, TechPanelSecondRow),
            new Vector2(90.0, TechPanelSecondRow),
            new Vector2(122.0, TechPanelSecondRow),
            new Vector2(154.0, TechPanelSecondRow),
            new Vector2(186.0, TechPanelSecondRow),
            new Vector2(218.0, TechPanelSecondRow),
            new Vector2(250.0, TechPanelSecondRow),
            new Vector2(282.0, TechPanelSecondRow),
            new Vector2(314.0, TechPanelSecondRow)
        };

        // top row
        var fh1 = Hole.FixingHole_M3;
        fh1.SetPosition(11.0, 7.0, true);
        var fh2 = Hole.FixingHole_M3;
        fh2.SetPosition(74.0, 7.0, true);
        var fh3 = Hole.FixingHole_M3;
        fh3.SetPosition(266.0, 7.0, true);
        var fh4 = Hole.FixingHole_M3;
        fh4.SetPosition(329.0, 7.0, true);
        // bottom row
        var fh5 = Hole.FixingHole_M3;
        fh5.SetPosition(11.0, 121.0, true);
        var fh6 = Hole.FixingHole_M3;
        fh6.SetPosition(74.0, 121.0, true);
        var fh7 = Hole.FixingHole_M3;
        fh7.SetPosition(266.0, 121.0, true);
        var fh8 = Hole.FixingHole_M3;
        fh8.SetPosition(329.0, 121.0, true);

        var fixingHoles = new List<Hole>() { fh1, fh2, fh3, fh4, fh5, fh6, fh7, fh8 };

        panel.SetSocketPoints(sockets);
        panel.SetFixingHoles(fixingHoles);

        return panel;
    }

    public static FixedGridPanel BuildTechPanel_5Gang()
    {
        var panel = new FixedGridPanel("Tech Panel - 5 Gang");
        var panelWidth = 425.0d;
        panel.SetSize(panelWidth, TechPanelHeight);

        panel.SetLabelText("TP 5 GANG");
        panel.SetLabelTextHeight(StandardTextHeight.Large);
        panel.SetLabelPosition(new Vector2(panelWidth / 2, 10.0d), true);
        panel.SetLabelTextAlignCenter(true);
        panel.CornerFilletRadius = 2.0d;

        var sockets = new List<Vector2>()
        {
            new Vector2(20.5, TechPanelFirstRow),
            new Vector2(52.5, TechPanelFirstRow),
            new Vector2(84.5, TechPanelFirstRow),
            new Vector2(116.5, TechPanelFirstRow),
            new Vector2(148.5, TechPanelFirstRow),
            new Vector2(148.5, TechPanelFirstRow),
            new Vector2(180.5, TechPanelFirstRow),
            new Vector2(212.5, TechPanelFirstRow),
            new Vector2(244.5, TechPanelFirstRow),
            new Vector2(276.5, TechPanelFirstRow),
            new Vector2(308.5, TechPanelFirstRow),
            new Vector2(340.5, TechPanelFirstRow),
            new Vector2(372.5, TechPanelFirstRow),
            new Vector2(404.5, TechPanelFirstRow),

            new Vector2(20.5, TechPanelSecondRow),
            new Vector2(52.5, TechPanelSecondRow),
            new Vector2(84.5, TechPanelSecondRow),
            new Vector2(116.5, TechPanelSecondRow),
            new Vector2(148.5, TechPanelSecondRow),
            new Vector2(148.5, TechPanelSecondRow),
            new Vector2(180.5, TechPanelSecondRow),
            new Vector2(212.5, TechPanelSecondRow),
            new Vector2(244.5, TechPanelSecondRow),
            new Vector2(276.5, TechPanelSecondRow),
            new Vector2(308.5, TechPanelSecondRow),
            new Vector2(340.5, TechPanelSecondRow),
            new Vector2(372.5, TechPanelSecondRow),
            new Vector2(404.5, TechPanelSecondRow),
        };

        // top row
        var fh1 = Hole.FixingHole_M3;
        fh1.SetPosition(11.0, 7.0, true);
        var fh2 = Hole.FixingHole_M3;
        fh2.SetPosition(159.0, 7.0, true);
        var fh3 = Hole.FixingHole_M3;
        fh3.SetPosition(266.0, 7.0, true);
        var fh4 = Hole.FixingHole_M3;
        fh4.SetPosition(414.0, 7.0, true);
        // bottom row
        var fh5 = Hole.FixingHole_M3;
        fh5.SetPosition(11.0, 121.0, true);
        var fh6 = Hole.FixingHole_M3;
        fh6.SetPosition(159.0, 121.0, true);
        var fh7 = Hole.FixingHole_M3;
        fh7.SetPosition(266.0, 121.0, true);
        var fh8 = Hole.FixingHole_M3;
        fh8.SetPosition(414.0, 121.0, true);

        var fixingHoles = new List<Hole>() { fh1, fh2, fh3, fh4, fh5, fh6, fh7, fh8 };

        panel.SetSocketPoints(sockets);
        panel.SetFixingHoles(fixingHoles);

        return panel;
    }
}
