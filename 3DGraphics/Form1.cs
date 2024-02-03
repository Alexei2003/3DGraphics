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
                foreach (var vertexIndex in modelData.Value.GeometricVertexIndexs)
                {
                    var vertexs = modelData.Value.GeometricVertexs;

                    var xShift = Width / 2 - 100;
                    var yShift = Height / 2 + 400;
                    var scale = 5;

                    var point0 = new PointF(vertexs[vertexIndex[0]].X * scale + xShift, vertexs[vertexIndex[0]].Y * scale + yShift);
                    var point1 = new PointF(vertexs[vertexIndex[1]].X * scale + xShift, vertexs[vertexIndex[1]].Y * scale + yShift);
                    g.DrawLine(new Pen(Color.Black), point0, point1);

                    var point2 = new PointF(vertexs[vertexIndex[2]].X * scale + xShift, vertexs[vertexIndex[2]].Y * scale + yShift);
                    g.DrawLine(new Pen(Color.Black), point1, point2);

                    if (vertexIndex.Length == 4)
                    {
                        var point3 = new PointF(vertexs[vertexIndex[3]].X * scale + xShift, vertexs[vertexIndex[3]].Y * scale + yShift);
                        g.DrawLine(new Pen(Color.Black), point2, point3);
                        g.DrawLine(new Pen(Color.Black), point3, point0);
                    }
                    else
                    {
                        g.DrawLine(new Pen(Color.Black), point2, point0);
                    }
                }
            }
        }

        private void opfdModelFile_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var modelFilePath = opfdModelFile.FileName;
            modelData = ObjFileReader.Read(modelFilePath);

            var tmp = modelData.Value;

            for (var i = 0; i < tmp.GeometricVertexs.Length; i++)
            {
                tmp.GeometricVertexs[i].Y = -tmp.GeometricVertexs[i].Y;
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
