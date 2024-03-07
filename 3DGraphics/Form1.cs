using _3DGraphics.Classes;

namespace _3DGraphics
{
    public partial class MainWindow : Form
    {
        private ObjFileReader.FileReaderResult modelData;
        private bool modelDataHasValue = false;
        private readonly System.Threading.Timer timer;
        private readonly CoordinateTransformations coordinateTransformations = new();

        public MainWindow()
        {
            InitializeComponent();
            opfdModelFile.InitialDirectory = "D:\\ALL_DOWNLOAD\\.Model";
            opfdModelFile.Filter = "model (*.obj)|";
            opfdModelFile.FilterIndex = 0;
            opfdModelFile.RestoreDirectory = true;

            tbFPS.Location = new Point(Width - tbFPS.Width - 20, tbFPS.Location.Y);

            TimerCallback timerCallback = new TimerCallback(UpdateFPS);
            timer = new System.Threading.Timer(timerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private void UpdateFPS(object state)
        {
            if (tbFPS.InvokeRequired)
            {
                tbFPS.Invoke((MethodInvoker)delegate
                {
                    tbFPS.Text = frameCount.ToString();
                    tbFPS.Update();
                });
            }
            else
            {
                tbFPS.Text = frameCount.ToString();
                tbFPS.Update();
            }
            lock (lockFrameCount)
            {
                frameCount = 0;
            }
        }

        private object lockFrameCount = new();
        private int frameCount = 0;

        private void MainWindow_Paint(object sender, PaintEventArgs e)
        {
            if (modelDataHasValue)
            {
                var bitmap = new Bitmap(Width, Height);
                LinerDrawing.DrawLines(bitmap, modelData.GeometricVertexCoordinates, modelData.GeometricVertexIndexs);
                BackgroundImage = bitmap;

                lock (lockFrameCount)
                {
                    frameCount++;
                }
            }
        }


        private void opfdModelFile_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var modelFilePath = opfdModelFile.FileName;
            modelData = ObjFileReader.Read(modelFilePath);

            coordinateTransformations.RotateVectorsAroundX(modelData.GeometricVertexCoordinates, 3.14);

            modelDataHasValue = true;
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
                const float translate = 5;

                if ((Control.ModifierKeys & Keys.Shift) != 0)
                {
                    shiftAxis = -shiftAxis;
                }

                switch (e.KeyCode)
                {
                    case Keys.X:
                        coordinateTransformations.RotateVectorsAroundX(modelData.GeometricVertexCoordinates, shiftAxis);
                        break;
                    case Keys.Y:
                        coordinateTransformations.RotateVectorsAroundY(modelData.GeometricVertexCoordinates, shiftAxis);
                        break;
                    case Keys.Z:
                        coordinateTransformations.RotateVectorsAroundZ(modelData.GeometricVertexCoordinates, shiftAxis);
                        break;
                    case Keys.Oemplus:
                    case Keys.Add:
                        coordinateTransformations.ScaleVectors(modelData.GeometricVertexCoordinates, new BaseGraphisStructs.CoordinateVector(scale, scale, scale));
                        break;
                    case Keys.OemMinus:
                    case Keys.Subtract:
                        scale = 1 / scale;
                        coordinateTransformations.ScaleVectors(modelData.GeometricVertexCoordinates, new BaseGraphisStructs.CoordinateVector(scale, scale, scale));
                        break;
                    case Keys.Q:
                        coordinateTransformations.TranslateVectors(modelData.GeometricVertexCoordinates, new BaseGraphisStructs.CoordinateVector(0, 0, translate));
                        break;
                    case Keys.E:
                        coordinateTransformations.TranslateVectors(modelData.GeometricVertexCoordinates, new BaseGraphisStructs.CoordinateVector(0, 0, -translate));
                        break;
                    case Keys.W:
                        coordinateTransformations.TranslateVectors(modelData.GeometricVertexCoordinates, new BaseGraphisStructs.CoordinateVector(0, -translate, 0));
                        break;
                    case Keys.S:
                        coordinateTransformations.TranslateVectors(modelData.GeometricVertexCoordinates, new BaseGraphisStructs.CoordinateVector(0, translate, 0));
                        break;
                    case Keys.A:
                        coordinateTransformations.TranslateVectors(modelData.GeometricVertexCoordinates, new BaseGraphisStructs.CoordinateVector(-translate, 0, 0));
                        break;
                    case Keys.D:
                        coordinateTransformations.TranslateVectors(modelData.GeometricVertexCoordinates, new BaseGraphisStructs.CoordinateVector(translate, 0, 0));
                        break;
                }
            }
        }
    }
}
