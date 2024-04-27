using System.Numerics;

namespace ExcelCableGeneratorApp.Dxf.Aggregates;

internal class PanelData
{
    protected const float Height_1U_mm = 44.5f;
    protected const float Panel_Standard_Width = 448f;
    protected const float PanelFixHole_Radius_M3 = 1.6f;
    protected const float CX_PanelFixHole_CLfromEdge = 6.5f;
    protected const float CY_PanelFixHole_CLfromEdge = CX_PanelFixHole_CLfromEdge;
    protected const float CX_PanelFixHole_CC = 145.0f;
    protected const float CY_PanelFixHole_CC = 120.0f;
    
    public static float PanelHeightMmFromU(int u) => u * Height_1U_mm;

    /// <summary>
    /// Creates an instance of a Full Width panel
    /// </summary>
    /// <param name="uHeight"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static PanelData Standard_UHeight_FullWidth(int uHeight) 
    {
        var panelHeightMm = PanelHeightMmFromU(uHeight);
        var panelEdgeOffset = 5.0f;
        var pd = new PanelData(uHeight, Panel_Standard_Width, panelEdgeOffset);
        var x_left = CX_PanelFixHole_CLfromEdge;
        var x_innerLeft = CX_PanelFixHole_CLfromEdge + CX_PanelFixHole_CC;
        var x_innerRight = Panel_Standard_Width - CX_PanelFixHole_CC - CX_PanelFixHole_CLfromEdge;
        var x_right = Panel_Standard_Width - CX_PanelFixHole_CLfromEdge;
        if (x_innerRight - x_innerLeft != CX_PanelFixHole_CC) throw new Exception("Values have changed and broken fixing hole placement");
        var y_top = CY_PanelFixHole_CLfromEdge;
        var y_bottom = panelHeightMm - CY_PanelFixHole_CLfromEdge;

        pd.AddFixingHole(new Vector2(x_left, y_top), PanelFixHole_Radius_M3); //top left
        pd.AddFixingHole(new Vector2(x_innerLeft, y_top), PanelFixHole_Radius_M3); //top middle left
        pd.AddFixingHole(new Vector2(x_innerRight, y_top), PanelFixHole_Radius_M3); //top middle right
        pd.AddFixingHole(new Vector2(x_right, y_top), PanelFixHole_Radius_M3); //top right
        pd.AddFixingHole(new Vector2(x_left, y_bottom), PanelFixHole_Radius_M3);
        pd.AddFixingHole(new Vector2(x_innerLeft, y_bottom), PanelFixHole_Radius_M3);
        pd.AddFixingHole(new Vector2(x_innerRight, y_bottom), PanelFixHole_Radius_M3);
        pd.AddFixingHole(new Vector2(x_right, y_bottom), PanelFixHole_Radius_M3);

        /** Handle these with EdgeOffset on Panel
         * 
         * var ridgeDepth = 0.5f;
         * pd.AddObstruction(Vector2.Zero, new Vector2(Panel_Standard_Width,ridgeDepth), 0); // top ridge at back of panel
         * pd.AddObstruction(new Vector2(0, panelHeightMm - ridgeDepth), new Vector2(Panel_Standard_Width, 0), 0); // bottom ridge at back of panel
         */

        var y_top2a = y_top - 17.5f;
        var y_top2b = y_top + 12.5f;
        var y_top2c = y_top - 15f;
        var y_top2d = y_top + 5f;
        
        var y_bot2a = y_bottom - 12.5f; 
        var y_bot2b = y_bottom + 17.5f;
        var y_bot2c = y_bottom - 5f; 
        var y_bot2d = y_bottom + 15f;

        var x_lef2a = x_left - 12.5f;
        var x_lef2b = x_left + 7.5f;

        var x_rig2a = x_right - 7.5f;
        var x_rig2b = x_right + 12.5f;

        pd.AddObstruction(new Vector2(x_lef2a, y_top2a), new Vector2(x_lef2b, y_top2b), 0, 45f); // fixing hole manifold blockages, top left
        pd.AddObstruction(new Vector2(x_innerLeft - 10f, y_top2c), new Vector2(x_innerLeft + 10f , y_top2d), 0, 45f); // top center
        pd.AddObstruction(new Vector2(x_innerRight - 10f, y_top2c), new Vector2(x_innerRight + 10f , y_top2d), 0, 45f); // top center
        pd.AddObstruction(new Vector2(x_rig2a, y_top2a), new Vector2(x_rig2b, y_top2b), 0, -45f); // top top right

        pd.AddObstruction(new Vector2(x_lef2a, y_bot2a), new Vector2(x_lef2b, y_bot2b), 0, -45f); // fixing hole manifold blockages, bottom left
        pd.AddObstruction(new Vector2(x_innerLeft - 10f, y_bot2c), new Vector2(x_innerLeft + 10f, y_bot2d), 0, 45f); // bottom center
        pd.AddObstruction(new Vector2(x_innerRight - 10f, y_bot2c), new Vector2(x_innerRight + 10f, y_bot2d), 0, 45f); // bottom center
        pd.AddObstruction(new Vector2(x_rig2a, y_bot2a), new Vector2(x_rig2b, y_bot2b), 0, 45f); // bottom right

        var section1 = new SectionData(
            new Vector2(Panel_Standard_Width - panelEdgeOffset*2, panelHeightMm - panelEdgeOffset*2), 
            Vector2.Zero
        );

        pd.AddSection(section1);

        return pd;
    }

    private Dictionary<int, SectionData> _sections;
    private List<Tuple<Vector2, float, HoleShape>> _fixingHoles; // Tuple of position and radius
    private List<Tuple<Vector2, Vector2, int, double>> _obstructions;
    public int PanelHeightInU { get; }
    public float PanelHeightInMm { get; }
    public float PanelWidthInMm { get; }
    public float PanelEdgeOffsetMm { get; }
    public List<Tuple<Vector2, float, HoleShape>> FixingHoles { get => new(_fixingHoles); }
    public List<Tuple<Vector2, Vector2, int, double>> Obstructions { get => new(_obstructions); }
    public Dictionary<int, SectionData> Sections { get => new(_sections); }

    public PanelData(int panelUheight, float panelWidth = Panel_Standard_Width, float panelEdgeOffset = 5.0f)
    {
        PanelHeightInU = panelUheight;
        PanelHeightInMm = PanelHeightMmFromU(panelUheight);
        PanelWidthInMm = panelWidth;
        PanelEdgeOffsetMm = panelEdgeOffset;

        _sections = [];
        _fixingHoles = [];
        _obstructions = [];
    }

    public void AddSection(SectionData section)
    {
        _sections[_sections.Count] = section;
    }

    /// <summary>
    /// Add a fixing hole to the Panel
    /// </summary>
    /// <param name="position">Position relative to the top left corner @ (0, 0)</param>
    /// <param name="radius"></param>
    /// <param name="shape"></param>
    public void AddFixingHole(Vector2 position, float radius, HoleShape shape = HoleShape.CIRCLE)
    {
        _fixingHoles.Add(Tuple.Create(position, radius, shape));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="boundTopLeft"></param>
    /// <param name="boundBottomRight"></param>
    /// <param name="z">Z axis position of obstruction: a value of 0 = Back, 1 = Front, defaults to Back</param>
    /// <param name="rotation">Rotation of the obstruction rect</param>
    public void AddObstruction(Vector2 boundTopLeft, Vector2 boundBottomRight, int z = 0, double rotation = 0)
    {
        _obstructions.Add(Tuple.Create(boundTopLeft, boundBottomRight, z, rotation));
    }
}
