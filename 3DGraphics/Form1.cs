using _3DGraphics.Classes;
using static _3DGraphics.Classes.BaseGraphisStructs;
using static _3DGraphics.Classes.ObjFileReader;

namespace _3DGraphics
{
    public partial class MainWindow : Form
    {
        private ObjFileReader.ModelData modelData = null;
        private ObjFileReader.ModelData modelDataPaint = null;
        private readonly System.Threading.Timer timer;
        private Camera camera;

        public MainWindow()
        {
            InitializeComponent();
            opfdModelFile.InitialDirectory = "D:\\ALL_DOWNLOAD\\.Model";
            opfdModelFile.Filter = "model (*.obj)|";
            opfdModelFile.FilterIndex = 0;
            opfdModelFile.RestoreDirectory = true;

            tbFPS.Location = new Point(Width - tbFPS.Width - 20, tbFPS.Location.Y);

            camera = new Camera(Width, Height);

            var timerCallback = new TimerCallback(UpdateFPS);
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

        private readonly object lockFrameCount = new();
        private int frameCount = 0;

        private void MainWindow_Paint(object sender, PaintEventArgs e)
        {
            if (modelDataPaint != null)
            {
                var bitmap = new Bitmap(Width, Height);

                LinerDrawing.DrawLines(bitmap, modelDataPaint.GeometricVertexCoordinates, modelDataPaint.GeometricVertexIndexs, modelDataPaint.CoordinateTransformationlateVector);

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
            modelData = Read(modelFilePath);
            modelData.CoordinateTransformationlateVector = new CoordinateVector(300, 300, 0);

            CoordinateTransformations.TranslateVectors(modelData.GeometricVertexCoordinates, new CoordinateVector(0, -GetAverangeY(modelData.GeometricVertexCoordinates), 0));
            camera = new Camera(Width, Height);
            tmpModelData = new ModelData(modelData);
            modelDataPaint = new ModelData(modelData);
            CreateModelDataPaint();
        }

        private static float GetAverangeY(GeometricVertex[] vectors)
        {
            return vectors.Max(v => v.Y) / 2;
        }

        private void bOpenModelFile_Click(object sender, EventArgs e)
        {
            opfdModelFile.ShowDialog();
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            tbFPS.Location = new Point(Width - tbFPS.Width - 20, tbFPS.Location.Y);
        }

        private ModelData tmpModelData;

        private void CreateModelDataPaint()
        {
            tmpModelData.SetCopyValue(modelData);
            CoordinateTransformations.GetViewVectors(tmpModelData.GeometricVertexCoordinates, camera);
            CoordinateTransformations.GetProjectionVectors(tmpModelData.GeometricVertexCoordinates, camera);
            CoordinateTransformations.GetViewWindowVectors(tmpModelData.GeometricVertexCoordinates, camera);
            modelDataPaint.SetCopyValue(tmpModelData);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (modelData != null)
            {
                double shiftAxis = Math.PI / 100;
                const float scale = 1f;
                const float translate = 5;

                if ((Control.ModifierKeys & Keys.Shift) != 0)
                {
                    shiftAxis = -shiftAxis;
                }

                switch (e.KeyCode)
                {
                    case Keys.X:
                        CoordinateTransformations.RotateAroundX(modelData.GeometricVertexCoordinates, shiftAxis);
                        break;
                    case Keys.Y:
                        CoordinateTransformations.RotateAroundY(modelData.GeometricVertexCoordinates, shiftAxis);
                        break;
                    case Keys.Z:
                        CoordinateTransformations.RotateAroundZ(modelData.GeometricVertexCoordinates, shiftAxis);
                        break;
                    case Keys.Oemplus:
                    case Keys.Add:
                        camera.IncFovAngle(scale);
                        break;
                    case Keys.OemMinus:
                    case Keys.Subtract:
                        camera.IncFovAngle(-scale);
                        break;
                    case Keys.Q:
                        CoordinateTransformations.TranslateCoordinate(modelData.CoordinateTransformationlateVector, new BaseGraphisStructs.CoordinateVector(0, 0, translate));
                        break;
                    case Keys.E:
                        CoordinateTransformations.TranslateCoordinate(modelData.CoordinateTransformationlateVector, new BaseGraphisStructs.CoordinateVector(0, 0, -translate));
                        break;
                    case Keys.W:
                        CoordinateTransformations.TranslateCoordinate(modelData.CoordinateTransformationlateVector, new BaseGraphisStructs.CoordinateVector(0, -translate, 0));
                        break;
                    case Keys.S:
                        CoordinateTransformations.TranslateCoordinate(modelData.CoordinateTransformationlateVector, new BaseGraphisStructs.CoordinateVector(0, translate, 0));
                        break;
                    case Keys.A:
                        CoordinateTransformations.TranslateCoordinate(modelData.CoordinateTransformationlateVector, new BaseGraphisStructs.CoordinateVector(-translate, 0, 0));
                        break;
                    case Keys.D:
                        CoordinateTransformations.TranslateCoordinate(modelData.CoordinateTransformationlateVector, new BaseGraphisStructs.CoordinateVector(translate, 0, 0));
                        break;
                }
                CreateModelDataPaint();
            }
        }
    }
}
