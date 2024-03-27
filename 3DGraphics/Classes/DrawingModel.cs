using System.Drawing.Imaging;
using static _3DGraphics.Classes.BaseGraphisStructs;

namespace _3DGraphics.Classes
{
    internal static class DrawingModel
    {
        public unsafe delegate void DrawObject(PointF[] points, int[] vertexIndex, int* rgbBitmap, BitmapData bitmapData, int colorInt, int widthZone, int heightZone);

        public static unsafe void Draw(Bitmap bitmap, GeometricVertex[] geometricVertexСoordinates, int[][] geometricVertexIndexs)
        {
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
            var colorInt = Color.White.ToArgb();

            DrawObject drawObject;
            switch (SwitchLab.Draw.Triangles)
            {
                case SwitchLab.Draw.Line:
                    drawObject = DrawLines;
                    break;
                case SwitchLab.Draw.LineRGB:
                    drawObject = DrawLinesRGB;
                    break;
                case SwitchLab.Draw.Triangles:
                    drawObject = DrawTriangles;
                    break;
            }

            ///

            const int SHIFT = 100;

            var points = new PointF[]
            {
                new PointF(SHIFT,SHIFT),
                new PointF(SHIFT*2,SHIFT),
                new PointF(SHIFT*2,SHIFT * 2),
                new PointF(SHIFT*3,SHIFT*2),
            };


            drawObject(points, new int[4], rgbBitmap, bitmapData, colorInt, widthZone, heightZone);

            ///

            /*            Parallel.ForEach(geometricVertexIndexs, vertexIndex =>
                        {
                            var points = new PointF[vertexIndex.Length];
                            for (var i = 0; i < vertexIndex.Length; i++)
                            {
                                ref var coordinate = ref geometricVertexСoordinates[vertexIndex[i]];
                                if (coordinate.X > widthMaxReder || widthMinReder > coordinate.X || coordinate.Y > heightMaxReder || heightMinReder > coordinate.Y)
                                {
                                    points = null;
                                    break;
                                }
                                points[i] = new PointF(coordinate.X, coordinate.Y);
                            }

                            drawObject(points, vertexIndex, rgbBitmap, bitmapData, colorInt, widthZone, heightZone);
                        });*/

            bitmap.UnlockBits(bitmapData);
        }

        private static unsafe void DrawLine(int* rgbBitmap, int stride, int colorInt, PointF point1, PointF point2, int widthZone, int heightZone)
        {
            var dx = point2.X - point1.X;
            var dy = point2.Y - point1.Y;
            var steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

            var XIncrement = dx / (float)steps;
            var YIncrement = dy / (float)steps;

            float x = point1.X;
            float y = point1.Y;
            int index;

            var strideInt = stride / 4;

            for (var i = 0; i <= steps; i++)
            {
                if (x > widthZone || x < 0 || y > heightZone || y < 0)
                {
                    x += XIncrement;
                    y += YIncrement;
                    continue;
                }

                index = ((int)Math.Ceiling(x)) + ((int)Math.Ceiling(y) * strideInt);
                rgbBitmap[index] = colorInt;

                x += XIncrement;
                y += YIncrement;
            }
        }

        public static unsafe void DrawLines(PointF[] points, int[] vertexIndex, int* rgbBitmap, BitmapData bitmapData, int colorInt, int widthZone, int heightZone)
        {
            if (points != null)
            {
                for (var i = 0; i < vertexIndex.Length - 1;)
                {
                    DrawLine(rgbBitmap, bitmapData.Stride, colorInt, points[i], points[++i], widthZone, heightZone);
                }
                DrawLine(rgbBitmap, bitmapData.Stride, colorInt, points[vertexIndex.Length - 1], points[0], widthZone, heightZone);
            }
        }

        public static unsafe void DrawLinesRGB(PointF[] points, int[] vertexIndex, int* rgbBitmap, BitmapData bitmapData, int colorInt, int widthZone, int heightZone)
        {
            if (points != null)
            {
                int r = 0;
                int g = 0;
                int b = 0;

                var rand = new Random();

                for (var i = 0; i < vertexIndex.Length - 1; i++)
                {
                    r += Convert.ToInt32(points[i].X + rand.Next(100));
                    g += Convert.ToInt32(points[i].Y + rand.Next(100));
                    b += Convert.ToInt32(points[i].X + points[i].Y + rand.Next(100));
                }

                colorInt = Color.FromArgb(255, Math.Abs(r % 255), Math.Abs(g % 255), Math.Abs(b % 255)).ToArgb();
                for (var i = 0; i < vertexIndex.Length - 1;)
                {
                    DrawLine(rgbBitmap, bitmapData.Stride, colorInt, points[i], points[++i], widthZone, heightZone);
                }
                DrawLine(rgbBitmap, bitmapData.Stride, colorInt, points[vertexIndex.Length - 1], points[0], widthZone, heightZone);
            }
        }

        public static unsafe void DrawTriangles(PointF[] points, int[] vertexIndex, int* rgbBitmap, BitmapData bitmapData, int colorInt, int widthZone, int heightZone)
        {
            if (points != null)
            {
                //Array.Sort(points, (p1, p2) => p1.X.CompareTo(p2.X));
                Array.Sort(points, (p1, p2) => p2.Y.CompareTo(p1.Y));

                int YIncrement = 1;

                for (var indP = 0; indP < vertexIndex.Length - 2; indP++)
                {
                    float y = points[indP].Y;
                    float x1 = points[indP].X;
                    float x2 = points[indP].X;
                    float minY = points[indP + 1].Y;

                    var dx1 = points[indP + 1].X - points[indP].X;
                    var dx2 = points[indP + 2].X - points[indP].X;

                    var steps = points[indP].Y - points[indP + 2].Y;
                    var XIncrement1 = dx1 / (float)steps;
                    var XIncrement2 = dx2 / (float)steps;

                    for (var i = 0; i <= steps; i++)
                    {
                        if (y <= minY)
                        {
                            x1 = points[indP + 1].X;
                            dx1 = points[indP + 2].X - points[indP + 1].X;
                            XIncrement1 = dx1 / (float)steps;
                            minY = points[indP + 2].Y;
                        }

                        DrawLine(rgbBitmap, bitmapData.Stride, colorInt, new PointF(x1, y), new PointF(x2, y), widthZone, heightZone);

                        x1 += XIncrement1;
                        x2 += XIncrement2;
                        y -= YIncrement;
                    }
                }
            }
        }
    }
}