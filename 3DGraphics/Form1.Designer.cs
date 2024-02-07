namespace _3DGraphics
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
            opfdModelFile = new OpenFileDialog();
            bOpenModelFile = new Button();
            tbFPS = new TextBox();
            bTest = new Button();
            SuspendLayout();
            // 
            // opfdModelFile
            // 
            opfdModelFile.FileName = "openFileDialog1";
            opfdModelFile.FileOk += opfdModelFile_FileOk;
            // 
            // bOpenModelFile
            // 
            bOpenModelFile.Location = new Point(12, 12);
            bOpenModelFile.Name = "bOpenModelFile";
            bOpenModelFile.Size = new Size(83, 56);
            bOpenModelFile.TabIndex = 0;
            bOpenModelFile.Text = "Открыть модель";
            bOpenModelFile.UseVisualStyleBackColor = true;
            bOpenModelFile.Click += bOpenModelFile_Click;
            // 
            // tbFPS
            // 
            tbFPS.Font = new Font("Segoe UI", 40F);
            tbFPS.Location = new Point(666, 12);
            tbFPS.Name = "tbFPS";
            tbFPS.ReadOnly = true;
            tbFPS.Size = new Size(106, 86);
            tbFPS.TabIndex = 1;
            tbFPS.Text = "000";
            tbFPS.TextAlign = HorizontalAlignment.Center;
            // 
            // bTest
            // 
            bTest.Location = new Point(101, 12);
            bTest.Name = "bTest";
            bTest.Size = new Size(83, 25);
            bTest.TabIndex = 2;
            bTest.Text = "button1";
            bTest.UseVisualStyleBackColor = true;
            bTest.Click += bTest_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 759);
            Controls.Add(bTest);
            Controls.Add(tbFPS);
            Controls.Add(bOpenModelFile);
            DoubleBuffered = true;
            KeyPreview = true;
            Name = "MainWindow";
            Text = "3DViewer";
            Paint += MainWindow_Paint;
            KeyDown += MainWindow_KeyDown;
            Resize += MainWindow_Resize;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private OpenFileDialog opfdModelFile;
        private Button bOpenModelFile;
        private TextBox tbFPS;
        private Button bTest;
    }
}
