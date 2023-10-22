namespace lab1
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            layoutPanel = new TableLayoutPanel();
            Canvas = new DoubleBufferedPictureBox();
            panel1 = new Panel();
            offsetBox = new NumericUpDown();
            label1 = new Label();
            borderModeRadioButton = new RadioButton();
            clearCanvasButton = new Button();
            deleteModeRadioButton = new RadioButton();
            moveModeRadioButton = new RadioButton();
            addModeRadioButton = new RadioButton();
            menuStrip = new ContextMenuStrip(components);
            horizontalToolStripMenuItem = new ToolStripMenuItem();
            verticalToolStripMenuItem = new ToolStripMenuItem();
            layoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Canvas).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)offsetBox).BeginInit();
            menuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // layoutPanel
            // 
            layoutPanel.ColumnCount = 2;
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 77.4530258F));
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22.54697F));
            layoutPanel.Controls.Add(Canvas, 0, 0);
            layoutPanel.Controls.Add(panel1, 1, 0);
            layoutPanel.Dock = DockStyle.Fill;
            layoutPanel.Location = new Point(0, 0);
            layoutPanel.Name = "layoutPanel";
            layoutPanel.RowCount = 1;
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            layoutPanel.Size = new Size(982, 553);
            layoutPanel.TabIndex = 0;
            // 
            // Canvas
            // 
            Canvas.BackColor = SystemColors.AppWorkspace;
            Canvas.Dock = DockStyle.Fill;
            Canvas.Location = new Point(3, 3);
            Canvas.Name = "Canvas";
            Canvas.Size = new Size(754, 547);
            Canvas.TabIndex = 0;
            Canvas.TabStop = false;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Controls.Add(offsetBox);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(borderModeRadioButton);
            panel1.Controls.Add(clearCanvasButton);
            panel1.Controls.Add(deleteModeRadioButton);
            panel1.Controls.Add(moveModeRadioButton);
            panel1.Controls.Add(addModeRadioButton);
            panel1.Location = new Point(763, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(216, 547);
            panel1.TabIndex = 1;
            // 
            // offsetBox
            // 
            offsetBox.Location = new Point(95, 193);
            offsetBox.Name = "offsetBox";
            offsetBox.Size = new Size(68, 27);
            offsetBox.TabIndex = 6;
            offsetBox.ValueChanged += offsetBox_ValueChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(37, 195);
            label1.Name = "label1";
            label1.Size = new Size(52, 20);
            label1.TabIndex = 5;
            label1.Text = "Offset:";
            // 
            // borderModeRadioButton
            // 
            borderModeRadioButton.AutoSize = true;
            borderModeRadioButton.Location = new Point(15, 163);
            borderModeRadioButton.Name = "borderModeRadioButton";
            borderModeRadioButton.Size = new Size(118, 24);
            borderModeRadioButton.TabIndex = 4;
            borderModeRadioButton.TabStop = true;
            borderModeRadioButton.Text = "Border Mode";
            borderModeRadioButton.UseVisualStyleBackColor = true;
            // 
            // clearCanvasButton
            // 
            clearCanvasButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            clearCanvasButton.Location = new Point(15, 481);
            clearCanvasButton.Name = "clearCanvasButton";
            clearCanvasButton.Size = new Size(192, 57);
            clearCanvasButton.TabIndex = 3;
            clearCanvasButton.Text = "Clear canvas";
            clearCanvasButton.UseVisualStyleBackColor = true;
            clearCanvasButton.Click += clearCanvasButton_Click;
            // 
            // deleteModeRadioButton
            // 
            deleteModeRadioButton.AutoSize = true;
            deleteModeRadioButton.Location = new Point(15, 121);
            deleteModeRadioButton.Name = "deleteModeRadioButton";
            deleteModeRadioButton.Size = new Size(121, 24);
            deleteModeRadioButton.TabIndex = 2;
            deleteModeRadioButton.TabStop = true;
            deleteModeRadioButton.Text = "Delete Mode ";
            deleteModeRadioButton.UseVisualStyleBackColor = true;
            // 
            // moveModeRadioButton
            // 
            moveModeRadioButton.AutoSize = true;
            moveModeRadioButton.Location = new Point(15, 75);
            moveModeRadioButton.Name = "moveModeRadioButton";
            moveModeRadioButton.Size = new Size(110, 24);
            moveModeRadioButton.TabIndex = 1;
            moveModeRadioButton.TabStop = true;
            moveModeRadioButton.Text = "Move Mode";
            moveModeRadioButton.UseVisualStyleBackColor = true;
            // 
            // addModeRadioButton
            // 
            addModeRadioButton.AutoSize = true;
            addModeRadioButton.Location = new Point(15, 28);
            addModeRadioButton.Name = "addModeRadioButton";
            addModeRadioButton.Size = new Size(101, 24);
            addModeRadioButton.TabIndex = 0;
            addModeRadioButton.TabStop = true;
            addModeRadioButton.Text = "Add Mode";
            addModeRadioButton.UseVisualStyleBackColor = true;
            // 
            // menuStrip
            // 
            menuStrip.ImageScalingSize = new Size(20, 20);
            menuStrip.Items.AddRange(new ToolStripItem[] { horizontalToolStripMenuItem, verticalToolStripMenuItem });
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(149, 52);
            // 
            // horizontalToolStripMenuItem
            // 
            horizontalToolStripMenuItem.Name = "horizontalToolStripMenuItem";
            horizontalToolStripMenuItem.Size = new Size(148, 24);
            horizontalToolStripMenuItem.Text = "Horizontal";
            horizontalToolStripMenuItem.Click += horizontalToolStripMenuItem_Click;
            // 
            // verticalToolStripMenuItem
            // 
            verticalToolStripMenuItem.Name = "verticalToolStripMenuItem";
            verticalToolStripMenuItem.Size = new Size(148, 24);
            verticalToolStripMenuItem.Text = "Vertical";
            verticalToolStripMenuItem.Click += verticalToolStripMenuItem_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(982, 553);
            Controls.Add(layoutPanel);
            Name = "MainWindow";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Polygon creator";
            Load += Form1_Load;
            Resize += MainWindow_Resize;
            layoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)Canvas).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)offsetBox).EndInit();
            menuStrip.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel layoutPanel;
        private Panel panel1;
        private RadioButton deleteModeRadioButton;
        private RadioButton moveModeRadioButton;
        private RadioButton addModeRadioButton;
        private Button clearCanvasButton;
        private RadioButton borderModeRadioButton;
        private ContextMenuStrip menuStrip;
        private ToolStripMenuItem horizontalToolStripMenuItem;
        private ToolStripMenuItem verticalToolStripMenuItem;
        private DoubleBufferedPictureBox Canvas;
        private Label label1;
        private NumericUpDown offsetBox;
    }
}