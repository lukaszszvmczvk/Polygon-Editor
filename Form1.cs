using System.Runtime.CompilerServices;

namespace lab1
{
    public partial class MainWindow : Form
    {
        PolygonCanvas polygonCanvas;
        public MainWindow()
        {
            InitializeComponent();
            offsetBox.Minimum = 0;
            offsetBox.Value = 20;
            offsetBox.Maximum = 1000;
            numericUpDown1.Value = 50;
            numericUpDown1.Maximum = 1000;
            addModeRadioButton.Checked = true;
            Canvas.BackColor = Color.White;
            addModeRadioButton.CheckedChanged += RadioButtonCheckedChanged;
            moveModeRadioButton.CheckedChanged += RadioButtonCheckedChanged;
            deleteModeRadioButton.CheckedChanged += RadioButtonCheckedChanged;
            borderModeRadioButton.CheckedChanged += RadioButtonCheckedChanged;
            Canvas.Image = new Bitmap(Canvas.Size.Width, Canvas.Size.Height);
            polygonCanvas = new PolygonCanvas(CanvasMode.Add, new List<PointF>(), Canvas, new List<Polygon>(), menuStrip);
            polygonCanvas.Offset = 20;
            polygonCanvas.CircleRadius = 50;
        }

        private void RadioButtonCheckedChanged(object? sender, EventArgs e)
        {
            if (addModeRadioButton.Checked)
            {
                polygonCanvas.Mode = CanvasMode.Add;
            }
            else if (moveModeRadioButton.Checked)
            {
                polygonCanvas.Mode = CanvasMode.Move;
            }
            else if (deleteModeRadioButton.Checked)
            {
                polygonCanvas.Mode = CanvasMode.Delete;
            }
            else if (borderModeRadioButton.Checked)
            {
                polygonCanvas.Mode = CanvasMode.Border;
            }
            else if (circleRadioButton.Checked)
            {
                polygonCanvas.Mode = CanvasMode.Circle;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            Canvas.Image.Dispose();
            Canvas.Image = new Bitmap(Canvas.Size.Width, Canvas.Size.Height);
            polygonCanvas.DrawPolygons();
        }

        private void clearCanvasButton_Click(object sender, EventArgs e)
        {
            polygonCanvas.Clear();
            addModeRadioButton.Checked = true;
        }

        private void horizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            polygonCanvas.SetEgdeRelation(EdgeOrientation.Horizontal);
        }

        private void verticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            polygonCanvas.SetEgdeRelation(EdgeOrientation.Vertical);
        }

        private void offsetBox_ValueChanged(object sender, EventArgs e)
        {
            if (polygonCanvas != null)
            {
                polygonCanvas.Offset = (int)offsetBox.Value;
                polygonCanvas.DrawPolygons();
            }
        }

        private void bresenhamCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (polygonCanvas != null)
            {
                if (bresenhamCheckBox.Checked)
                    polygonCanvas.UseBresenham = true;
                else
                    polygonCanvas.UseBresenham = false;

                polygonCanvas.DrawPolygons();
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (polygonCanvas != null)
            {
                polygonCanvas.CircleRadius = (int)numericUpDown1.Value;
                polygonCanvas.DrawPolygons();
            }
        }
    }
    public class DoubleBufferedPictureBox : PictureBox
    {
        public DoubleBufferedPictureBox()
        {
            DoubleBuffered = true;
        }
    }
}