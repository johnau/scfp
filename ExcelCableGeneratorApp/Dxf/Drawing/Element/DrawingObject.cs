using netDxf;
using netDxf.Collections;
using netDxf.Entities;
using netDxf.Tables;
namespace ExcelCableGeneratorApp.Dxf.Drawing.Element;

internal class DrawingObject
{
    public string Id { get; init; }
    public virtual ElementType Type => ElementType.NONE;
    public string NameTag { get; private set; }
    public Vector2 Position { get; private set; }
    public bool PositionIsCenter { get; private set; } // alternative being the default (top left)
    /// <summary>
    /// 
    /// </summary>
    public Vector2 Size { get; private set; }
    public Vector2 Center { get; private set; } // relative to the size + position
    public double Rotation { get; private set; }

    public DrawingObject? Parent { get; set; }

    ///// <summary>
    ///// Bounds calculated either side of center (?) not sure if this is the right approach, maybe bounds should be from 0,0 to SizeX, SizeY, in which case bounds = size.
    ///// </summary>
    //public Tuple<Vector2, Vector2> Bounds => Tuple.Create(new Vector2(-Size.X/2, -Size.Y/2), new Vector2(Size.X/2, Size.Y/2));
    ///// <summary>
    ///// For convience and documentation
    ///// </summary>
    //public Vector2 CenterOfBounds => Vector2.Zero;

    // Margin 
    public float MarginTop { get; private set; }
    public float MarginBottom { get; private set; }
    public float MarginLeft { get; private set; }
    public float MarginRight { get; private set; }

    public DrawingObject(string nameTag, float margin_Top = 0, float margin_Bottom = 0, float margin_Left = 0, float margin_Right = 0)
    {
        Id = Guid.NewGuid().ToString();

        Position = Vector2.Zero;
        PositionIsCenter = false;
        Size = Vector2.Zero;
        Center = CalculateCenter();

        NameTag = nameTag;
        MarginTop = margin_Top;
        MarginBottom = margin_Bottom;
        MarginLeft = margin_Left;
        MarginRight = margin_Right;
    }

    private Vector2 CalculateCenter()
    {
        if (PositionIsCenter)
        {
            return Position;
        }

        return new Vector2(Position.X + Size.X / 2, Position.Y + Size.Y / 2);
    }

    /// <summary>
    /// Sets the width and height of the drawing element
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <exception cref="ArgumentException"></exception>
    public virtual void SetSize(double width, double height)
    {
        if (width < 0.0f || height < 0.0f)
            throw new ArgumentException("Cannot set dimensions less than 1.0mm");
        Size = new Vector2(width, height);
        Center = CalculateCenter();
    }

    public void SetSize(Vector2 size)
    {
        SetSize(size.X, size.Y);
    }

    /// <summary>
    /// Sets the position of the drawing element relative to its parent
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public virtual void SetPosition(double x, double y, bool centerPoint = false)
    {
        SetPosition(new Vector2(x, y), centerPoint);
    }

    /// <summary>
    /// Sets the position of the drawing element relative to its parent
    /// </summary>
    /// <param name="position"></param>
    public virtual void SetPosition(Vector2 position, bool centerPoint = false)
    {
        Position = position;
        PositionIsCenter = centerPoint;
        Center = CalculateCenter();

        RepositionChildren();
        //set position of children
    }

    protected virtual void RepositionChildren() { }

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
    public void SetMarginTop(float value)
    {
        if (value < 0)
            throw new ArgumentException("Margin cannot be less than 0");
        MarginTop = value;
    }
    /// <summary>
    /// Set Bottom margin
    /// </summary>
    /// <param name="value"></param>
    public void SetMarginBottom(float value)
    {
        if (value < 0)
            throw new ArgumentException("Margin cannot be less than 0");
        MarginBottom = value;
    }
    /// <summary>
    /// Set Left margin
    /// </summary>
    /// <param name="value"></param>
    public void SetMarginLeft(float value)
    {
        if (value < 0)
            throw new ArgumentException("Margin cannot be less than 0");
        MarginLeft = value;
    }
    /// <summary>
    /// Set Right margin
    /// </summary>
    /// <param name="value"></param>
    public void SetMarginRight(float value)
    {
        if (value < 0)
            throw new ArgumentException("Margin cannot be less than 0");
        MarginRight = value;
    }

    public void SetRotation(double degrees)
    {
        if (degrees < -360 || degrees > 360)
            throw new ArgumentException("Invalid rotation value");

        Rotation = degrees;
    }

    /// <summary>
    /// Set name tag
    /// </summary>
    /// <param name="value"></param>
    public void SetNameTag(string value) => NameTag = value;

    /// <summary>
    /// Get the Vector2 Vertices for this DrawingObject
    /// </summary>
    /// <returns>List of Vector2 points, in order: Top Left, Top Right, Bottom Right, Bottom Left</returns>
    public virtual List<Vector2> GetDwgVertices()
    {
        Vector2 topLeft, topRight, bottomLeft, bottomRight;
        if (PositionIsCenter)
        {
            topLeft = new Vector2(Position.X - Size.X / 2, Position.Y - Size.Y / 2);
            topRight = new Vector2(Position.X + Size.X / 2, Position.Y - Size.Y / 2);
            bottomRight = new Vector2(Position.X + Size.X / 2, Position.Y + Size.Y / 2);
            bottomLeft = new Vector2(Position.X - Size.X / 2, Position.Y + Size.Y / 2);
        }
        else
        {
            topLeft = Position;
            topRight = new Vector2(Position.X + Size.X, Position.Y);
            bottomRight = new Vector2(Position.X + Size.X, Position.Y + Size.Y);
            bottomLeft = new Vector2(Position.X, Position.Y + Size.Y);
        }

        List<Vector2> verts = [topLeft, topRight, bottomRight, bottomLeft];

        var verts_rotated = VecHelper.RotatePointsAroundPoint(verts, Rotation, Center);

        var verts_flipped = VecHelper.FlipYAxis(verts_rotated);

        return verts_flipped;

        //// Rotate all vertices
        //double angleRadians = MathHelper.DegToRad * Rotation; // Convert 45 degrees to radians
        //for (int i = 0; i < verts.Count; i++)
        //{
        //    if (Rotation != 0)
        //    {
        //        var translatedVert = verts[i] - Center;
        //        var rotatedVert = Vector2.Rotate(translatedVert, angleRadians);
        //        verts[i] = rotatedVert + Center;
        //    }

        //    verts[i] = new Vector2(verts[i].X, -verts[i].Y);
        //}

        //return [topLeft, topRight, bottomRight, bottomLeft];
    }

    public virtual List<Tuple<Vector2, Vector2>> GetEdges()
    {
        throw new NotImplementedException();
    }

    public virtual Vector2 GetCenterPoint()
    {
        return Center;
    }

    /// <summary>
    /// Returns Top Left and Bottom Right points
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual Tuple<Vector2, Vector2> GetBounds()
    {
        Vector2 topLeft, topRight, bottomLeft, bottomRight;
        if (PositionIsCenter)
        {
            topLeft = new Vector2(Position.X - Size.X / 2, Position.Y - Size.Y / 2);
            topRight = new Vector2(Position.X + Size.X / 2, Position.Y - Size.Y / 2);
            bottomRight = new Vector2(Position.X + Size.X / 2, Position.Y + Size.Y / 2);
            bottomLeft = new Vector2(Position.X - Size.X / 2, Position.Y + Size.Y / 2);
        } else
        {
            topLeft = Position;
            topRight = new Vector2(Position.X + Size.X, Position.Y);
            bottomRight = new Vector2(Position.X + Size.X, Position.Y + Size.Y);
            bottomLeft = new Vector2(Position.X, Position.Y + Size.Y);
        }
        return Tuple.Create(topLeft, bottomRight);
    }

    public virtual bool Draw(DrawingEntities drawing)
    {
        throw new NotImplementedException("Draw " + NameTag);
    }

    public virtual bool ConfigureDrawing(DxfDocument dxf)
    {
        
        var panelDebug = new Layer("panel_debug");
        var sectionDebug = new Layer("section_debug");
        var groupDebug = new Layer("group_debug");
        dxf.Layers.Add(panelDebug);
        dxf.Layers.Add(sectionDebug);
        dxf.Layers.Add(groupDebug);


        return true;
    }

    protected virtual void DrawOutline(DrawingEntities drawing, AciColor color)
    {
        //List<Vector2> vertices = [
        //    new Vector2(Position.X, -Position.Y),
        //    new Vector2(Position.X + Size.X, -Position.Y), 
        //    new Vector2(Position.X + Size.X, -(Position.Y + Size.Y)), 
        //    new Vector2(Position.X, -(Position.Y + Size.Y))  
        //];

        var vertices = GetDwgVertices();

        var polyline = new Polyline2D(vertices, true);
        polyline.Color = color;

        drawing.Add(polyline);
    }
}
