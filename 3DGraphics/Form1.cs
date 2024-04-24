using _3DGraphics.Classes;
using _3DGraphics.Drawing;
using System.Numerics;
using static _3DGraphics.Classes.BaseGraphisStructs;
using static _3DGraphics.Classes.ObjFileReader;

namespace _3DGraphics
{
    public partial class MainWindow : Form
    {
        private ObjFileReader.ModelData modelData = null;
        private ObjFileReader.ModelData modelDataPaint = null;
        private readonly System.Threading.Timer timerFPS;
        private Camera camera = null;

        private const int WIDTH = 600;
        private const int HEIGHT = 600;

        private readonly Dictionary<string, Size> controlsSize = [];
        private readonly float baseTextSize = 10f;

        public MainWindow()
        {
            InitializeComponent();

            Size = new Size(WIDTH, HEIGHT);
            controlsSize.Add("bOpenModelFile", bOpenModelFile.Size);
            controlsSize.Add("bShowInfo", bShowInfo.Size);
            controlsSize.Add("rtbInfo", rtbInfo.Size);
            controlsSize.Add("pInfo", pInfo.Size);

            opfdModelFile.InitialDirectory = "D:\\ALL_DOWNLOAD\\.Model";
            opfdModelFile.Filter = "model (*.obj)|";
            opfdModelFile.FilterIndex = 0;
            opfdModelFile.RestoreDirectory = true;

            ControlResize();
            ZBuffer.Resaze(WIDTH, Height);

            camera = new Camera(Width, Height);

            var timerCallback = new TimerCallback(UpdateFPS);
            timerFPS = new System.Threading.Timer(timerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private void UpdateFPS(object state)
        {
            var strInfo = "" +
                    $"FPS = {frameCount}\n" +
                    $"Aspect = {camera.Aspect}\n" +
                    $"Width = {camera.Size.Width}\n" +
                    $"Height = {camera.Size.Height}\n" +
                    $"Fov = {camera.FovAngle}\n" +
                    $"Scale = {camera.Scale.X}\n" +
                    $"ZCamera = {camera.Eye.Z}\n" +
                    $"ZTarget = {camera.Translate.Z}\n";

            lock (lockFrameCount)
            {
                frameCount = 0;
            }

            if (rtbInfo.InvokeRequired)
            {
                rtbInfo.Invoke((MethodInvoker)delegate
                {
                    rtbInfo.Text = strInfo;
                    rtbInfo.Update();
                });
            }
            else
            {
                rtbInfo.Text = strInfo;
                rtbInfo.Update();
            }
        }

        private readonly object lockFrameCount = new();
        private int frameCount = 0;

        private void MainWindow_Paint(object sender, PaintEventArgs e)
        {
            //AutoRotate();

            var bitmap = new Bitmap(Width, Height);

            using (var g = Graphics.FromImage(bitmap))
            {
                using var brush = new SolidBrush(Color.Black);
                g.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
            }

            if (modelDataPaint != null)
            {
                DrawingModel.Draw(bitmap, modelDataPaint.GeometricVertexCoordinates, modelDataPaint.GeometricVertexIndexs);

                lock (lockFrameCount)
                {
                    frameCount++;
                }
            }
            BackgroundImage = bitmap;
        }

        private void opfdModelFile_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var modelFilePath = opfdModelFile.FileName;
            modelData = Read(modelFilePath);

            CoordinateTransformations.TranslateVectors(modelData.GeometricVertexCoordinates, new CoordinateVector(
                -(modelData.GeometricVertexCoordinates.Max(v => v.X) + modelData.GeometricVertexCoordinates.Min(v => v.X)) / 2,
                -(modelData.GeometricVertexCoordinates.Max(v => v.Y) + modelData.GeometricVertexCoordinates.Min(v => v.Y)) / 2,
                -(modelData.GeometricVertexCoordinates.Max(v => v.Z) + modelData.GeometricVertexCoordinates.Min(v => v.Z)) / 2));

            modelDataPaint = new ModelData(modelData);
            camera = new Camera(Width, Height);
            CreateModelDataPaint();
        }

        private void bOpenModelFile_Click(object sender, EventArgs e)
        {
            opfdModelFile.ShowDialog();
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            ControlResize();

            ZBuffer.Resaze(Width, Height);

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
                const float angelRotate = (float)Math.PI / 100;
                const float cahngeAngelFov = 1f;
                var scale = new Vector3(1.1f, 1.1f, 1.1f);
                const int translate = 1;
                const int near = 25;

                switch (e.KeyCode)
                {
                    case Keys.X:
                        if ((Control.ModifierKeys & Keys.Shift) != 0)
                        {
                            camera.AngelsRotate.X -= angelRotate;
                        }
                        else
                        {
                            camera.AngelsRotate.X += angelRotate;
                        }
                        break;
                    case Keys.Y:
                        if ((Control.ModifierKeys & Keys.Shift) != 0)
                        {
                            camera.AngelsRotate.Y -= angelRotate;
                        }
                        else
                        {
                            camera.AngelsRotate.Y += angelRotate;
                        }
                        break;
                    case Keys.Z:
                        if ((Control.ModifierKeys & Keys.Shift) != 0)
                        {
                            camera.AngelsRotate.Z -= angelRotate;
                        }
                        else
                        {
                            camera.AngelsRotate.Z += angelRotate;
                        }
                        break;
                    case Keys.Oemplus:
                        camera.Scale *= scale;
                        break;
                    case Keys.OemMinus:
                        camera.Scale /= scale;
                        break;
                    case Keys.Up:
                        camera.IncEyeZ(-near);
                        break;
                    case Keys.Down:
                        camera.IncEyeZ(near);
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
                    default:
                        return;
                }
                CreateModelDataPaint();
            }
        }

        private void ControlUpdate()
        {
            bOpenModelFile.Update();
            pInfo.Update();
            bShowInfo.Update();
        }

        private void ControlResize()
        {
            float changeResolutionWidth = (float)Width / WIDTH;
            float changeResolutionHeight = (float)Height / HEIGHT;

            float size;
            Font tmpFont;
            if (changeResolutionWidth < changeResolutionHeight)
            {
                tmpFont = new Font(bOpenModelFile.Font.FontFamily, baseTextSize * changeResolutionWidth);
                size = changeResolutionWidth;
            }
            else
            {
                tmpFont = new Font(bOpenModelFile.Font.FontFamily, baseTextSize * changeResolutionHeight);
                size = changeResolutionHeight;
            }

            bOpenModelFile.Size = new Size(Convert.ToInt32(controlsSize["bOpenModelFile"].Width * size), Convert.ToInt32(controlsSize["bOpenModelFile"].Height * size));
            bShowInfo.Size = new Size(Convert.ToInt32(controlsSize["bShowInfo"].Width * size), Convert.ToInt32(controlsSize["bShowInfo"].Height * size));
            rtbInfo.Size = new Size(Convert.ToInt32(controlsSize["rtbInfo"].Width * size), Convert.ToInt32(controlsSize["rtbInfo"].Height * size));
            pInfo.Size = new Size(Convert.ToInt32(controlsSize["pInfo"].Width * size), Convert.ToInt32(controlsSize["pInfo"].Height * size));

            bShowInfo.Location = new Point(Width - bShowInfo.Width - 20, bShowInfo.Location.Y);
            pInfo.Location = new Point(Width - pInfo.Width - 20, Convert.ToInt32(bShowInfo.Location.Y + bShowInfo.Size.Height + 10 * size));

            bOpenModelFile.Font = tmpFont;
            bShowInfo.Font = tmpFont;
            rtbInfo.Font = tmpFont;
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            ControlUpdate();
        }

        private void bShowInfo_Click(object sender, EventArgs e)
        {
            pInfo.Visible = !pInfo.Visible;
        }

        private readonly Random rand = new();
        private void AutoRotate()
        {
            if (modelData != null)
            {
                const int COUNT_RAND = 10;
                const float SHIFT_AXIS = (float)Math.PI / 1000;
                camera.AngelsRotate.X += SHIFT_AXIS * (1 + rand.Next(COUNT_RAND));
                camera.AngelsRotate.Y += SHIFT_AXIS * (1 + rand.Next(COUNT_RAND));
                camera.AngelsRotate.Z += SHIFT_AXIS * (1 + rand.Next(COUNT_RAND));
                CreateModelDataPaint();
            }
        }
    }
}
