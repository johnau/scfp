
using netDxf;

namespace ExcelCableGeneratorApp.Dxf.Drawing.Element
{
    internal class Line : DrawingObject
    {
        public Vector2 DirectionAndMagnitude { get; private set; }
        public Vector2 End { get => Position + DirectionAndMagnitude; }
        public Vector2 Start { get => Position; }

        public Line(Vector2 start, Vector2 end) 
            : base("Line", 0, 0, 0, 0)
        {
            SetPosition(start);
            DirectionAndMagnitude = end - start;
        }

        public void SetLinePoints(Vector2 start, Vector2 end)
        {
            SetPosition(start);
            DirectionAndMagnitude = end - start;
        }
    }
}
