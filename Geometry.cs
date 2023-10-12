using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1
{
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
        public static bool IsPointInsidePolygon(PointF[] poly, PointF p)
        {
            PointF p1, p2;
            bool inside = false;

            if (poly.Length < 3)
            {
                return inside;
            }

            var oldPoint = new PointF(
                poly[poly.Length - 1].X, poly[poly.Length - 1].Y);

            for (int i = 0; i < poly.Length; i++)
            {
                var newPoint = new PointF(poly[i].X, poly[i].Y);

                if (newPoint.X > oldPoint.X)
                {
                    p1 = oldPoint;
                    p2 = newPoint;
                }
                else
                {
                    p1 = newPoint;
                    p2 = oldPoint;
                }

                if ((newPoint.X < p.X) == (p.X <= oldPoint.X)
                    && (p.Y - (long)p1.Y) * (p2.X - p1.X)
                    < (p2.Y - (long)p1.Y) * (p.X - p1.X))
                {
                    inside = !inside;
                }

                oldPoint = newPoint;
            }

            return inside;
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

            int n = points.Count;

            for (int i = 0; i < n; i++)
            {
                var v1 = NormalizeVector(points[i].X - points[(i+1)%n].X, points[i].Y - points[(i+1)%n].Y);
                var v2 = NormalizeVector(points[(i+2)%n].X - points[(i + 1) % n].X, points[(i+2)%n].Y - points[(i + 1) % n].Y);
                var x = v1.x+v2.x;
                var y = v1.y+v2.y;
                var v3 = NormalizeVector(x, y, offset);
                boundingPoints.Add(new PointF(points[(i+1)%n].X - (float)v3.x, points[(i + 1) % n].Y - (float)v3.y));
            }

            return boundingPoints;
        }
        public static (double x, double y) NormalizeVector(double x, double y, int n=1)
        {
            double length = Math.Sqrt(x * x + y * y);

            if (length == 0)
            {
                // Jeśli długość wektora wynosi 0 (wektor zerowy), zwracamy (0, 0).
                return (0, 0);
            }

            double normalizedX = n * x / length;
            double normalizedY = n * y / length;

            return (normalizedX, normalizedY);
        }

    }
}
