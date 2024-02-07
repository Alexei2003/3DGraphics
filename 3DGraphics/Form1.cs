using _3DGraphics.Classes;
using System.Diagnostics;

namespace _3DGraphics
{
    public partial class MainWindow : Form
    {
        private ObjFileReader.FileReaderResult modelData;
        private bool modelDataHasValue = false;

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
            if (modelDataHasValue)
            {
                var bitmap = new Bitmap(Width, Height);
                LinerDrawing.DrawLines(bitmap, modelData.GeometricVertexCoordinates, modelData.GeometricVertexIndexs);
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

            Parallel.For(0, modelData.GeometricVertexCoordinates.Length, i =>
            {
                modelData.GeometricVertexCoordinates[i].Y = -modelData.GeometricVertexCoordinates[i].Y;
            });

            modelDataHasValue = true;

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
            if (modelDataHasValue)
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
                        modelData.GeometricVertexCoordinates = CoordinateTransformations.RotateVectorsAroundX(modelData.GeometricVertexCoordinates, shiftAxis);
                        break;
                    case Keys.Y:
                        modelData.GeometricVertexCoordinates = CoordinateTransformations.RotateVectorsAroundY(modelData.GeometricVertexCoordinates, shiftAxis);
                        break;
                    case Keys.Z:
                        modelData.GeometricVertexCoordinates = CoordinateTransformations.RotateVectorsAroundZ(modelData.GeometricVertexCoordinates, shiftAxis);
                        break;
                    case Keys.Oemplus:
                    case Keys.Add:
                        modelData.GeometricVertexCoordinates = CoordinateTransformations.ScaleVectors(modelData.GeometricVertexCoordinates, new BaseGraphisStructs.CoordinateVector(scale, scale, scale));
                        break;
                    case Keys.OemMinus:
                    case Keys.Subtract:
                        scale = 1 / scale;
                        modelData.GeometricVertexCoordinates = CoordinateTransformations.ScaleVectors(modelData.GeometricVertexCoordinates, new BaseGraphisStructs.CoordinateVector(scale, scale, scale));
                        break;
                }

                Invalidate();
                Update();
            }
        }
    }
}
