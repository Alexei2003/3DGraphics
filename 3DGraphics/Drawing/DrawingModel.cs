using _3DGraphics.Classes;
using System.Drawing.Imaging;

namespace _3DGraphics.Drawing
{
    internal static class DrawingModel
    {
        public delegate void DrawObject(DrawingParams @params);

        public static unsafe void Draw(Bitmap bitmap, ObjFileReader.ModelData modelData)
        {
            ZBuffer.Clear();

            var widthZone = bitmap.Width - 1;
            var heightZone = bitmap.Height - 1;

            var widthMaxReder = bitmap.Width * 2;
            var heightMaxReder = bitmap.Height * 2;

            var widthMinReder = -bitmap.Width;
            var heightMinReder = -bitmap.Height;


            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var ptr = bitmapData.Scan0;

            var rgbBitmap = (int*)ptr;

            // Цвет в int
            var colorInts = new List<int>();

            var drawObjectFuncs = new List<DrawObject>();
            switch (SettingLab.DrawModel)
            {
                case SettingLab.DrawModelType.Lines:
                    drawObjectFuncs.Add(Lines.Draw);
                    colorInts.Add(Color.White.ToArgb());
                    break;
                case SettingLab.DrawModelType.LinesRGB:
                    drawObjectFuncs.Add(Lines.DrawRGB);
                    colorInts.Add(Color.White.ToArgb());
                    break;
                case SettingLab.DrawModelType.Triangles:
                    drawObjectFuncs.Add(Triangles.Draw);
                    colorInts.Add(Color.White.ToArgb());
                    break;
                case SettingLab.DrawModelType.TrianglesRGB:
                    drawObjectFuncs.Add(Triangles.DrawRGB);
                    colorInts.Add(Color.White.ToArgb());
                    break;
                case SettingLab.DrawModelType.TrianglesLines:
                    drawObjectFuncs.Add(Triangles.Draw);
                    colorInts.Add(Color.White.ToArgb());
                    drawObjectFuncs.Add(Lines.Draw);
                    colorInts.Add(Color.Red.ToArgb());
                    break;

            }

            for (var j = 0; j < drawObjectFuncs.Count; j++)
            {
                for(var index = 0; index< modelData.GeometricVertexIndexs.Length; index++) 
                //Parallel.For(0, modelData.GeometricVertexIndexs.Length, index =>
                {
                    var geometricPoints = new BaseGraphisStructs.CoordinateVector[modelData.GeometricVertexIndexs[index].Length];
                    var geometricToNormalPoints = new BaseGraphisStructs.CoordinateVector[modelData.GeometricVertexIndexs[index].Length];
                    var normalPoints = new BaseGraphisStructs.NormalVector[modelData.NormalVertexIndexs[index].Length];
                    for (var i = 0; i < modelData.GeometricVertexIndexs[index].Length; i++)
                    {
                        ref var geometricCoordinate = ref modelData.GeometricVertexCoordinates[modelData.GeometricVertexIndexs[index][i]];
                        if (geometricCoordinate.X > widthMaxReder || widthMinReder > geometricCoordinate.X || geometricCoordinate.Y > heightMaxReder || heightMinReder > geometricCoordinate.Y)
                        {
                            geometricPoints = null;
                            break;
                        }
                        geometricPoints[i] = geometricCoordinate;
                        geometricToNormalPoints[i] = modelData.GeometricVertexToNormalCoordinates[modelData.GeometricVertexIndexs[index][i]];
                        normalPoints[i] = modelData.NormalVertexCoordinates[modelData.NormalVertexIndexs[index][i]];
                    }

                    if (geometricPoints != null)
                    {
                        drawObjectFuncs[j](new DrawingParams()
                        {
                            Coordinate = geometricPoints,
                            CoordinateToNormal = geometricToNormalPoints,
                            Normal = normalPoints,
                            RgbBitmap = rgbBitmap,
                            Stride = bitmapData.Stride,
                            ColorInt = colorInts[j],
                            WidthZone = widthZone,
                            HeightZone = heightZone,
                        });
                    }
                //});
                }
            }

            bitmap.UnlockBits(bitmapData);
        }

        public static int GetRGBColor(BaseGraphisStructs.CoordinateVector[] points)
        {
            int r = 0;
            int g = 0;
            int b = 0;

            for (var i = 0; i < points.Length - 1; i++)
            {
                r += Convert.ToInt32(points[i].X);
                g += Convert.ToInt32(points[i].Y);
                b += Convert.ToInt32(points[i].Z);
            }
            return Color.FromArgb(255, Math.Abs(r % 255), Math.Abs(g % 255), Math.Abs(b % 255)).ToArgb();
        }
    }
}