using netDxf;
using netDxf.Collections;
using netDxf.Entities;
using netDxf.Tables;
using System.Diagnostics;

namespace ExcelCableGeneratorApp.Dxf.Drawing.Element;

internal class FixedGridPanel : LabelledDrawingObject
{
    private int _nextFreeSocketPoint;
    private Dictionary<int, Vector2> _socketPoints;
    private Dictionary<string, List<int>> _socketGroups;
    private Dictionary<int, (string, SocketType)> _socketTypes;
    private Dictionary<int, Socket> _socketObjects; // using the already created socket objects to draw the actual sockets
    private Dictionary<string, GroupingBracket> _groupingBracketObjects;
    private List<Hole> _fixingHoles;

    public Dictionary<int, Vector2> SocketPoints => new Dictionary<int, Vector2>(_socketPoints);
    //public double SocketCenterToLabelGap { get; set; }
    public double CornerFilletRadius { get; set; }
    public double GroupBracketDistFromSocketCenter { get; set; }
    public double LabelDistanceFromBracket { get; set; }

    public FixedGridPanel(string nameTag, float margin_Top = 0, float margin_Bottom = 0, float margin_Left = 0, float margin_Right = 0) 
        : base(nameTag, margin_Top, margin_Bottom, margin_Left, margin_Right)
    {
        _socketPoints = [];
        _socketGroups = [];
        _socketTypes = [];
        _socketObjects = [];
        _groupingBracketObjects = [];
        _fixingHoles = [];
        _nextFreeSocketPoint = 1;
        
        //SocketCenterToLabelGap = 18.5d;
        CornerFilletRadius = 2.0d;
        GroupBracketDistFromSocketCenter = 27.0d;
        LabelDistanceFromBracket = 1.0d;
    }

    //public void SetSocketPoints(Dictionary<int, Vector2> socketPoints)
    public void SetSocketPoints(List<Vector2> socketPoints)
    {
        // check socket points all fit on panel

        // check socket indexes are consecutive
        //for (int i = 1; i <= socketPoints.Count; i++)
        for (int i = 0; i < socketPoints.Count; i++)
        {
            _socketPoints[_socketPoints.Count + 1] = socketPoints[i];
            //if (!socketPoints.TryGetValue(i, out _))
            //{
            //    throw new Exception("The provided socket points must have consecutive ID numbers, starting at 1.");
            //}
        }

        //_socketPoints = socketPoints;
    }

    public int NextAvailableSocketPoint()
    {
        return _nextFreeSocketPoint;
    }

    public void SetupSocketPoint(int index, string label, SocketType socketType)
    {
        _socketTypes.Add(index, (label, socketType));

        var socketObject = Socket.Default(socketType);
        var socketPos = _socketPoints[index];
        var absSocketPos = socketPos + Position;

        var labelHeight = 4.0f;
        socketObject.SetPosition(absSocketPos, true);
        socketObject.SetLabelPosition(new Vector2(0, -socketObject.LabelDistFromSocketCenter - labelHeight), true); // this is not nice, we need to have some property for label offset from the object it is labelling (from its original center point/position)
        socketObject.SetLabelText(label);
        socketObject.SetLabelTextHeight(labelHeight);
        _socketObjects.Add(index, socketObject);
    }

    public int SetupNextFreeSocketPoint(string label, SocketType socketType, SocketFormat socketFormat = SocketFormat.TYPE_A)
    {
        var index = _nextFreeSocketPoint;

        SetupSocketPoint(index, label, socketType);

        _nextFreeSocketPoint++;
        return index;
    }

    public void CreateSocketGroup(string groupLabel)
    {
        if (!_socketGroups.ContainsKey(groupLabel))
        {
            _socketGroups.Add(groupLabel, []);
            var bracket = new GroupingBracket("Grouping Bracket for: " + groupLabel);
            bracket.SetLabelText(groupLabel);
            _groupingBracketObjects.Add(groupLabel, bracket);
        }
    }

    public void AddSocketIndexToGroup(int socketIndex, string groupLabel)
    {
        if (!_socketGroups.TryGetValue(groupLabel, out var indexList))
        {
            throw new ArgumentException("There was no group with label: " + groupLabel);
        }

        indexList.Add(socketIndex);

        // loop through socket indexes in group, find min and max x values
        var minX = double.MaxValue;
        var maxX = double.MinValue;
        foreach (var idx in indexList)
        {
            var pos = _socketPoints[idx];
            if (pos.X < minX)
                minX = pos.X;

            if (pos.X > maxX)
                maxX = pos.X;
        }

        var bracketStartX = minX - 10.0d; // arbitrary bracket extent
        var bracketEndX = maxX + 10.0d;

        var socket = _socketObjects[socketIndex];

        //var bracketPos = Position + new Vector2(0, GroupBracketDistFromSocketCenter);
        var labelTextHeight = 5.0f;
        var bracketHeight = 1.0d;
        var bracket = _groupingBracketObjects[groupLabel];
        var bracketX = (bracketEndX - bracketStartX) / 2 + bracketStartX + Position.X;
        var bracketY = socket.Position.Y - GroupBracketDistFromSocketCenter + bracketHeight/2 + Position.Y;
        var bracketPos = new Vector2(bracketX, bracketY);
        bracket.SetPosition(bracketPos, true);
        bracket.SetLabelPosition(new Vector2(0, -LabelDistanceFromBracket - labelTextHeight), true); // not nice setting the label position this way (same as socket label)
        bracket.SetLabelTextHeight(labelTextHeight);
        bracket.SetSize(bracketEndX - bracketStartX, bracketHeight);

        Debug.WriteLine($"Bracket for {groupLabel} now has Size: {bracket.Size} @ position: {bracket.Position}");
    }

    public void SetFixingHoles(List<Hole> fixingHoles)
    {
        // check all holes fit on the panel

        _fixingHoles = fixingHoles;
    }

    public override bool Draw(DrawingEntities drawing)
    {
        DrawOutline(drawing, AciColor.LightGray);

        // draw fixing holes
        foreach (var fixingHole in _fixingHoles)
        {
            fixingHole.Draw(drawing);
        }

        // draw the header label
        var absPosition = VecHelper.FlipYAxis(Label.Position);  // for now we have to flip every Y axis value when drawing
        var yShiftText = new Vector2(0, -Label.Height); // shift the label by its height to be at expected position
        var panelHeader = new Text(Label.Text, absPosition, Label.Height);
        panelHeader.Style = new TextStyle("Arial", FontStyle.Bold);
        if (Label.AlignTextCenter)
            panelHeader.Alignment = TextAlignment.BaselineCenter;

        //var panelHeader = new MText(_label.Text, absPosition, _label.Height);
        //panelHeader.Style.FontStyle = FontStyle.Italic;

        //if (_label.AlignTextCenter)
        //{
        //    var formatting = new MTextFormattingOptions();
        //    formatting.Bold = true;
        //}

        drawing.Add(panelHeader);

        // draw the center point for each socket location
        for (int i = 1; i <= _socketPoints.Count; i++)
        {
            var position = _socketPoints[i]; // index should exist as it is checked on the way in
            position = VecHelper.FlipYAxis(position);
            var xVert = new netDxf.Entities.Line(new Vector2(position.X, position.Y - 1.5), new Vector2(position.X, position.Y + 1.5));
            xVert.Color = AciColor.Red;
            var xHori = new netDxf.Entities.Line(new Vector2(position.X - 1.5, position.Y), new Vector2(position.X + 1.5, position.Y));
            xHori.Color = AciColor.Red;

            drawing.Add(xVert);
            drawing.Add(xHori);
        }

        // draw the socket holes for each filled socket
        // loop through all socket objects and draw them
        for (int i = 1; i <= _socketObjects.Count; i++)
        {
            var s = _socketObjects[i];
            s.Draw(drawing);
        }

        foreach (var bracket in _groupingBracketObjects)
        {
            bracket.Value.Draw(drawing);
        }

        return true;
    }

    protected override void DrawOutline(DrawingEntities drawing, AciColor color)
    {
        base.DrawOutline(drawing, color);

        var vertices = GetDwgVertices();
        //vertices = VecHelper.FlipYAxis(vertices);

        var topLeft = vertices[0];
        var topRight = vertices[1];
        var bottomRight = vertices[2];
        var bottomLeft = vertices[3];

        var topStart = new Vector2(topLeft.X + CornerFilletRadius, topLeft.Y);
        var topEnd = new Vector2(topRight.X - CornerFilletRadius, topRight.Y);

        var rightStart = new Vector2(topRight.X, topRight.Y - CornerFilletRadius);
        var rightEnd = new Vector2(bottomRight.X, bottomRight.Y + CornerFilletRadius);
        
        var bottomStart = new Vector2(bottomLeft.X + CornerFilletRadius, bottomLeft.Y);
        var bottomEnd = new Vector2(bottomRight.X - CornerFilletRadius, bottomRight.Y);
        
        var leftStart = new Vector2(topLeft.X, topLeft.Y - CornerFilletRadius);
        var leftEnd = new Vector2(bottomLeft.X, bottomLeft.Y + CornerFilletRadius);

        var top = new netDxf.Entities.Line(topStart, topEnd);
        var right = new netDxf.Entities.Line(rightStart, rightEnd);
        var bottom = new netDxf.Entities.Line(bottomStart, bottomEnd);
        var left = new netDxf.Entities.Line(leftStart, leftEnd);

        var actualOutlineColor = AciColor.Yellow;

        top.Color = actualOutlineColor;
        right.Color = actualOutlineColor;
        bottom.Color = actualOutlineColor;
        left.Color = actualOutlineColor;

        var filletCornerTopLeftPos = new Vector2(topLeft.X + CornerFilletRadius, topLeft.Y - CornerFilletRadius);
        var filletCornerTopRightPos = new Vector2(topRight.X - CornerFilletRadius, topRight.Y - CornerFilletRadius);
        var filletCornerBottomRightPos = new Vector2(bottomRight.X - CornerFilletRadius, bottomRight.Y + CornerFilletRadius);
        var filletCornerBottomLeftPos = new Vector2(bottomLeft.X + CornerFilletRadius, bottomLeft.Y + CornerFilletRadius);

        var cornerTopLeft = new Arc(filletCornerTopLeftPos, CornerFilletRadius, 90, 180);
        var cornerTopRight = new Arc(filletCornerTopRightPos, CornerFilletRadius, 0, 90);
        var cornerBottomRight = new Arc(filletCornerBottomRightPos, CornerFilletRadius, 270, 0);
        var cornerBottomLeft = new Arc(filletCornerBottomLeftPos, CornerFilletRadius, 180, 270);

        cornerTopLeft.Color = actualOutlineColor;
        cornerTopRight.Color = actualOutlineColor;
        cornerBottomRight.Color = actualOutlineColor;
        cornerBottomLeft.Color = actualOutlineColor;

        drawing.Add(top);
        drawing.Add(bottom);
        drawing.Add(left);
        drawing.Add(right);

        drawing.Add(cornerTopLeft);
        drawing.Add(cornerTopRight);
        drawing.Add(cornerBottomRight);
        drawing.Add(cornerBottomLeft);
    }
}
