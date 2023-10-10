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
            Canvas.Image = new Bitmap(Canvas.Size.Width, Canvas.Size.Height);
            polygonCanvas = new PolygonCanvas(CanvasMode.Add, new List<Point>(), Canvas, new List<Polygon>());
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
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {

        }

        private void clearCanvasButton_Click(object sender, EventArgs e)
        {
            polygonCanvas.Clear();
        }
    }
}