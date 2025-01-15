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
            pInfo = new Panel();
            rtbInfo = new RichTextBox();
            bShowInfo = new Button();
            pInfo.SuspendLayout();
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
            // pInfo
            // 
            pInfo.Controls.Add(rtbInfo);
            pInfo.Location = new Point(432, 74);
            pInfo.Name = "pInfo";
            pInfo.Size = new Size(140, 249);
            pInfo.TabIndex = 3;
            // 
            // rtbInfo
            // 
            rtbInfo.Location = new Point(3, 3);
            rtbInfo.Name = "rtbInfo";
            rtbInfo.ReadOnly = true;
            rtbInfo.Size = new Size(134, 243);
            rtbInfo.TabIndex = 0;
            rtbInfo.Text = "F1\nF2\nF3\nF4\nF5\nF6\nF7";
            // 
            // bShowInfo
            // 
            bShowInfo.Location = new Point(489, 12);
            bShowInfo.Name = "bShowInfo";
            bShowInfo.Size = new Size(83, 56);
            bShowInfo.TabIndex = 4;
            bShowInfo.Text = "Показать информацию";
            bShowInfo.UseVisualStyleBackColor = true;
            bShowInfo.Click += bShowInfo_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(584, 559);
            Controls.Add(bShowInfo);
            Controls.Add(pInfo);
            Controls.Add(bOpenModelFile);
            DoubleBuffered = true;
            KeyPreview = true;
            Name = "MainWindow";
            Text = "3DViewer";
            Activated += MainWindow_Activated;
            Paint += MainWindow_Paint;
            KeyDown += MainWindow_KeyDown;
            Resize += MainWindow_Resize;
            pInfo.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private OpenFileDialog opfdModelFile;
        private Button bOpenModelFile;
        private Panel pInfo;
        private RichTextBox rtbInfo;
        private Button bShowInfo;
    }
}
