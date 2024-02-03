using _3DGraphics.Classes;
using static _3DGraphics.Classes.BaseGraphisStructs;

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
                var g = e.Graphics;
                foreach (var vertex in modelData.Value.GeometricVertexs)
                {
                    var tmpVertex = new GeometricVertex(vertex.X * 100 + Width / 2 - 100, vertex.Y * 100 + Height / 2 - 100, vertex.Z * 100, 1);
                    g.DrawLine(new Pen(Color.Black), new PointF(tmpVertex.X, tmpVertex.Y), new PointF(tmpVertex.X + 10, tmpVertex.Y + 10));
                }
            }
        }

        private void opfdModelFile_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var modelFilePath = opfdModelFile.FileName;
            modelData = ObjFileReader.Read(modelFilePath);
            Invalidate();
            Update();
        }

        private void bOpenModelFile_Click(object sender, EventArgs e)
        {
            opfdModelFile.ShowDialog();
        }
    }
}
