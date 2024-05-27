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
            ZBuffer.Resize(WIDTH, Height);

            Camera.Resize(Width, Height);

            var timerCallback = new TimerCallback(UpdateFPS);
            timerFPS = new System.Threading.Timer(timerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private void UpdateFPS(object state)
        {
            var strInfo = "" +
                    $"FPS = {frameCount}\n" +
                    $"Aspect = {Camera.Aspect}\n" +
                    $"Width = {Camera.Size.Width}\n" +
                    $"Height = {Camera.Size.Height}\n" +
                    $"Fov = {Camera.FovAngle}\n" +
                    $"Scale = {Camera.Scale.X}\n" +
                    $"ZCamera = {Camera.Eye.Z}\n" +
                    $"ZTarget = {Camera.Translate.Z}\n";

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
            try
            {
                var bitmap = new Bitmap(Width, Height);

                using (var g = Graphics.FromImage(bitmap))
                {
                    using var brush = new SolidBrush(Color.Black);
                    g.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
                }

                if (modelDataPaint != null)
                {
                    DrawingModel.Draw(bitmap, modelDataPaint);

                    lock (lockFrameCount)
                    {
                        frameCount++;
                    }
                }
                BackgroundImage = bitmap;
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private void opfdModelFile_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var modelFilePath = opfdModelFile.FileName;
            modelData = Read(modelFilePath);

            if (modelData.GeometricVertexCoordinates.Length == 0)
            {
                modelData = null;
                return;
            }

            CoordinateTransformar.TranslateVectors(modelData.GeometricVertexCoordinates, new CoordinateVector(
                -(modelData.GeometricVertexCoordinates.Max(v => v.X) + modelData.GeometricVertexCoordinates.Min(v => v.X)) / 2,
                -(modelData.GeometricVertexCoordinates.Max(v => v.Y) + modelData.GeometricVertexCoordinates.Min(v => v.Y)) / 2,
                -(modelData.GeometricVertexCoordinates.Max(v => v.Z) + modelData.GeometricVertexCoordinates.Min(v => v.Z)) / 2));

            modelDataPaint = new ModelData(modelData);
            Camera.Resize(Width, Height);
            CreateModelDataPaint();
        }

        private void bOpenModelFile_Click(object sender, EventArgs e)
        {
            opfdModelFile.ShowDialog();
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            ControlResize();

            ZBuffer.Resize(Width, Height);

            Camera.Size = Size;
            CreateModelDataPaint();
            ControlUpdate();
        }

        private void CreateModelDataPaint()
        {
            if (modelData != null)
            {
                modelDataPaint.SetCopyValue(modelData);
                CoordinateTransformar.GetFinalVectors(modelDataPaint);
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (modelData != null)
            {
                const float angelRotate = (float)Math.PI / 10;
                const float cahngeAngelFov = 1f;
                var scale = new Vector3(1.05f, 1.05f, 1.05f);
                const int translate = 1;
                const int near = 5;

                switch (e.KeyCode)
                {
                    case Keys.X:
                        if ((Control.ModifierKeys & Keys.Shift) != 0)
                        {
                            CoordinateTransformar.RotateCamera(new Vector3(-angelRotate, 0, 0));
                        }
                        else
                        {
                            CoordinateTransformar.RotateCamera(new Vector3(angelRotate, 0, 0));
                        }
                        break;
                    case Keys.Y:
                        if ((Control.ModifierKeys & Keys.Shift) != 0)
                        {
                            CoordinateTransformar.RotateCamera(new Vector3(0, -angelRotate, 0));
                        }
                        else
                        {
                            CoordinateTransformar.RotateCamera(new Vector3(0, angelRotate, 0));
                        }
                        break;
                    case Keys.Z:
                        if ((Control.ModifierKeys & Keys.Shift) != 0)
                        {
                            CoordinateTransformar.RotateCamera(new Vector3(0, 0, -angelRotate));
                        }
                        else
                        {
                            CoordinateTransformar.RotateCamera(new Vector3(0, 0, angelRotate));
                        }
                        break;
                    case Keys.Oemplus:
                        Camera.Scale *= scale;
                        break;
                    case Keys.OemMinus:
                        Camera.Scale /= scale;
                        break;
                    case Keys.Up:
                        Camera.IncEyeDistance(-near);
                        break;
                    case Keys.Down:
                        Camera.IncEyeDistance(near);
                        break;
                    case Keys.Add:
                        Camera.IncFovAngle(cahngeAngelFov);
                        break;
                    case Keys.Subtract:
                        Camera.IncFovAngle(-cahngeAngelFov);
                        break;
                    case Keys.Q:
                        Camera.Translate.Z -= translate;
                        break;
                    case Keys.E:
                        Camera.Translate.Z += translate;
                        break;
                    case Keys.W:
                        Camera.Translate.Y += translate;
                        break;
                    case Keys.S:
                        Camera.Translate.Y -= translate;
                        break;
                    case Keys.A:
                        Camera.Translate.X -= translate;
                        break;
                    case Keys.D:
                        Camera.Translate.X += translate;
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

        private void AutoRotate()
        {
            if (modelData != null)
            {
                const float angelRotate = (float)Math.PI / 10;
                CoordinateTransformar.RotateCamera(new Vector3(0, angelRotate, 0));
                CreateModelDataPaint();
            }
        }
    }
}
