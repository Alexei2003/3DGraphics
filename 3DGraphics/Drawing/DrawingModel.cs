using System.Drawing.Imaging;
using static _3DGraphics.Classes.BaseGraphisStructs;

namespace _3DGraphics.Drawing
{
    internal static class DrawingModel
    {
        public unsafe delegate void DrawObject(Point3DF[] points, int* rgbBitmap, BitmapData bitmapData, int colorInt, int widthZone, int heightZone);

        public static unsafe void Draw(Bitmap bitmap, GeometricVertex[] geometricVertexСoordinates, int[][] geometricVertexIndexs)
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

            const int SHIFT = 100;

            //geometricVertexСoordinates = [new GeometricVertex(0+ SHIFT, 0 + SHIFT, 0,0), new GeometricVertex(200 + SHIFT, 0 + SHIFT, 0, 0), new GeometricVertex(300 + SHIFT, 100 + SHIFT, 0, 0), new GeometricVertex(100 + SHIFT, 100 + SHIFT, 0, 0),];
            //geometricVertexСoordinates = [new GeometricVertex(100 + SHIFT, 0 + SHIFT, 0, 0), new GeometricVertex(300 + SHIFT, 0 + SHIFT, 0, 0), new GeometricVertex(0 + SHIFT, 100 + SHIFT, 0, 0), new GeometricVertex(200 + SHIFT, 100 + SHIFT, 0, 0),];

            //geometricVertexСoordinates = [new GeometricVertex(100 + SHIFT, 0 + SHIFT, 0, 0), new GeometricVertex(100 + SHIFT, 300 + SHIFT, 0, 0), new GeometricVertex(0 + SHIFT, 200 + SHIFT, 0, 0), new GeometricVertex(0 + SHIFT, 100 + SHIFT, 0, 0),];

            //geometricVertexСoordinates = [new GeometricVertex(100 + SHIFT, 0 + SHIFT, 0, 0), new GeometricVertex(100 + SHIFT, 200 + SHIFT, 0, 0), new GeometricVertex(0 + SHIFT, 300 + SHIFT, 0, 0), new GeometricVertex(0 + SHIFT, 100 + SHIFT, 0, 0),];

            //geometricVertexIndexs = [[0,1,2,3]];

/*            geometricVertexСoordinates = [
                new GeometricVertex(0 + SHIFT, 0 + SHIFT, -15, 0), new GeometricVertex(400 + SHIFT, 0 + SHIFT, 0, 0), new GeometricVertex(0 + SHIFT, 400 + SHIFT, 0, 0), new GeometricVertex(400 + SHIFT, 400 + SHIFT, 0, 0),
                new GeometricVertex(100 + SHIFT, 0 + SHIFT, -10, 0), new GeometricVertex(100 + SHIFT, 200 + SHIFT, -10, 0), new GeometricVertex(0 + SHIFT, 300 + SHIFT, -10, 0), new GeometricVertex(0 + SHIFT, 100 + SHIFT, -10, 0),
            ];

            geometricVertexIndexs = [[0, 1, 2, 3], [4, 5, 6, 7]];*/

            for (var j = 0; j < drawObjectFuncs.Count; j++)
            {
                Parallel.ForEach(geometricVertexIndexs, vertexIndex =>
                //foreach (var vertexIndex in geometricVertexIndexs)
                {
                    var points = new Point3DF[vertexIndex.Length];
                    for (var i = 0; i < vertexIndex.Length; i++)
                    {
                        ref var coordinate = ref geometricVertexСoordinates[vertexIndex[i]];
                        if (coordinate.X > widthMaxReder || widthMinReder > coordinate.X || coordinate.Y > heightMaxReder || heightMinReder > coordinate.Y)
                        {
                            points = null;
                            break;
                        }
                        points[i] = new Point3DF(coordinate.X, coordinate.Y, coordinate.Z);
                    }

                    drawObjectFuncs[j](points, rgbBitmap, bitmapData, colorInts[j], widthZone, heightZone);
                });
                //}
            }

            bitmap.UnlockBits(bitmapData);
        }

        public static unsafe int GetRGBColor(Point3DF[] points)
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

        public struct Point3DF
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }

            public Point3DF(float x, float y, float z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public Point3DF(PointF p, float z)
            {
                X = p.X;
                Y = p.Y;
                Z = z;
            }
        }

    }
}