﻿using System;
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
    public class Polygon
    {
        public bool ShowBorder { get; set; }
        public List<PointF> Points;
        public List<PointF> BorderPoints;
        public Polygon(List<PointF> points)
        {
            Points = points;
            ShowBorder = false;
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
        public PictureBox Canvas;
        public List<Polygon> Polygons;
        private int radius = 5;
        private int offset = 20;

        /// <summary>
        ///  Edit mode variables
        /// </summary>
        private Polygon? selectedPolygon = null;
        private PointF? previousPoint = null;
        private PointF? selectedPoint = null;
        private Edge? selectedEdge = null;
        public PolygonCanvas(CanvasMode mode, List<PointF> points, PictureBox canvas, List<Polygon> polygons)
        {
            this.Mode = mode;
            this.Points = points;
            this.Canvas = canvas;
            this.Canvas.MouseDown += this.canvasMouseDown;
            this.Canvas.MouseUp += this.canvasMouseUp;
            this.Canvas.MouseMove += this.canvasMouseMove;
            Polygons = polygons;
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
                            if(poly.ShowBorder)
                            {
                                poly.ShowBorder = false;
                            }
                            else
                            {
                                poly.BorderPoints = Geometry.CreateBoundedPolygon(poly.Points, offset);
                                poly.ShowBorder = true;
                            }
                            DrawPolygons();
                        }
                        break;
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
                    selectedPolygon.Points = selectedPolygon.Points.Select((p) =>
                    {
                        if (p.X == selectedPoint.Value.X && p.Y == selectedPoint.Value.Y)
                            return new PointF(p.X + X, p.Y + Y);
                        else
                            return p;
                    }).ToList();
                    selectedPoint = new PointF(selectedPoint.Value.X + X, selectedPoint.Value.Y + Y);
                }
                else if(selectedEdge != null)
                {
                    selectedPolygon.Points = selectedPolygon.Points.Select((p) =>
                    {
                        if ((p.X == selectedEdge.p1.X && p.Y == selectedEdge.p1.Y) ||
                                (p.X == selectedEdge.p2.X && p.Y == selectedEdge.p2.Y) )
                            return new PointF(p.X + X, p.Y + Y);
                        else
                            return p;
                    }).ToList();
                    selectedEdge.p1 = new PointF(selectedEdge.p1.X+X, selectedEdge.p1.Y+Y);
                    selectedEdge.p2 = new PointF(selectedEdge.p2.X+X, selectedEdge.p2.Y+Y);
                }
                else
                {
                    selectedPolygon.Points = selectedPolygon.Points.Select((p) => new PointF(p.X + X, p.Y + Y)).ToList();
                }
                if (selectedPolygon.ShowBorder)
                    selectedPolygon.BorderPoints = Geometry.CreateBoundedPolygon(selectedPolygon.Points, offset);
                DrawPolygons();
            }
        }
        private void AddPoint(MouseEventArgs e)
        {
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
                DrawLine(Points[Points.Count - 2], Points[Points.Count - 1]);
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
                DrawLine(Points[Points.Count-1], Points[0]);
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
                    if (Geometry.IsPointClicked(e, poly.Points[j]))
                    {
                        selectedPolygon = poly;
                        selectedPoint = poly.Points[j];
                        return true;
                    }
                    if(Geometry.IsEdgeClicked(e,new Edge(poly.Points[j], poly.Points[mod(j - 1, poly.Points.Count)])))
                    {
                        selectedPolygon = poly;
                        selectedEdge = new Edge(poly.Points[mod(j-1,poly.Points.Count)], poly.Points[j]);
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
                    if(Geometry.IsPointClicked(e, poly.Points[i]))
                    {
                        poly.Points.RemoveAt(i);
                        CheckConditionsAndDraw(poly);
                        return true;
                    }
                    else if(Geometry.IsEdgeClicked(e, new Edge(poly.Points[mod(i - 1,poly.Points.Count)], poly.Points[i])))
                    {
                        poly.Points.RemoveAt(i);
                        poly.Points.RemoveAt(mod(i-1,poly.Points.Count));
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
        private void DrawLine(PointF p1, PointF p2)
        {
            Pen blackPen = new Pen(Color.Black, 2);

            using (var graphics = Graphics.FromImage(Canvas.Image))
            {
                graphics.DrawLine(blackPen, p1.X, p1.Y, p2.X, p2.Y);
            }
        }
        private void DrawPolygons()
        {
            Canvas.Image.Dispose();
            Canvas.Image = new Bitmap(Canvas.Size.Width, Canvas.Size.Height);
            SolidBrush brush = new SolidBrush(Color.FromArgb(128, 0, 0, 0));
            Pen redPen = new Pen(Color.Red, 2);
            foreach (var poly in Polygons)
            {
                for(int i=0; i<poly.Points.Count; ++i)
                {
                    var point = poly.Points[i];
                    using (Graphics g = Graphics.FromImage(Canvas.Image))
                    {
                        g.FillEllipse(Brushes.Black, point.X - radius, point.Y - radius, radius * 2, radius * 2);
                        DrawLine(poly.Points[mod(i-1,poly.Points.Count)], point);
                    }
                }
                using (Graphics g = Graphics.FromImage(Canvas.Image))
                {
                    g.FillPolygon(brush, poly.Points.ToArray());
                    if(poly.ShowBorder)
                    {
                        g.DrawPolygon(redPen, poly.BorderPoints.Select(x => new Point((int)x.X,(int)x.Y)).ToArray());
                    }
                }
            }
        }
        private void CheckConditionsAndDraw(Polygon poly)
        {
            if (poly.Points.Count == 0)
                Polygons.Remove(poly);
            if (poly.ShowBorder && poly.Points.Count >= 3)
                Geometry.CreateBoundedPolygon(Points, offset);
            DrawPolygons();
        }
        private int mod(int x, int m)
        {
            return (x % m + m) % m;
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
