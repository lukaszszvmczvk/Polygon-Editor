using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace lab1
{
    public class Line
    {
        public float A;
        public float B;
        public float C;

        public Line(float a, float b, float c)
        {
            this.A = a;
            this.B = b;
            this.C = c;
        }

        public override bool Equals(object? obj)
        {
            return obj is Line line &&
                   A == line.A &&
                   B == line.B &&
                   C == line.C;
        }
    }
    public static class Geometry
    {
        private static int radius = 5;
        public static double FindDistanceToSegment(PointF pt, PointF p1, PointF p2, out PointF closest)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            float t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) /
                (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                closest = new PointF(p1.X, p1.Y);
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                closest = new PointF(p2.X, p2.Y);
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                closest = new PointF(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }
        public static bool IsPointInsidePolygon(PointF[] polygon, PointF testPoint)
        {
            bool result = false;
            int j = polygon.Length - 1;
            for (int i = 0; i < polygon.Length; i++)
            {
                if (polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y ||
                    polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y)
                {
                    if (polygon[i].X + (testPoint.Y - polygon[i].Y) /
                       (polygon[j].Y - polygon[i].Y) *
                       (polygon[j].X - polygon[i].X) < testPoint.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }
        public static bool IsPointClicked(MouseEventArgs e, PointF p)
        {
            int X = e.X;
            int Y = e.Y;
            return Math.Abs(X - p.X) <= radius && Math.Abs(Y - p.Y) <= radius;
        }
        public static bool IsEdgeClicked(MouseEventArgs e, Edge edge)
        {
            var closest = new PointF();
            return FindDistanceToSegment(new PointF(e.X, e.Y), edge.p1, edge.p2, out closest) <= radius;
        }
        public static List<PointF> CreateBoundedPolygon(List<PointF> points, int offset)
        {
            List<PointF> boundingPoints = new List<PointF>();
            Line[] lines = new Line[points.Count];
            int n = points.Count;
            points = OrderPointList(points);

            for (int i = 0; i < n; i++)
            {
                var segmentLine = FindlLineEquation(points[i], points[(i + 1) % n]);
                var offsetLines = FindParallelLines(points[(i + 1) % n], points[(i + 2) % n],offset);
                var offsetLine = offsetLines.l1;
                var intersectionPoint = FindIntersection(offsetLine,segmentLine);
                if (Math.Sign(points[(i + 1) % n].X - points[i].X) == Math.Sign(points[(i + 1) % n].X - intersectionPoint.X) &&
                    Math.Sign(points[(i + 1) % n].Y - points[i].Y) == Math.Sign(points[(i + 1) % n].Y - intersectionPoint.Y))
                {
                    offsetLine = offsetLines.l2;
                }
                var orientation = Orientation(points[i], points[(i + 1) % n], points[(i + 2) % n]);
                if (orientation == 2 || orientation == 0)
                {
                    if (offsetLine == offsetLines.l1)
                        offsetLine = offsetLines.l2;
                    else
                        offsetLine = offsetLines.l1;
                }
                lines[i] = new Line(offsetLine.A, offsetLine.B, offsetLine.C);
            }
            for (int i = 0; i < lines.Length; ++i)
            {
                var intersectionPoint = FindIntersection(lines[i], lines[(i+1)%n]);
                boundingPoints.Add(new PointF(intersectionPoint.X, intersectionPoint.Y));
            }
            return boundingPoints;
        }
        public static Line FindlLineEquation(PointF p1, PointF p2)
        {
            var A = p1.Y - p2.Y;
            var B = p2.X - p1.X;
            var C = (p1.X - p2.X) * p1.Y + (p2.Y - p1.Y) * p1.X;
            return new Line(A, B, C);
        }
        public static PointF FindIntersection(Line l1, Line l2)
        {
            (var x, var y) = ((l1.B * l2.C - l2.B * l1.C) / (l1.A * l2.B - l2.A * l1.B), (l1.C * l2.A - l2.C * l1.A) / (l1.A * l2.B - l2.A * l1.B));
            return new PointF(x, y);
        }
        public static (Line l1, Line l2) FindParallelLines(PointF p1, PointF p2, int offset)
        {
            if(p2.X == p1.X)
            {
                var l1 = FindlLineEquation(new PointF(p1.X - offset, p1.Y), new PointF(p2.X - offset, p2.Y));
                var l2 = FindlLineEquation(new PointF(p1.X + offset, p1.Y), new PointF(p2.X + offset, p2.Y));
                return (l1, l2);
            }
            else if(p2.Y == p1.Y)
            {
                var l1 = FindlLineEquation(new PointF(p1.X, p1.Y - offset), new PointF(p2.X, p2.Y - offset));
                var l2 = FindlLineEquation(new PointF(p1.X, p1.Y + offset), new PointF(p2.X, p2.Y + offset));
                return (l1, l2);

            }
            var m = (p2.Y - p1.Y) / (p2.X-p1.X);

            var x1 = Math.Abs(offset * m) / Math.Sqrt(1 + m * m) + p1.X;
            var x2 = -Math.Abs(offset * m) / Math.Sqrt(1 + m * m) + p1.X;
            var y1 = -(x1 - p1.X) / m + p1.Y;
            var y2 = -(x2 - p1.X) / m + p1.Y;
            var cp1 = new PointF((float)x1, (float)y1);
            var cp2 = new PointF((float)x2, (float)y2);

            x1 = Math.Abs(offset * m) / Math.Sqrt(1 + m * m) + p2.X;
            x2 = -Math.Abs(offset * m) / Math.Sqrt(1 + m * m) + p2.X;
            y1 = -(x1 - p2.X) / m + p2.Y;
            y2 = -(x2 - p2.X) / m + p2.Y;
            var cp3 = new PointF((float)x1, (float)y1);
            var cp4 = new PointF((float)x2, (float)y2);

            return (FindlLineEquation(cp1, cp3), FindlLineEquation(cp2, cp4));
        }
        public static List<PointF> OrderPointList(List<PointF> points)
        {
            int n = points.Count;
            var point = points.OrderByDescending(p => p.Y).ThenBy(p=> p.X).First();

            var index = points.FindIndex(p => p == point);
            var n1 = points[Utils.mod(index - 1, n)];
            var n2 = points[Utils.mod(index + 1, n)];
            var orientation = Orientation(n1, point, n2);
            if (orientation == 2)
                points.Reverse();

            return points;
        }
        public static int Orientation(PointF p1, PointF p2, PointF p3)
        {
            int val = (int)((p2.Y - p1.Y) * (p3.X - p2.X)-(p2.X - p1.X) * (p3.Y - p2.Y));

            if (val == 0)
                return 0; 

            return (val > 0) ? 1 : 2; // clock or counterclock wise
        }
    }

}
