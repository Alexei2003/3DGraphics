using _3DGraphics.Classes;
using static _3DGraphics.Classes.BaseGraphisStructs;
using static _3DGraphics.Classes.ObjFileReader;

namespace _3DGraphics
{
    public partial class MainWindow : Form
    {
        private ObjFileReader.ModelData modelData = null;
        private ObjFileReader.ModelData modelDataPaint = null;
        private readonly System.Threading.Timer timerFPS;
        private System.Threading.Timer timerRotateY = null;
        private Camera camera;

        private readonly object lockModelData = new();

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
            timerFPS = new System.Threading.Timer(timerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private void AutoRotateY(object state)
        {
            if (modelData != null)
            {
                lock (lockModelData)
                {
                    const float shiftAxis = (float)Math.PI / 100;
                    camera.AngelsRotate.Y += shiftAxis;
                    CreateModelDataPaint();
                }
            }
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

                LinerDrawing.DrawLines(bitmap, modelDataPaint.GeometricVertexCoordinates, modelDataPaint.GeometricVertexIndexs);

                BackgroundImage = bitmap;

                lock (lockFrameCount)
                {
                    frameCount++;
                }
            }
        }


        private void opfdModelFile_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            lock (lockModelData)
            {
                var modelFilePath = opfdModelFile.FileName;
                modelData = Read(modelFilePath);

                CoordinateTransformations.TranslateVectors(modelData.GeometricVertexCoordinates, new CoordinateVector(
                    -(modelData.GeometricVertexCoordinates.Max(v => v.X) + modelData.GeometricVertexCoordinates.Min(v => v.X)) / 2,
                    -(modelData.GeometricVertexCoordinates.Max(v => v.Y) + modelData.GeometricVertexCoordinates.Min(v => v.Y)) / 2,
                    -(modelData.GeometricVertexCoordinates.Max(v => v.Z) + modelData.GeometricVertexCoordinates.Min(v => v.Z)) / 2));

                modelDataPaint = new ModelData(modelData);
                CreateModelDataPaint();
            }
        }

        private void bOpenModelFile_Click(object sender, EventArgs e)
        {
            opfdModelFile.ShowDialog();
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            tbFPS.Location = new Point(Width - tbFPS.Width - 20, tbFPS.Location.Y);

            camera.Size = Size;
            CreateModelDataPaint();
            ControlUpdate();
        }

        private void CreateModelDataPaint()
        {
            if (modelData != null)
            {
                modelDataPaint.SetCopyValue(modelData);
                CoordinateTransformations.GetFinalVectors(modelDataPaint.GeometricVertexCoordinates, camera);
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (modelData != null)
            {
                var angelRotate = (float)Math.PI / 100;
                var cahngeAngelFov = 1f;
                var scale = new CoordinateVector(1.1f, 1.1f, 1.1f);
                var translate = 10;


                if ((Control.ModifierKeys & Keys.Shift) != 0)
                {
                    angelRotate = -angelRotate;
                }

                lock (lockModelData)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.X:
                            camera.AngelsRotate.X += angelRotate;
                            break;
                        case Keys.Y:
                            camera.AngelsRotate.Y += angelRotate;
                            break;
                        case Keys.Z:
                            camera.AngelsRotate.Z += angelRotate;
                            break;
                        case Keys.Oemplus:
                            camera.Scale.Coordinates *= scale.Coordinates;
                            break;
                        case Keys.OemMinus:
                            camera.Scale.Coordinates /= scale.Coordinates;
                            break;
                        case Keys.Up:
                            camera.Eye.Z -= 10;
                            break;
                        case Keys.Down:
                            camera.Eye.Z += 10;
                            break;
                        case Keys.Add:
                            camera.IncFovAngle(cahngeAngelFov);
                            break;
                        case Keys.Subtract:
                            camera.IncFovAngle(-cahngeAngelFov);
                            break;
                        case Keys.Q:
                            camera.Translate.Z -= translate;
                            break;
                        case Keys.E:
                            camera.Translate.Z += translate;
                            break;
                        case Keys.W:
                            camera.Translate.Y += translate;
                            break;
                        case Keys.S:
                            camera.Translate.Y -= translate;
                            break;
                        case Keys.A:
                            camera.Translate.X -= translate;
                            break;
                        case Keys.D:
                            camera.Translate.X += translate;
                            break;
                    }
                    CreateModelDataPaint();
                }
            }
        }

        private void ControlUpdate()
        {
            bOpenModelFile.Update();
            tbFPS.Update();
            bAutoRotateY.Update();
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            ControlUpdate();
        }

        private void bAutoRotateY_Click(object sender, EventArgs e)
        {
            if (timerRotateY == null)
            {
                var timerCallback = new TimerCallback(AutoRotateY);
                timerRotateY = new System.Threading.Timer(timerCallback, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
            }
            else
            {
                timerRotateY.Dispose();
                timerRotateY = null;
            }
        }
    }
}
