using _3DGraphics.Classes;

namespace _3DGraphics
{
    public partial class MainWindow : Form
    {
        private ObjFileReader.FileReaderResult? modelData = null;

        public MainWindow()
        {
            InitializeComponent();
            opfdModelFile.InitialDirectory = "c:\\";
            opfdModelFile.Filter = "model (*.obj)|";
            opfdModelFile.FilterIndex = 0;
            opfdModelFile.RestoreDirectory = true;
        }

        private void MainWindow_Paint(object sender, PaintEventArgs e)
        {
            if (modelData != null)
            {
                var bitmap = new Bitmap(Width, Height);
                LinerDrawing.DrawLines(bitmap, modelData.Value.GeometricVertex—oordinates, modelData.Value.GeometricVertexIndexs);
                BackgroundImage = bitmap;
            }
        }

        private void opfdModelFile_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var modelFilePath = opfdModelFile.FileName;
            modelData = ObjFileReader.Read(modelFilePath);

            var tmp = modelData.Value;

            for (var i = 0; i < tmp.GeometricVertex—oordinates.Length; i++)
            {
                tmp.GeometricVertex—oordinates[i].Y = -tmp.GeometricVertex—oordinates[i].Y;
            }

            modelData = tmp;

            Invalidate();
            Update();
        }

        private void bOpenModelFile_Click(object sender, EventArgs e)
        {
            opfdModelFile.ShowDialog();
        }
    }
}
