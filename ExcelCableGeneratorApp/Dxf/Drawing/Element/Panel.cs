namespace ExcelCableGeneratorApp.Dxf.Drawing.Element;

internal class Panel : LabelledDrawingObject
{
    public static Panel RU_1 = new Panel("1RU Width Panel");
    public static Panel RU_2 = new Panel("2RU Width Panel");
    public static Panel RU_3 = new Panel("3RU Width Panel");
    public static Panel RU_4 = new Panel("4RU Width Panel");

    public static Panel U_1 = new Panel("1U Height Panel");
    public static Panel U_2 = new Panel("2U Height Panel");
    public static Panel U_3 = new Panel("3U Height Panel");
    public static Panel U_4 = new Panel("4U Height Panel");

    private List<Obstruction> _obstructions;
    private List<Screw> _screws;
    private List<Hole> _holes;
    public List<Obstruction> Obstructions { get => new List<Obstruction>(_obstructions); }
    public List<Screw> Screws { get => new List<Screw>(_screws); }
    public List<Hole> Holes { get => new List<Hole>(_holes);  }

    public Panel(string nameTag, float margin_Top = 0, float margin_Bottom = 0, float margin_Left = 0, float margin_Right = 0) 
        : base(nameTag, margin_Top, margin_Bottom, margin_Left, margin_Right)
    {
        _obstructions = [];
        _screws = [];
        _holes = [];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obstruction"></param>
    /// <returns></returns>
    public bool TryAddObstruction(Obstruction obstruction)
    {
        // check against other obstructions

        // check against holes

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

        _holes.Add(hole);
        return true;
    }
}
