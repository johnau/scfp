using netDxf;
using netDxf.Collections;

namespace ExcelCableGeneratorApp.Dxf.Drawing.Element;

internal class Socket : LabelledDrawingObject
{
    public static Socket XLR_Female_TypeA = new Socket("Female XLR Type-A", SocketType.XLR_FEMALE_TYPE_A);
    public static Socket XLR_Male_TypeA = new Socket("Male XLR Type-A", SocketType.XLR_MALE_TYPE_A);
    public static Socket XLR_Female_TypeD = new Socket("Female XLR Type-D", SocketType.XLR_FEMALE_TYPE_D);
    public static Socket XLR_Male_TypeD = new Socket("Male XLR Type-D", SocketType.XLR_MALE_TYPE_D);
    public static Socket DMX_Female_TypeA = new Socket("Female DMX Type-A", SocketType.DMX_FEMALE_TYPE_A);
    public static Socket DMX_Male_TypeA = new Socket("Male DMX Type-A", SocketType.DMX_MALE_TYPE_A);
    public static Socket DMX_Female_TypeD = new Socket("Female DMX Type-D", SocketType.DMX_FEMALE_TYPE_D);
    public static Socket DMX_Male_TypeD = new Socket("Male DMX Type-D", SocketType.DMX_MALE_TYPE_D);
    public static Socket CAT6A_Female_TypeD = new Socket("Female Cat6A Type-D", SocketType.CAT6A_FEMALE_TYPE_D);

    public static readonly float SocketToTextGap = 3.0f;

    private List<Hole> _fixingHoles;
    private List<Hole> _otherHoles;
    public override ElementType Type => ElementType.SOCKET;
    public SocketType SocketType { get; init; }
    public float SocketHoleRadius { get; private set; }
    public Vector2 SocketHolePosition => Center;

    public Socket(string nameTag, SocketType type, float margin_Top = 0, float margin_Bottom = 0, float margin_Left = 0, float margin_Right = 0)
        : base(nameTag, margin_Top, margin_Bottom, margin_Left, margin_Right)
    {
        _fixingHoles = [];
        _otherHoles = [];
        SocketType = type;
        SetSocketPropertiesBasedOnType();
        SetLabelVisible(true);
        SetLabelTextHeight(4.0f);
    }

    /// <summary>
    /// Override SetLabelVisible as Socket must always have a Visible Label
    /// </summary>
    /// <param name="visible"></param>
    /// <exception cref="Exception"></exception>
    public override void SetLabelVisible(bool visible)
    {
        if (visible == false)
        {
            throw new Exception("Cannot turn label off for Socket");
        }
        base.SetLabelVisible(true);
    }

    private void SetSocketPropertiesBasedOnType()
    {
        switch (SocketType)
        {
            case SocketType.XLR_FEMALE_TYPE_A:
            case SocketType.XLR_MALE_TYPE_A:
            case SocketType.DMX_FEMALE_TYPE_A:
            case SocketType.DMX_MALE_TYPE_A:
                SetupTypeASocket();
                break;
            case SocketType.XLR_FEMALE_TYPE_D:
            case SocketType.XLR_MALE_TYPE_D:
            case SocketType.DMX_FEMALE_TYPE_D:
            case SocketType.DMX_MALE_TYPE_D:
                SetupTypeDSocket();
                break;
            case SocketType.CAT6A_FEMALE_TYPE_D:
                SetupTypeDSocket();
                break;
            default:
                break;
        }
    }

    public bool TryAddFixingHole(Hole hole)
    {
        // check collisions with other holes

        _fixingHoles.Add(hole);
        return true;
    }

    private void SetupTypeASocket()
    {
        SocketHoleRadius = 11.0f;

        SetSize(26.0f, 31.0f); // bounding box size designed by Lovey
        var fixingHole1 = Hole.FixingHole_M3;
        var fixingHole2 = Hole.FixingHole_M3;

        fixingHole1.SetPosition(new Vector2(Center.X - 9.9f, Center.Y - 9.9f));
        fixingHole2.SetPosition(new Vector2(Center.X + 9.9f, Center.Y + 9.9f));

        _fixingHoles.AddRange([fixingHole1, fixingHole2]);
    }

    private void SetupTypeDSocket()
    {
        SocketHoleRadius = 12.0f;

        SetSize(26.0f, 31.0f); // bounding box size designed by Lovey
        var fixingHole1 = Hole.FixingHole_M3;
        var fixingHole2 = Hole.FixingHole_M3;
        
        fixingHole1.SetPosition(new Vector2(Center.X - 9.5f, Center.Y - 12.0f));
        fixingHole2.SetPosition(new Vector2(Center.X + 9.5f, Center.Y + 12.0f));
        
        _fixingHoles.AddRange([fixingHole1, fixingHole2]);
    }

    public override bool Draw(DrawingEntities drawing)
    {
        DrawLabel(drawing);
        
        DrawOutline(drawing, AciColor.Green);

        return true;
    }
}
