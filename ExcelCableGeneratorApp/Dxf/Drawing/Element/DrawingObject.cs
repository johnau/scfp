using System.Numerics;
namespace ExcelCableGeneratorApp.Dxf.Drawing.Element;

internal class DrawingObject
{
    public string Id { get; init; }
    public string NameTag { get; private set; }
    public Vector2 PositionRelative { get; private set; }
    /// <summary>
    /// 
    /// </summary>
    public Vector2 Size { get; private set; }
    public Vector2 Center { get; private set; } // relative to the size
    ///// <summary>
    ///// Bounds calculated either side of center (?) not sure if this is the right approach, maybe bounds should be from 0,0 to SizeX, SizeY, in which case bounds = size.
    ///// </summary>
    //public Tuple<Vector2, Vector2> Bounds => Tuple.Create(new Vector2(-Size.X/2, -Size.Y/2), new Vector2(Size.X/2, Size.Y/2));
    ///// <summary>
    ///// For convience and documentation
    ///// </summary>
    //public Vector2 CenterOfBounds => Vector2.Zero;
    public float MarginTop { get; private set; }
    public float MarginBottom { get; private set; }
    public float MarginLeft { get; private set; }
    public float MarginRight { get; private set; }

    public DrawingObject(string nameTag, float margin_Top = 0, float margin_Bottom = 0, float margin_Left = 0, float margin_Right = 0)
    {
        Id = Guid.NewGuid().ToString();

        PositionRelative = Vector2.Zero;
        Size = Vector2.One;
        Center = CalculateCenter();

        NameTag = nameTag;
        MarginTop = margin_Top;
        MarginBottom = margin_Bottom;
        MarginLeft = margin_Left;
        MarginRight = margin_Right;
    }

    private Vector2 CalculateCenter()
    {
        return new Vector2(Size.X / 2, Size.Y / 2);
    }

    /// <summary>
    /// Sets the width and height of the drawing element
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <exception cref="ArgumentException"></exception>
    public virtual void SetSize(float width, float height)
    {
        if (width < 1.0f || height < 1.0f)
            throw new ArgumentException("Cannot set dimensions less than 1.0mm");
        Size = new Vector2(width, height);
        Center = CalculateCenter();
    }

    /// <summary>
    /// Sets the position of the drawing element relative to its parent
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public virtual void SetPositionRelative(float x, float y)
    {
        PositionRelative = new Vector2(x, y);
    }

    /// <summary>
    /// Sets the position of the drawing element relative to its parent
    /// </summary>
    /// <param name="position"></param>
    public virtual void SetPositionRelative(Vector2 position)
    {
        PositionRelative = position;
    }

    /// <summary>
    /// Sets the margins of the drawing element
    /// </summary>
    /// <param name="value"></param>
    /// <exception cref="ArgumentException"></exception>
    public void SetMargin(float value)
    {
        if (value < 0) 
            throw new ArgumentException("Margin cannot be less than 0");

        MarginTop = value;
        MarginBottom = value;
        MarginLeft = value;
        MarginRight = value;
    }

    /// <summary>
    /// Set Top margin
    /// </summary>
    /// <param name="value"></param>
    public void SetMarginTop(float value) => MarginTop = value;
    /// <summary>
    /// Set Bottom margin
    /// </summary>
    /// <param name="value"></param>
    public void SetMarginBottom(float value) => MarginBottom = value;
    /// <summary>
    /// Set Left margin
    /// </summary>
    /// <param name="value"></param>
    public void SetMarginLeft(float value) => MarginLeft = value;
    /// <summary>
    /// Set Right margin
    /// </summary>
    /// <param name="value"></param>
    public void SetMarginRight(float value) => MarginRight = value;
    /// <summary>
    /// Set name tag
    /// </summary>
    /// <param name="value"></param>
    public void SetNameTag(string value) => NameTag = value;

    public virtual List<Vector2> GetVertices()
    {
        throw new NotImplementedException();
    }

    public virtual List<Tuple<Vector2, Vector2>> GetEdges()
    {
        throw new NotImplementedException();
    }

    public virtual Vector2 GetCenterPoint()
    {
        throw new NotImplementedException();
    }

    public virtual Tuple<Vector2, Vector2> GetBounds()
    {
        throw new NotImplementedException();
    }
}
