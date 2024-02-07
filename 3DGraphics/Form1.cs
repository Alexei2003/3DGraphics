using _3DGraphics.Classes;
using System.Diagnostics;

namespace _3DGraphics
{
    public partial class MainWindow : Form
    {
        private ObjFileReader.FileReaderResult? modelData = null;

        public MainWindow()
        {
            InitializeComponent();
            opfdModelFile.InitialDirectory = "D:\\ALL_DOWNLOAD\\.Model";
            opfdModelFile.Filter = "model (*.obj)|";
            opfdModelFile.FilterIndex = 0;
            opfdModelFile.RestoreDirectory = true;

            tbFPS.Location = new Point(Width - tbFPS.Width - 20, tbFPS.Location.Y);
        }

        private readonly Stopwatch stopwatch = new();
        private int frameCount = 0;

        private void MainWindow_Paint(object sender, PaintEventArgs e)
        {
            if (modelData.HasValue)
            {
                var bitmap = new Bitmap(Width, Height);
                LinerDrawing.DrawLines(bitmap, modelData.Value.GeometricVertexCoordinates, modelData.Value.GeometricVertexIndexs);
                BackgroundImage = bitmap;

                // Запускаем секундомер, если он еще не запущен
                if (!stopwatch.IsRunning)
                {
                    stopwatch.Start();
                }

                frameCount++;

                // Если прошла секунда или более, обновляем счетчик кадров
                if (stopwatch.ElapsedMilliseconds >= 1000)
                {
                    tbFPS.Text = frameCount.ToString();
                    tbFPS.Update();
                    frameCount = 0;
                    stopwatch.Restart();
                }
            }
        }


        private void opfdModelFile_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var modelFilePath = opfdModelFile.FileName;
            modelData = ObjFileReader.Read(modelFilePath);

            var tmp = modelData.Value;

            for (var i = 0; i < tmp.GeometricVertexCoordinates.Length; i++)
            {
                tmp.GeometricVertexCoordinates[i].Y = -tmp.GeometricVertexCoordinates[i].Y;
            }

            modelData = tmp;

            Invalidate();
            Update();
        }

        private void bOpenModelFile_Click(object sender, EventArgs e)
        {
            opfdModelFile.ShowDialog();
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            tbFPS.Location = new Point(Width - tbFPS.Width - 20, tbFPS.Location.Y);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (modelData.HasValue)
            {
                double shiftAxis = 3.14 / 10;
                float scale = 1.1f;

                if ((Control.ModifierKeys & Keys.Shift) != 0)
                {
                    shiftAxis = -shiftAxis;
                }

                switch (e.KeyCode)
                {
                    case Keys.X:
                        Parallel.For(0, modelData.Value.GeometricVertexCoordinates.Length, i =>
                        {
                            modelData.Value.GeometricVertexCoordinates[i] = CoordinateTransformations.RotateVectorAroundX(modelData.Value.GeometricVertexCoordinates[i], shiftAxis);
                        });
                        break;
                    case Keys.Y:
                        Parallel.For(0, modelData.Value.GeometricVertexCoordinates.Length, i =>
                        {
                            modelData.Value.GeometricVertexCoordinates[i] = CoordinateTransformations.RotateVectorAroundY(modelData.Value.GeometricVertexCoordinates[i], shiftAxis);
                        });
                        break;
                    case Keys.Z:
                        Parallel.For(0, modelData.Value.GeometricVertexCoordinates.Length, i =>
                        {
                            modelData.Value.GeometricVertexCoordinates[i] = CoordinateTransformations.RotateVectorAroundZ(modelData.Value.GeometricVertexCoordinates[i], shiftAxis);
                        });
                        break;
                    case Keys.Oemplus:
                    case Keys.Add:
                        Parallel.For(0, modelData.Value.GeometricVertexCoordinates.Length, i =>
                        {
                            modelData.Value.GeometricVertexCoordinates[i] = CoordinateTransformations.ScaleVector(modelData.Value.GeometricVertexCoordinates[i], new BaseGraphisStructs.CoordinateVector(scale, scale, scale));
                        });
                        break;
                    case Keys.OemMinus:
                    case Keys.Subtract:
                        scale = 1 / scale;
                        Parallel.For(0, modelData.Value.GeometricVertexCoordinates.Length, i =>
                        {
                            modelData.Value.GeometricVertexCoordinates[i] = CoordinateTransformations.ScaleVector(modelData.Value.GeometricVertexCoordinates[i], new BaseGraphisStructs.CoordinateVector(scale, scale, scale));
                        });
                        break;
                }
                Invalidate();
                Update();
            }
        }
    }
}
