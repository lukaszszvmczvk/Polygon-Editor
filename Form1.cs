using System.Runtime.CompilerServices;

namespace lab1
{
    public partial class MainWindow : Form
    {
        PolygonCanvas polygonCanvas;
        public MainWindow()
        {
            InitializeComponent();
            addModeRadioButton.Checked = true;
            addModeRadioButton.CheckedChanged += RadioButtonCheckedChanged;
            moveModeRadioButton.CheckedChanged += RadioButtonCheckedChanged;
            deleteModeRadioButton.CheckedChanged += RadioButtonCheckedChanged;
            borderModeRadioButton.CheckedChanged += RadioButtonCheckedChanged;
            Canvas.Image = new Bitmap(Canvas.Size.Width, Canvas.Size.Height);
            polygonCanvas = new PolygonCanvas(CanvasMode.Add, new List<PointF>(), Canvas, new List<Polygon>());
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
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            Canvas.Image = new Bitmap(Canvas.Size.Width, Canvas.Size.Height);
            polygonCanvas.DrawPolygons();
        }

        private void clearCanvasButton_Click(object sender, EventArgs e)
        {
            polygonCanvas.Clear();
            addModeRadioButton.Checked = true;
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