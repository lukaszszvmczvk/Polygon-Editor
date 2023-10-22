using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        Border
    }
    public enum EdgeOrientation
    {
        None,
        Horizontal,
        Vertical
    }
    public class Polygon
    {
        public bool ShowBorder { get; set; }
        public List<PointF> Points;
        public List<PointF> BorderPoints;
        public EdgeOrientation[] EdgeRelations;
        public Polygon(List<PointF> points)
        {
            Points = points;
            ShowBorder = false;
            EdgeRelations = new EdgeOrientation[points.Count];
        }
    }
    public class Edge
    {
        public PointF p1;
        public PointF p2;
        public Edge(PointF p1, PointF p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }
    }
    public class PolygonCanvas
    {
        public CanvasMode Mode;
        public List<PointF> Points;
        private PictureBox Canvas;
        private ContextMenuStrip MenuStrip;
        public List<Polygon> Polygons;
        private int radius = 5;
        public int offset { get; set; }

        /// <summary>
        ///  Edit mode variables
        /// </summary>
        private Polygon? selectedPolygon = null;
        private PointF? previousPoint = null;
        private PointF? selectedPoint = null;
        private Edge? selectedEdge = null;

        private Edge? edgeToSetRelation = null;
        private Polygon? polygonToSetRelation = null;

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
                }
            }
            else if(e.Button == MouseButtons.Right)
            {
                if(SelectPointOrEdge(e))
                {
                    if(selectedEdge != null)
                    {
                        MenuStrip.Show(Canvas.PointToScreen(e.Location));
                        edgeToSetRelation = selectedEdge;
                        polygonToSetRelation = selectedPolygon;
                    }
                }
            }
        }
        public void canvasMouseUp(object sender, MouseEventArgs e)
        {
            selectedPolygon = null;
            selectedPoint = null;
            selectedEdge = null;
        }
        public void canvasMouseMove(object sender, MouseEventArgs e)
        {
            if(Mode == CanvasMode.Move && selectedPolygon != null)
            {
                (var X, var Y) = (e.X - previousPoint.Value.X, e.Y - previousPoint.Value.Y);
                previousPoint = new PointF(e.X, e.Y);
                if(selectedPoint != null)
                {
                    for(int i=0; i<selectedPolygon.Points.Count; ++i)
                    {
                        var p = selectedPolygon.Points[i];
                        if (p.X == selectedPoint.Value.X && p.Y == selectedPoint.Value.Y)
                        {
                            ChangePointCoordinates(p, i, X, Y);
                            break;
                        }

                    }
                    selectedPoint = new PointF(selectedPoint.Value.X + X, selectedPoint.Value.Y + Y);
                }
                else if(selectedEdge != null)
                {
                    var counter = 0;
                    for(int i=0; i<selectedPolygon.Points.Count; ++i)
                    {
                        var p = selectedPolygon.Points[i];
                        if ((p.X == selectedEdge.p1.X && p.Y == selectedEdge.p1.Y) ||
                               (p.X == selectedEdge.p2.X && p.Y == selectedEdge.p2.Y))
                        {
                            ++counter;
                            ChangePointCoordinates(p, i, X, Y);
                            if (counter == 2)
                                break;
                        }
                    }
                    selectedEdge.p1 = new PointF(selectedEdge.p1.X+X, selectedEdge.p1.Y+Y);
                    selectedEdge.p2 = new PointF(selectedEdge.p2.X+X, selectedEdge.p2.Y+Y);
                }
                else
                    selectedPolygon.Points = selectedPolygon.Points.Select((p) => new PointF(p.X + X, p.Y + Y)).ToList();
                DrawPolygons();
            }
        }
        private void AddPoint(MouseEventArgs e)
        {
            if(Points.Count == 0 && SelectPointOrEdge(e))
            {
                if(selectedEdge != null)
                {
                    for(int i=0; i<selectedPolygon.Points.Count; i++)
                    {
                        if (selectedPolygon.Points[i] == selectedEdge.p1)
                        {
                            selectedPolygon.Points.Insert(i+1, new PointF(e.X,e.Y));
                            selectedPolygon.EdgeRelations = new EdgeOrientation[polygonToSetRelation.Points.Count];
                            DrawPolygons();
                            return;
                        }
                    }
                }
            }
            if (CheckIfPolygonIsCrated(e))
            {
                Canvas.Refresh();
                return;
            }
            Points.Add(new PointF(e.X, e.Y));
            using (Graphics g = Graphics.FromImage(Canvas.Image))
            {
                g.FillEllipse(Brushes.Black, e.X - radius, e.Y - radius, radius * 2, radius * 2);
            }
            if (Points.Count > 1)
                DrawLine(Points[Points.Count - 2], Points[Points.Count - 1], Canvas.Image as Bitmap);
            Canvas.Refresh();
        }
        private bool CheckIfPolygonIsCrated(MouseEventArgs e)
        {
            if(Points.Count<2)
                return false;
            
            if (Geometry.IsPointClicked(e, Points[0]))
            {
                SolidBrush brush = new SolidBrush(Color.FromArgb(128, 0, 0, 0));
                Polygons.Add(new Polygon(Points));
                using (Graphics g = Graphics.FromImage(Canvas.Image))
                {
                    g.FillPolygon(brush, Points.ToArray());
                }
                DrawLine(Points[Points.Count-1], Points[0], Canvas.Image as Bitmap);
                Points = new List<PointF>();
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
                for(int j = 0; j < poly.Points.Count; ++j)
                {
                    if(Geometry.IsEdgeClicked(e,new Edge(poly.Points[j], poly.Points[Utils.mod(j - 1, poly.Points.Count)])))
                    {
                        selectedPolygon = poly;
                        if (Geometry.IsPointClicked(e, poly.Points[Utils.mod(j - 1, poly.Points.Count)]))
                            selectedPoint = poly.Points[Utils.mod(j - 1, poly.Points.Count)];
                        else if (Geometry.IsPointClicked(e, poly.Points[j]))
                            selectedPoint = poly.Points[j];
                        else
                            selectedEdge = new Edge(poly.Points[Utils.mod(j-1,poly.Points.Count)], poly.Points[j]);
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
        private void DrawLine(PointF p1, PointF p2, Bitmap canvas)
        {
            Pen blackPen = new Pen(Color.Black, 2);

            using (var graphics = Graphics.FromImage(canvas))
            {
                graphics.DrawLine(blackPen, p1.X, p1.Y, p2.X, p2.Y);
            }
        }
        public void SetEgdeRelation(EdgeOrientation orientation)
        {
            var poly = polygonToSetRelation;
            var edge = edgeToSetRelation;
            var n = poly.Points.Count;
            for(int i = 0; i < n; i++)
            {
                if ((poly.Points[i] == edge.p1 && poly.Points[(i+1)%n] == edge.p2) ||
                    (poly.Points[i] == edge.p2 && poly.Points[(i + 1) % n] == edge.p1))
                {
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
                        var offset = poly.Points[id2].X - poly.Points[id1].X;
                        poly.Points[id2] = new PointF(poly.Points[id1].X + offset, poly.Points[id1].Y);
                    }
                    else
                    {
                        var offset = poly.Points[id2].Y- poly.Points[id1].Y;
                        poly.Points[id2] = new PointF(poly.Points[id1].X, poly.Points[id1].Y + offset);
                    }
                    poly.EdgeRelations[i] = orientation;
                }
            }
            DrawPolygons();
        }
        public void DrawPolygons()
        {
            var newCanvas = new Bitmap(Canvas.Size.Width, Canvas.Size.Height);
            SolidBrush brush = new SolidBrush(Color.FromArgb(128, 0, 0, 0));
            Pen redPen = new Pen(Color.Red, 2);
            Pen orangePen = new Pen(Color.Orange, 2);
            foreach (var poly in Polygons)
            {
                for(int i=0; i<poly.Points.Count; ++i)
                {
                    var point = poly.Points[i];
                    using (Graphics g = Graphics.FromImage(newCanvas))
                    {
                        g.FillEllipse(Brushes.Black, point.X - radius, point.Y - radius, radius * 2, radius * 2);
                        DrawLine(poly.Points[Utils.mod(i-1,poly.Points.Count)], point, newCanvas);
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
                        poly.BorderPoints = Geometry.CreateBoundedPolygon(poly, offset);
                        g.DrawPolygon(redPen, poly.BorderPoints.Select(x => new Point((int)x.X, (int)x.Y)).ToArray());
                    }
                }
            }
            Canvas.Image = newCanvas;
            Canvas.Refresh();
        }
        private void CheckConditionsAndDraw(Polygon poly)
        {
            if (poly.Points.Count == 0)
            {
                Polygons.Remove(poly);
                return;
            }
            if(poly.Points.Count < 3)
                poly.ShowBorder = false;
            DrawPolygons();
        }
        private void ChangePointCoordinates(PointF p, int i, float X, float Y)
        {
            p.X += X;
            p.Y += Y;
            selectedPolygon.Points[i] = p;
        }
        public void Clear()
        {
            Canvas.Image = new Bitmap(Canvas.Size.Width, Canvas.Size.Height);
            Polygons.Clear();
            Points.Clear();
            selectedEdge = null;
            selectedPoint = null;
            selectedPolygon = null;
            Mode = CanvasMode.Add;
        }
    }
}
