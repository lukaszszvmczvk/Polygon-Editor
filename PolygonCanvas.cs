using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace lab1
{
    public enum CanvasMode
    {
        Add,
        Move,
        Delete,
        Border,
        Circle
    }
    public enum EdgeOrientation
    {
        None,
        Horizontal,
        Vertical
    }
    public class Edge
    {
        public PointF p1 { get; set; }
        public PointF p2 { get; set; }
        public Edge(PointF p1, PointF p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }
    }
    public class Polygon
    {
        public bool ShowBorder { get; set; }
        public List<PointF> Points { get; set; }
        public List<PointF> BorderPoints { get; set; }
        public EdgeOrientation[] EdgeRelations { get; set; }
        public Polygon(List<PointF> points)
        {
            Points = points;
            ShowBorder = false;
            EdgeRelations = new EdgeOrientation[points.Count];
            BorderPoints = new List<PointF>();
        }
    }
    public class Circle
    {
        public int Radius { get; set; }
        public Point Center { get; set; }
        public Circle(int radius, Point center)
        {
            Radius = radius;
            Center = center;
        }
    }
    public class PolygonCanvas
    {
        /// <summary>
        /// Non setable variables
        /// </summary>
        private PictureBox Canvas;
        private ContextMenuStrip MenuStrip;
        private const int radius = 5;

        /// <summary>
        /// Setable variables
        /// </summary>
        public CanvasMode Mode { get; set; }
        public List<PointF> Points { get; set; }
        public List<Polygon> Polygons { get; set; }
        public int Offset { get; set; }
        public bool UseBresenham { get; set; }
        public List<Circle> Circles { get; set; }
        public int CircleRadius { get; set; }

        /// <summary>
        ///  Edit mode variables
        /// </summary>
        private Polygon? selectedPolygon = null;
        private PointF? previousPoint = null;
        private int selectedPointIndex = -1;
        private int selectedEdgeStartIndex = -1;
        private Circle? selectedCircle = null;
        private PointF currentPosition;

        /// <summary>
        ///  Relations variables
        /// </summary>
        private Polygon? polygonToSetRelation = null;
        private int edgeIndexToSetRelation = -1;

        public PolygonCanvas(CanvasMode mode, List<PointF> points, PictureBox canvas, List<Polygon> polygons, ContextMenuStrip menuStrip)
        {
            this.Mode = mode;
            this.Points = points;
            this.Canvas = canvas;
            this.Canvas.MouseDown += this.canvasMouseDown;
            this.Canvas.MouseUp += this.canvasMouseUp;
            this.Canvas.MouseMove += this.canvasMouseMove;
            Polygons = polygons;
            this.MenuStrip = menuStrip;
            currentPosition = new PointF(0, 0);
            Circles = new List<Circle>();
            DefineStartPolygons();
        }
        private void DefineStartPolygons()
        {
            Polygons = new List<Polygon>();
            Polygons.Add(new Polygon(new List<PointF>
            {
               new PointF(100, 100),
               new PointF(100, 300),
               new PointF(400,300)
            }));
            Polygons[0].EdgeRelations[0] = EdgeOrientation.Vertical;
            Polygons[0].EdgeRelations[1] = EdgeOrientation.Horizontal;


            Polygons.Add(new Polygon(new List<PointF>
            {
               new PointF(600, 200),
               new PointF(600, 400),
               new PointF(500, 400),
               new PointF(500,200)
            }));
            Polygons[1].EdgeRelations[0] = EdgeOrientation.Vertical;
            Polygons[1].EdgeRelations[1] = EdgeOrientation.Horizontal;
            Polygons[1].EdgeRelations[2] = EdgeOrientation.Vertical;
            Polygons[1].EdgeRelations[3] = EdgeOrientation.Horizontal;

            DrawPolygons();
        }
        public void canvasMouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                switch(Mode)
                {
                    case CanvasMode.Add:
                        AddPoint(e);
                        break;
                    case CanvasMode.Move:
                        if(!SelectCircle(e))
                            if(!SelectPointOrEdge(e))
                                SelectPolygon(e);
                        previousPoint = new PointF(e.X, e.Y);
                        break;
                    case CanvasMode.Delete:
                        if (!DeletePointOrEdge(e))
                            DeletePolygon(e);
                        break;
                    case CanvasMode.Border:
                        if(SelectPolygon(e))
                        {
                            var poly = selectedPolygon;
                            if(poly.ShowBorder || poly.Points.Count<3)
                                poly.ShowBorder = false;
                            else
                                poly.ShowBorder = true;
                            DrawPolygons();
                        }
                        break;
                    case CanvasMode.Circle:
                        AddCircle(e);
                        DrawPolygons();
                        break;
                }
            }
            else if(e.Button == MouseButtons.Right)
            {
                if(SelectPointOrEdge(e))
                {
                    if(selectedEdgeStartIndex != -1)
                    {
                        MenuStrip.Show(Canvas.PointToScreen(e.Location));
                        edgeIndexToSetRelation = selectedEdgeStartIndex;
                        polygonToSetRelation = selectedPolygon;
                    }
                }
            }
        }
        private bool SelectCircle(MouseEventArgs e)
        {
            foreach(var circle in Circles)
            {
                var distance = Math.Sqrt((e.X - circle.Center.X) * (e.X - circle.Center.X) + (e.Y - circle.Center.Y) * (e.Y - circle.Center.Y));
                if (distance<=circle.Radius)
                {
                    selectedCircle = circle;
                    return true;
                }
            }
            return false;
        }
        private void AddCircle(MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);
            Circles.Add(new Circle(CircleRadius, p));
        }
        public void canvasMouseUp(object sender, MouseEventArgs e)
        {
            selectedPolygon = null;
            selectedCircle = null;
            selectedEdgeStartIndex = -1;
            selectedPointIndex = -1;
        }
        public void canvasMouseMove(object sender, MouseEventArgs e)
        {
            currentPosition = new PointF(e.X, e.Y);
            if (Mode == CanvasMode.Move && (selectedPolygon != null || selectedCircle != null))
            {
                (var X, var Y) = (e.X - previousPoint.Value.X, e.Y - previousPoint.Value.Y);
                previousPoint = new PointF(e.X, e.Y);
                if(selectedCircle!= null)
                {
                    selectedCircle.Center = new Point(selectedCircle.Center.X+(int)X, selectedCircle.Center.Y+(int)Y);
                }
                else if (selectedPointIndex != -1)
                {
                    int i = selectedPointIndex;
                    ChangePointCoordinates(i, X, Y);
                    if (selectedPolygon.EdgeRelations[i] == EdgeOrientation.Horizontal)
                        ChangePointCoordinates(i + 1, 0, Y);
                    if (selectedPolygon.EdgeRelations[i] == EdgeOrientation.Vertical)
                        ChangePointCoordinates(i + 1, X, 0);
                    if (selectedPolygon.EdgeRelations[Utils.mod(i - 1, selectedPolygon.Points.Count)] == EdgeOrientation.Horizontal)
                        ChangePointCoordinates(i - 1, 0, Y);
                    if (selectedPolygon.EdgeRelations[Utils.mod(i - 1, selectedPolygon.Points.Count)] == EdgeOrientation.Vertical)
                        ChangePointCoordinates(i - 1, X, 0);
                }
                else if (selectedEdgeStartIndex != -1)
                {
                    var counter = 0;
                    for (int j = selectedEdgeStartIndex; j <= selectedEdgeStartIndex + 1; ++j)
                    {
                        int i = Utils.mod(j, selectedPolygon.Points.Count);
                        ++counter;
                        ChangePointCoordinates(i, X, Y);
                        if (selectedPolygon.EdgeRelations[i] == EdgeOrientation.Horizontal && counter == 2)
                            ChangePointCoordinates(i + 1, 0, Y);
                        if (selectedPolygon.EdgeRelations[i] == EdgeOrientation.Vertical && counter == 2)
                            ChangePointCoordinates(i + 1, X, 0);
                        if (selectedPolygon.EdgeRelations[Utils.mod(i - 1, selectedPolygon.Points.Count)] == EdgeOrientation.Horizontal && counter == 1)
                            ChangePointCoordinates(i - 1, 0, Y);
                        if (selectedPolygon.EdgeRelations[Utils.mod(i - 1, selectedPolygon.Points.Count)] == EdgeOrientation.Vertical && counter == 1)
                            ChangePointCoordinates(i - 1, X, 0);
                    }
                }
                else
                    selectedPolygon.Points = selectedPolygon.Points.Select((p) => new PointF(p.X + X, p.Y + Y)).ToList();
                DrawPolygons();
            }
            else if (Mode == CanvasMode.Add)
                DrawPolygons();
        }
        private void AddPoint(MouseEventArgs e)
        {
            if(Points.Count == 0 && SelectPointOrEdge(e))
            {
                if(selectedEdgeStartIndex != -1)
                {
                    int i = selectedEdgeStartIndex;
                    selectedPolygon.Points.Insert(i + 1, new PointF(e.X, e.Y));
                    selectedPolygon.EdgeRelations = new EdgeOrientation[selectedPolygon.Points.Count];
                    DrawPolygons();
                }
                return;
            }
            if (CheckIfPolygonIsCrated(e))
                return;
            Points.Add(new PointF(e.X, e.Y));
            DrawPolygons();
        }
        private bool CheckIfPolygonIsCrated(MouseEventArgs e)
        {
            if(Points.Count<2)
                return false;
            
            if (Geometry.IsPointClicked(e, Points[0]))
            {
                SolidBrush brush = new SolidBrush(Color.FromArgb(128, 0, 0, 0));
                Polygons.Add(new Polygon(Points));
                Points = new List<PointF>();
                DrawPolygons();
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool SelectPointOrEdge(MouseEventArgs e)
        {
            var point = new PointF(e.X, e.Y);
            foreach (var poly in Polygons)
            {
                var n = poly.Points.Count;
                for(int j = 0; j < n; ++j)
                {
                    if(Geometry.IsEdgeClicked(e,new Edge(poly.Points[j], poly.Points[Utils.mod(j - 1, n)])))
                    {
                        selectedPolygon = poly;

                        if (Geometry.IsPointClicked(e, poly.Points[Utils.mod(j - 1, n)]))
                            selectedPointIndex = Utils.mod(j - 1, n);
                        else if (Geometry.IsPointClicked(e, poly.Points[j]))
                            selectedPointIndex = j;
                        else
                            selectedEdgeStartIndex = Utils.mod(j - 1, n);
                        return true;
                    }
                }
            }
            return false;
        }
        private bool SelectPolygon(MouseEventArgs e)
        {
            for (int i = 0; i < Polygons.Count; ++i)
            {
                if (Geometry.IsPointInsidePolygon(Polygons[i].Points.ToArray(), new PointF(e.X, e.Y)))
                {
                    selectedPolygon = Polygons[i];
                    return true;
                }
            }
            return false;
        }
        private bool DeletePointOrEdge(MouseEventArgs e)
        {
            foreach(var poly in Polygons)
            {
                for(int i=0; i<poly.Points.Count; ++i)
                {
                    if (Geometry.IsEdgeClicked(e, new Edge(poly.Points[i], poly.Points[Utils.mod(i - 1, poly.Points.Count)])))
                    {
                        if (Geometry.IsPointClicked(e, poly.Points[Utils.mod(i - 1, poly.Points.Count)]))
                            poly.Points.RemoveAt(Utils.mod(i - 1, poly.Points.Count));
                        else if (Geometry.IsPointClicked(e, poly.Points[i]))
                            poly.Points.RemoveAt(i);
                        else
                        {
                            poly.Points.RemoveAt(i);
                            poly.Points.RemoveAt(Utils.mod(i - 1, poly.Points.Count));
                        }
                        poly.EdgeRelations = new EdgeOrientation[poly.Points.Count];
                        CheckConditionsAndDraw(poly);
                        return true;
                    }
                }
            }
            return false;
        }
        private void DeletePolygon(MouseEventArgs e)
        {
            var p = new PointF(e.X, e.Y);
            foreach (var poly in Polygons)
            {
                if (Geometry.IsPointInsidePolygon(poly.Points.ToArray(), p))
                {
                    Polygons.Remove(poly);
                    DrawPolygons();
                    return;
                }
            }
        }
        private void DrawLine(PointF p1, PointF p2, Bitmap canvas, Pen pen)
        {
            if (UseBresenham)
                Bresenham(p1, p2, canvas,pen.Color);
            else
            {
                using (var graphics = Graphics.FromImage(canvas))
                {
                    graphics.DrawLine(pen, p1.X, p1.Y, p2.X, p2.Y);
                }
            }
        }

        // Source: https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
        private void Bresenham(PointF p1, PointF p2, Bitmap canvas, Color color)
        {
            var x0 = (int)p1.X; var y0 = (int)p1.Y;
            var x1 = (int)p2.X; var y1 = (int)p2.Y;

            if(Math.Abs(y1 - y0) < Math.Abs(x1-x0))
            {
                if (x0 > x1)
                    plotLineLow(x1, y1, x0, y0, canvas,color);
                else
                    plotLineLow(x0, y0, x1, y1, canvas, color);
            }
            else
            {
                if(y0>y1)
                    plotLineHigh(x1,y1,x0,y0, canvas, color);
                else
                    plotLineHigh(x0, y0, x1,y1, canvas, color);
            }
        }
        private void plotLineLow(int x0, int y0, int x1, int y1, Bitmap canvas, Color color)
        {
            var dx = x1 - x0;
            var dy = y1 - y0;
            var yi = 1;

            if(dy<0)
            {
                yi = -1;
                dy = -dy;
            }
            var D = 2 * dy - dx;
            var y = y0;
            for(int x=x0; x<=x1; ++x)
            {
                canvas.SetPixel(x, y, color);
                if(D>0)
                {
                    y += yi;
                    D += 2 * (dy - dx);
                }
                else
                {
                    D += 2 * dy;
                }
            }
        }
        private void plotLineHigh(int x0, int y0, int x1, int y1, Bitmap canvas, Color color)
        {
            var dx = x1 - x0;
            var dy = y1 - y0;
            var xi = 1;

            if (dx < 0)
            {
                xi = -1;
                dx = -dx;
            }
            var D = 2 * dx - dy;
            var x = x0;
            for (int y = y0; y <= y1; ++y)
            {
                canvas.SetPixel(x, y, color);
                if (D > 0)
                {
                    x += xi;
                    D += 2 * (dx - dy);
                }
                else
                {
                    D += 2 * dx;
                }
            }
        }
        public void SetEgdeRelation(EdgeOrientation orientation)
        {
            var poly = polygonToSetRelation;
            var n = poly.Points.Count;
            for(int j = edgeIndexToSetRelation; j <= edgeIndexToSetRelation+1; j++)
            {
                int i = Utils.mod(j, polygonToSetRelation.Points.Count);
                if (poly.EdgeRelations[i] == orientation)
                    {
                        poly.EdgeRelations[i] = EdgeOrientation.None;
                        break;
                    }
                if (poly.EdgeRelations[Utils.mod(i - 1, n)] == orientation || poly.EdgeRelations[(i + 1) % n] == orientation)
                    break;
                var id1 = i;
                var id2 = (i + 1) % n;
                if (poly.Points[id1].Y < poly.Points[id2].Y)
                    (id1, id2) = (id2, id1);
                if(orientation == EdgeOrientation.Horizontal)
                {
                    poly.Points[id2] = new PointF(poly.Points[id2].X, poly.Points[id1].Y);
                }
                else
                {
                    poly.Points[id2] = new PointF(poly.Points[id1].X, poly.Points[id2].Y);
                }
                poly.EdgeRelations[i] = orientation;
            }
            DrawPolygons();
        }
        // Musiałem tylko zmienić kolor tła panelu, na którym kolorowałem
        private void DrawCircle(Circle circle, Color color, Bitmap canvas)
        {
            var x0 = circle.Center.X;
            var y0 = circle.Center.Y;
            int R = circle.Radius;
            int x = R;
            int y = 0;
            float T = 0;
            var I = 255;

            List<int> list = new List<int>();

            List<Point> circlePoints = new List<Point>();
            circlePoints.Add(new Point(x, y));
            circlePoints.Add(new Point(y, x));

            circlePoints.Add(new Point(-x, y));
            circlePoints.Add(new Point(y, -x));

            circlePoints.Add(new Point(x, -y));
            circlePoints.Add(new Point(-y, x));

            circlePoints.Add(new Point(-x, -y));
            circlePoints.Add(new Point(-y, -x));

            list.Add((int)(I * (1 - D(R, y))));
            list.Add((int)(I * (1 - D(R, y))));
            list.Add((int)(I * (1 - D(R, y))));
            list.Add((int)(I * (1 - D(R, y))));
            list.Add((int)(I * (1 - D(R, y))));
            list.Add((int)(I * (1 - D(R, y))));
            list.Add((int)(I * (1 - D(R, y))));
            list.Add((int)(I * (1 - D(R, y))));

            while (x>y)
            {
                y++;
                if (D(R, y) < T)
                    x--;
                circlePoints.Add(new Point(x, y));
                circlePoints.Add(new Point(y, x));

                circlePoints.Add(new Point(-x, y));
                circlePoints.Add(new Point(y, -x));

                circlePoints.Add(new Point(x, -y));
                circlePoints.Add(new Point(-y, x));

                circlePoints.Add(new Point(-x, -y));
                circlePoints.Add(new Point(-y, -x));

                list.Add((int)(I * (1 - D(R, y))));
                list.Add((int)(I * (1 - D(R, y))));
                list.Add((int)(I * (1 - D(R, y))));
                list.Add((int)(I * (1 - D(R, y))));
                list.Add((int)(I * (1 - D(R, y))));
                list.Add((int)(I * (1 - D(R, y))));
                list.Add((int)(I * (1 - D(R, y))));
                list.Add((int)(I * (1 - D(R, y))));

                circlePoints.Add(new Point(x-1, y));
                circlePoints.Add(new Point(y, x-1));

                circlePoints.Add(new Point(-(x-1), y));
                circlePoints.Add(new Point(y, -(x-1)));

                circlePoints.Add(new Point(x-1, -y));
                circlePoints.Add(new Point(-y, x-1));

                circlePoints.Add(new Point(-(x - 1), -y));
                circlePoints.Add(new Point(-y, -(x - 1)));

                list.Add((int)(I * D(R, y)));
                list.Add((int)(I * D(R, y)));
                list.Add((int)(I * D(R, y)));
                list.Add((int)(I * D(R, y)));
                list.Add((int)(I * D(R, y)));
                list.Add((int)(I * D(R, y)));
                list.Add((int)(I * D(R, y)));
                list.Add((int)(I * D(R, y)));

                T = (float)D(R, y);
            }

            for(int i=0; i<circlePoints.Count; ++i)
            {
                var pt = circlePoints[i];
                using (Graphics g = Graphics.FromImage(canvas))
                {
                    Brush brush = new SolidBrush(Color.FromArgb(I-list[i], I-list[i],I - list[i]));
                    g.FillRectangle(brush, pt.X + circle.Center.X, pt.Y + circle.Center.Y, 1, 1);
                }
            }
        }
        private double D(int R, int y)
        {
            return Math.Ceiling(Math.Sqrt(R * R - y * y)) - Math.Sqrt(R * R - y * y);
        }
        public void DrawPolygons()
        {
            var newCanvas = new Bitmap(Canvas.Size.Width, Canvas.Size.Height);
            SolidBrush brush = new SolidBrush(Color.FromArgb(128, 0, 0, 0));
            Pen redPen = new Pen(Color.Red, 2);
            Pen orangePen = new Pen(Color.Orange, 2);
            Pen blackPen = new Pen(Color.Black, 2);
            if(Points.Count > 0)
            {
                for(int i=0; i<Points.Count; i++)
                {
                    using (Graphics g = Graphics.FromImage(newCanvas))
                    {
                        g.FillEllipse(Brushes.Black, Points[i].X - radius, Points[i].Y - radius, radius * 2, radius * 2);
                        if(i > 0)
                            DrawLine(Points[i], Points[i-1], newCanvas,blackPen);
                    }
                }
                if(Mode == CanvasMode.Add)
                    DrawLine(currentPosition, Points[Points.Count-1], newCanvas, blackPen);
            }
            foreach (var poly in Polygons)
            {
                for(int i=0; i<poly.Points.Count; ++i)
                {
                    var point = poly.Points[i];
                    using (Graphics g = Graphics.FromImage(newCanvas))
                    {
                        g.FillEllipse(Brushes.Black, point.X - radius, point.Y - radius, radius * 2, radius * 2);
                        DrawLine(poly.Points[Utils.mod(i-1,poly.Points.Count)], point, newCanvas,blackPen);
                        var x = (point.X + poly.Points[Utils.mod(i + 1, poly.Points.Count)].X) / 2;
                        var y = (point.Y + poly.Points[Utils.mod(i + 1, poly.Points.Count)].Y) / 2;
                        if (poly.EdgeRelations[i] == EdgeOrientation.Horizontal)
                        {
                            g.DrawRectangle(orangePen, x, y+2*radius, 15, 2);
                        }
                        else if (poly.EdgeRelations[i] == EdgeOrientation.Vertical)
                        {
                            g.DrawRectangle(orangePen, x+2*radius, y, 2, 15);
                        }
                    }
                }
                using (Graphics g = Graphics.FromImage(newCanvas))
                {
                    g.FillPolygon(brush, poly.Points.ToArray());
                    if (poly.ShowBorder)
                    {
                        poly.BorderPoints = Geometry.CreateBoundedPolygon(poly, Offset);
                        var borderPoints = poly.BorderPoints.Select(x => new Point((int)x.X, (int)x.Y)).ToArray();
                        if (UseBresenham == false)
                        { 
                            g.DrawPolygon(redPen,borderPoints);
                        }
                        else
                        {
                            for(int i = 0; i < borderPoints.Length; i++)
                            {
                                DrawLine(borderPoints[i], borderPoints[(i + 1) % borderPoints.Length], newCanvas, redPen);
                            }
                        }
                    }
                }
            }
            foreach(var circle in Circles)
            {
                circle.Radius = CircleRadius;
                DrawCircle(circle, Color.Black, newCanvas);
            }
            Canvas.Image.Dispose();
            Canvas.Image = newCanvas;
            Canvas.Refresh();
        }
        private void CheckConditionsAndDraw(Polygon poly)
        {
            if (poly.Points.Count <= 1)
            {
                Polygons.Remove(poly);
                return;
            }
            if(poly.Points.Count < 3)
                poly.ShowBorder = false;
            DrawPolygons();
        }
        private void ChangePointCoordinates(int i, float X, float Y)
        {
            var id = Utils.mod(i, selectedPolygon.Points.Count);
            var p = selectedPolygon.Points[id];
            p.X += X;
            p.Y += Y;
            selectedPolygon.Points[id] = p;
        }
        public void Clear()
        {
            Canvas.Image.Dispose();
            Canvas.Image = new Bitmap(Canvas.Size.Width, Canvas.Size.Height);
            Polygons.Clear();
            Points.Clear();
            Circles.Clear();
            selectedPointIndex = -1;
            selectedPointIndex = -1;
            selectedPolygon = null;
            Mode = CanvasMode.Add;
        }
    }
}
