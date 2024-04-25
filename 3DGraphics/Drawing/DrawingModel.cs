using _3DGraphics.Classes;
using System.Drawing.Imaging;
using System.Numerics;
using Windows.Media.Devices;
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
                //Parallel.ForEach(geometricVertexIndexs, vertexIndex =>
                foreach (var vertexIndex in geometricVertexIndexs)
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
                //});
                }
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

        public static unsafe int GetRGBLight(Point3DF[] points)
        {
            int light = 0;

            //ABC
            var ab = new Vector3(points[1].X - points[0].X, points[1].Y - points[0].Y, points[1].Z - points[0].Z);
            var ac = new Vector3(points[2].X - points[0].X, points[2].Y - points[0].Y, points[2].Z - points[0].Z);

            float n1 = (ab.Y * ac.Z) - (ab.Z*ac.Y);
            float n2 = ab.Z * ac.X - ab.X * ac.Z;
            float n3 = ab.X * ac.Y - ab.Y * ac.X;

            var normA = Vector3.Normalize(new Vector3(n1, n2, n3));

            var normCameraA = Vector3.Normalize(new Vector3(points[0].X - Camera.Eye.X, points[0].Y - Camera.Eye.Y, points[0].Z - Camera.Eye.Z));

            var dotNormalA = Vector3.Dot(normA,normCameraA);

            var lengthNormA = float.Sqrt(float.Pow(normA.X,2) + float.Pow(normA.Y, 2) + float.Pow(normA.Z, 2));
            var lengthnormCameraA = float.Sqrt(float.Pow(normCameraA.X, 2) + float.Pow(normCameraA.Y, 2) + float.Pow(normCameraA.Z, 2));

            var cosA = dotNormalA / (lengthnormCameraA * lengthnormCameraA);


            light = Convert.ToInt32(255 * (float.Abs(cosA)) % 255);

            return Color.FromArgb(255, Math.Abs(light), Math.Abs(light), Math.Abs(light)).ToArgb();
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