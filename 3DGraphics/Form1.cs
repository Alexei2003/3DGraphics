using _3DGraphics.Classes;
using static _3DGraphics.Classes.BaseGraphisStructs;

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

            camera = new Camera(Width,Height);

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
            modelData = ObjFileReader.Read(modelFilePath);
            modelData.CoordinateTransformationlateVector = new CoordinateVector(300, 300, 0);
            camera = new Camera(Width, Height);
            CreateModelDataPaint();
        }

        private void bOpenModelFile_Click(object sender, EventArgs e)
        {
            opfdModelFile.ShowDialog();
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            tbFPS.Location = new Point(Width - tbFPS.Width - 20, tbFPS.Location.Y);
        }

        private void CreateModelDataPaint()
        {
            if (modelData != null)
            {
                var tmpModelData =new ObjFileReader.ModelData(modelData);
                tmpModelData.GeometricVertexCoordinates = CoordinateTransformations.GetViewVectors(tmpModelData.GeometricVertexCoordinates, camera);
                CoordinateTransformations.GetProjectionVectors(tmpModelData.GeometricVertexCoordinates,camera);
                CoordinateTransformations.GetViewWindowVectors(tmpModelData.GeometricVertexCoordinates);
                modelDataPaint = tmpModelData;
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
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
                    camera.RotateAroundX(shiftAxis);
                    break;
                case Keys.Y:
                    camera.RotateAroundY(shiftAxis);
                    break;
                case Keys.Z:
                    camera.RotateAroundZ(shiftAxis);
                    break;
                case Keys.Oemplus:
                case Keys.Add:
                    CoordinateTransformations.ScaleVectors(modelData.GeometricVertexCoordinates, new BaseGraphisStructs.CoordinateVector(scale, scale, scale));
                    break;
                case Keys.OemMinus:
                case Keys.Subtract:
                    scale = 1 / scale;
                    CoordinateTransformations.ScaleVectors(modelData.GeometricVertexCoordinates, new BaseGraphisStructs.CoordinateVector(scale, scale, scale));
                    break;
                case Keys.Q:
                    CoordinateTransformations.TranslateVectors(modelData.CoordinateTransformationlateVector, new BaseGraphisStructs.CoordinateVector(0, 0, translate));
                    break;
                case Keys.E:
                    CoordinateTransformations.TranslateVectors(modelData.CoordinateTransformationlateVector, new BaseGraphisStructs.CoordinateVector(0, 0, -translate));
                    break;
                case Keys.W:
                    CoordinateTransformations.TranslateVectors(modelData.CoordinateTransformationlateVector, new BaseGraphisStructs.CoordinateVector(0, -translate, 0));
                    break;
                case Keys.S:
                    CoordinateTransformations.TranslateVectors(modelData.CoordinateTransformationlateVector, new BaseGraphisStructs.CoordinateVector(0, translate, 0));
                    break;
                case Keys.A:
                    CoordinateTransformations.TranslateVectors(modelData.CoordinateTransformationlateVector, new BaseGraphisStructs.CoordinateVector(-translate, 0, 0));
                    break;
                case Keys.D:
                    CoordinateTransformations.TranslateVectors(modelData.CoordinateTransformationlateVector, new BaseGraphisStructs.CoordinateVector(translate, 0, 0));
                    break;
            }
            CreateModelDataPaint();
        }
    }
}
