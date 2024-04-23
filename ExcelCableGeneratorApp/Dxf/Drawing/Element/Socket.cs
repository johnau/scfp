using System.Numerics;

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
    public SocketType Type { get; init; }
    public float SocketHoleRadius { get; private set; }
    public Vector2 SocketHolePosition => Center;

    public Socket(string nameTag, SocketType type, float margin_Top = 0, float margin_Bottom = 0, float margin_Left = 0, float margin_Right = 0)
        : base(nameTag, margin_Top, margin_Bottom, margin_Left, margin_Right)
    {
        _fixingHoles = [];
        _otherHoles = [];
        Type = type;
        SetSocketBoundingBoxDimensionsBasedOnType();
    }

    private void SetSocketBoundingBoxDimensionsBasedOnType()
    {
        Hole fixingHole1;
        Hole fixingHole2;
        switch (Type)
        {
            case SocketType.XLR_FEMALE_TYPE_A:
            case SocketType.XLR_MALE_TYPE_A:
            case SocketType.DMX_FEMALE_TYPE_A:
            case SocketType.DMX_MALE_TYPE_A:
                SetSize(26.0f, 31.0f); // bounding box size designed by Lovey
                fixingHole1 = Hole.FixingHole_M3;
                fixingHole1.SetPositionRelative(new Vector2(Center.X - 9.9f, Center.Y - 9.9f));
                fixingHole2 = Hole.FixingHole_M3;
                fixingHole2.SetPositionRelative(new Vector2(Center.X + 9.9f, Center.Y + 9.9f));
                _fixingHoles.AddRange([fixingHole1, fixingHole2]);
                SocketHoleRadius = 11.0f;
                break;
            case SocketType.XLR_FEMALE_TYPE_D:
            case SocketType.XLR_MALE_TYPE_D:
            case SocketType.DMX_FEMALE_TYPE_D:
            case SocketType.DMX_MALE_TYPE_D:
                // need to work out Type D fixing hole positions and socket hole diameter, plus notches
                SetSize(26.0f, 31.0f); // bounding box size designed by Lovey
                fixingHole1 = Hole.FixingHole_M3;
                fixingHole1.SetPositionRelative(new Vector2(Center.X - 9.5f, Center.Y - 12.0f));
                fixingHole2 = Hole.FixingHole_M3;
                fixingHole2.SetPositionRelative(new Vector2(Center.X + 9.5f, Center.Y + 12.0f));
                _fixingHoles.AddRange([fixingHole1, fixingHole2]);
                SocketHoleRadius = 12.0f;
                break;
            case SocketType.CAT6A_FEMALE_TYPE_D:
                // need to work out Type D fixing hole positions and socket hole diameter, plus notches
                break;
            default:
                break;
        }
    }

    public void TryAddFixingHole(Hole hole)
    {
        // check collisions with other holes


    }
}
