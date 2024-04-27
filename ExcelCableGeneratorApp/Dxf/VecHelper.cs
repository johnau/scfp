using ExcelCableGeneratorApp.Dxf.Drawing.Element;
using netDxf;
using System.ComponentModel;

namespace ExcelCableGeneratorApp.Dxf
{
    internal class VecHelper
    {
        public static List<Vector2> RotatePointsAroundPoint(List<Vector2> points, double degrees, Vector2 rotationCenter)
        {
            if (degrees == 0)
                return points;

            List<Vector2> rotated = [];

            // Rotate all vertices
            double angleRadians = MathHelper.DegToRad * degrees; // Convert 45 degrees to radians
            for (int i = 0; i < points.Count; i++)
            {
                var translatedVert = points[i] - rotationCenter;
                var rotatedVert = Vector2.Rotate(translatedVert, angleRadians);
                rotated.Add(rotatedVert + rotationCenter);
            }

            return rotated; ;
        }

        public static List<Vector2> FlipYAxis(List<Vector2> points)
        {
            List<Vector2> flipped = [];
            for (int i = 0; i < points.Count; i++)
            {
                flipped.Add(FlipYAxis(points[i]));
            }
            return flipped;
        }

        public static Vector2 FlipYAxis(Vector2 point)
        {
            return new Vector2(point.X, -point.Y);
        }

        public static bool CheckForIntersection(List<LwLine> lines1, List<LwLine> lines2)
        {
            for (int i = 0; i < lines1.Count; i++)
            {
                var _line = lines1[i];
                var lineStart = _line.Start;
                var lineEnd = _line.End;

                for (int i2 = 0; i2 < lines2.Count; i2++)
                {
                    var _line2 = lines2[i2];
                    var lineStart2 = _line2.Start;
                    var lineEnd2 = _line2.End;

                    var result = MathHelper.FindIntersection(lineStart, lineEnd - lineStart, lineStart2, lineEnd2 - lineStart2);

                    var inLine1 = MathHelper.PointInSegment(result, lineStart, lineEnd);
                    var inLine2 = MathHelper.PointInSegment(result, lineStart2, lineEnd2);

                    if (inLine1 == 0 && inLine2 == 0)
                        return true;
                }
            }

            return false;
        }

        public static List<LwLine> GetEdgesFromVertices(List<Vector2> vertices)
        {
            List<LwLine> edges = [];
            for (int i = 0; i < vertices.Count; i++)
            {
                Vector2 p1 = vertices[i];
                Vector2 p2 = vertices[(i + 1) % vertices.Count];
                edges.Add(new LwLine(p1, p2));
            }

            return edges;
        }

        public static bool CalculateMinTranslationVectorX(List<Vector2> fixedPos, List<Vector2> toTranslate, out Vector2 translateX)
        {
            translateX = Vector2.Zero;

            bool doPointsOverlap = false;
            if (IsAnyPointInsidePolygon(fixedPos, toTranslate))
                doPointsOverlap = true;

            if (IsAnyPointInsidePolygon(toTranslate, fixedPos))
                doPointsOverlap = true;

            if (!doPointsOverlap)
                return false;

            // polygons overlap in some way
            // calculate right most point of fixed poly
            var maxXPoint = GetPointMaxX(fixedPos);
            var minXPoint = GetPointMaxX(toTranslate);

            /** Handling Shifting of objects
             * For the time being, this behavior is written with the assumption that the object
             * being translated will be a rectangle/square, without any rotation.
             * 
             * The calculation is handled in a fairly naive way, the rect is either completely above
             * or completely below the obstruction's right most point, or it's edge is overlapping 
             * the point.  These 3 cases are handled discretely.
             */

            /** Case 1: The entire rect is below the right most point
             * Handle by finding the intersection and moving to that point
             */
            var maxY_tt = GetPointMaxXY(toTranslate);
            if (maxY_tt.Y < maxXPoint.Y)
            {
                Vector2 lowerRight = Vector2.Zero;
                for (int i = 0; i < fixedPos.Count; i++)
                {
                    var p = fixedPos[i];
                    if (p.Y < maxXPoint.Y && p.X > lowerRight.X)
                        lowerRight = p;
                }
                var lrLine = new LwLine(lowerRight, maxXPoint);
                var xProjection = new LwLine(maxY_tt, new Vector2(maxY_tt.X + 100d, maxY_tt.Y));
                var intersection = MathHelper.FindIntersection(xProjection.Start, xProjection.Direction, lrLine.Start, lrLine.Direction);

                translateX = new Vector2(intersection.X - maxY_tt.X, 0);

                return true;
            }

            /** Case 2: The entire rect is above the right most point
             * Handle by finding intersection and moving to that point
             */
            var minY_tt = GetPointMaxXMinY(toTranslate);
            if (minY_tt.Y > maxXPoint.Y)
            {
                Vector2 upperRight = Vector2.Zero;
                for (int i = 0; i < fixedPos.Count; i++)
                {
                    var p = fixedPos[i];
                    if (p.Y > maxXPoint.Y && p.X > upperRight.X)
                        upperRight = p;
                }
                var lrLine = new LwLine(upperRight, maxXPoint);

                var xProjection = new LwLine(minY_tt, new Vector2(minY_tt.X + 100d, minY_tt.Y));
                var intersection = MathHelper.FindIntersection(xProjection.Start, xProjection.Direction, lrLine.Start, lrLine.Direction);

                translateX = new Vector2(intersection.X - maxY_tt.X, 0);

                return true;
            }

            /** Case 3: The left edge or more is past the right most point
             * Handle by shifting the socket over to the right most point
             */

            var distance = maxXPoint.X - minXPoint.X;
            translateX = new Vector2(distance, 0);
            
            return true;



            //find intersection of top point

            // check if bottom most point of translating poly is above the right most point of fixed
            // -- if it is we can check for a closer intersection point on the other side

            // 



            // calculate left most point of translatable poly
            // calculate distance

            //foreach (Vector2 tVert in toTranslate)
            //{
            //    // work out distance of all verts to exit all edges of the polygon on x axis to the right
            //    var xRightDir = new Vector2(tVert.X + 100d, tVert.Y) - tVert;

            //    // x-axis line
            //    foreach (var el in GetEdgesFromVertices(fixedPos))
            //    {
            //        // direction and distance from vert
            //        var edgeDir = el.End - el.Start;
            //        //var vertDistToEdge = MathHelper.PointLineDistance(vert, el.Start, edgeDir);   
            //        var intersection = MathHelper.FindIntersection(tVert, xRightDir, el.Start, edgeDir);
            //        var dist = intersection.X - tVert.X;
            //        if (dist < minDistX) 
            //            minDistX = dist;
            //    }
            //}

            //return true;
        }

        public static Vector2 GetPointMaxX(List<Vector2> points)
        {
            var maxX = double.MinValue;
            var vec = Vector2.Zero;
            for (int i = 0; i < points.Count; i++) 
            {
                var p = points[i];
                if (p.X > maxX) 
                {
                    maxX = p.X;
                    vec = p;
                }
            }
            return vec;
        }

        public static Vector2 GetPointMinX(List<Vector2> points)
        {
            var minX = double.MaxValue;
            var vec = Vector2.Zero;
            for (int i = 0; (i < points.Count); i++)
            {
                var p = points[i];
                if (p.X < minX)
                {
                    minX = p.X;
                    vec = p;
                }
            }
            return vec;
        }

        public static Vector2 GetPointMaxY(List<Vector2> points)
        {
            var maxY = double.MinValue;
            var vec = Vector2.Zero;
            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];
                if (p.Y > maxY)
                {
                    maxY = p.Y;
                    vec = p;
                }
            }
            return vec;
        }

        public static Vector2 GetPointMaxXY(List<Vector2> points)
        {
            var maxX = double.MinValue;
            var maxY = double.MinValue;
            var vec = Vector2.Zero;
            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];
                if (p.X > maxX && p.Y > maxY)
                {
                    maxX = p.X;
                    maxY = p.Y;
                    vec = p;
                }
            }
            return vec;
        }

        public static Vector2 GetPointMinY(List<Vector2> points)
        {
            var minY = double.MaxValue;
            var vec = Vector2.Zero;
            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];
                if (p.Y < minY)
                {
                    minY = p.Y;
                    vec = p;
                }
            }
            return vec;
        }

        public static Vector2 GetPointMaxXMinY(List<Vector2> points)
        {
            var maxX = double.MinValue;
            var minY = double.MaxValue;
            var vec = Vector2.Zero;
            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];
                if (p.X > maxX && p.Y < minY)
                {
                    minY = p.Y;
                    vec = p;
                }
            }
            return vec;
        }

        public static bool PolygonContainsPolygon(List<Vector2> contains, List<Vector2> contained)
        {
            var count = 0;
            for (int i = 0; i < contained.Count; i++)
            {
                var point = contained[i];
                if (IsPointInsidePolygon(point, contains))
                    count++;
            }

            return count == contained.Count;
        }

        public static bool IsAnyPointInsidePolygon(List<Vector2> container, List<Vector2> points) 
        {
            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                if (IsPointInsidePolygon(point, container))
                    return true;
            }

            return false;
        }

        public static bool IsPointInsidePolygon(Vector2 point, List<Vector2> polygon)
        {
            int count = 0;
            int numVertices = polygon.Count;
            for (int i = 0; i < numVertices; i++)
            {
                Vector2 vertex1 = polygon[i];
                Vector2 vertex2 = polygon[(i + 1) % numVertices];

                if ((vertex1.Y <= point.Y && point.Y < vertex2.Y || vertex2.Y <= point.Y && point.Y < vertex1.Y) &&
                    point.X < (vertex2.X - vertex1.X) * (point.Y - vertex1.Y) / (vertex2.Y - vertex1.Y) + vertex1.X)
                {
                    count++;
                }
            }
            return count % 2 == 1;
        }

        public static List<Vector2> TranslateAll(List<Vector2> points, Vector2 translateVector)
        {
            List<Vector2> translated = [];
            for (int i = 0; i < points.Count; i++)
            {
                translated.Add(points[i] + translateVector);
            }

            return translated;
        }
    }
}
