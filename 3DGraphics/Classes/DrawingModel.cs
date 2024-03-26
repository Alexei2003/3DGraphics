using System.Drawing.Imaging;
using static _3DGraphics.Classes.BaseGraphisStructs;

namespace _3DGraphics.Classes
{
    internal static class DrawingModel
    {
        public unsafe delegate void DrawObject(Point[] points, int[] vertexIndex, int* rgbBitmap, BitmapData bitmapData, int colorInt, int widthZone, int heightZone);

        public static unsafe void Draw(Bitmap bitmap, GeometricVertex[] GeometricVertexСoordinates, int[][] GeometricVertexIndexs)
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
            switch (SwitchLab.Draw.Line)
            {
                case SwitchLab.Draw.Line:
                    drawObject = DrawLines;
                    break;
                case SwitchLab.Draw.LineRGB:
                    drawObject = DrawLinesRGB;
                    break;
                case SwitchLab.Draw.Triangles:
                    drawObject = DrawLineTriangles;
                    break;
            }
            Parallel.ForEach(GeometricVertexIndexs, vertexIndex =>
            {
                var points = new Point[vertexIndex.Length];
                for (var i = 0; i < vertexIndex.Length; i++)
                {
                    ref var coordinate = ref GeometricVertexСoordinates[vertexIndex[i]];
                    if (coordinate.X > widthMaxReder || widthMinReder > coordinate.X || coordinate.Y > heightMaxReder || heightMinReder > coordinate.Y)
                    {
                        points = null;
                        break;
                    }
                    points[i] = new Point(Convert.ToInt32(coordinate.X), Convert.ToInt32(coordinate.Y));
                }

                drawObject(points, vertexIndex, rgbBitmap, bitmapData, colorInt, widthZone, heightZone);
            });
            bitmap.UnlockBits(bitmapData);
        }

        private static unsafe void DrawLine(int* rgbBitmap, int stride, int colorInt, Point point1, Point point2, int widthZone, int heightZone)
        {
            int dx = point2.X - point1.X;
            int dy = point2.Y - point1.Y;
            int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

            float XIncrement = dx / (float)steps;
            float YIncrement = dy / (float)steps;

            float X = point1.X;
            float Y = point1.Y;
            int index;

            var strideInt = stride / 4;

            for (var i = 0; i <= steps; i++)
            {
                if (X > widthZone || X < 0 || Y > heightZone || Y < 0)
                {
                    X += XIncrement;
                    Y += YIncrement;
                    continue;
                }

                index = ((int)Math.Round(X)) + ((int)Math.Round(Y) * strideInt);
                rgbBitmap[index] = colorInt;

                X += XIncrement;
                Y += YIncrement;
            }
        }

        public static unsafe void DrawLines(Point[] points, int[] vertexIndex, int* rgbBitmap, BitmapData bitmapData, int colorInt, int widthZone, int heightZone)
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

        public static unsafe void DrawLinesRGB(Point[] points, int[] vertexIndex, int* rgbBitmap, BitmapData bitmapData, int colorInt, int widthZone, int heightZone)
        {
            if (points != null)
            {
                int r = 0;
                int g = 0;
                int b = 0;

                var rand = new Random();

                for (var i = 0; i < vertexIndex.Length - 1; i++)
                {
                    r += points[i].X + rand.Next(100);
                    g += points[i].Y + rand.Next(100);
                    b += points[i].X + points[i].Y + rand.Next(100);
                }

                colorInt = Color.FromArgb(255, Math.Abs(r % 255), Math.Abs(g % 255), Math.Abs(b % 255)).ToArgb();
                for (var i = 0; i < vertexIndex.Length - 1;)
                {
                    DrawLine(rgbBitmap, bitmapData.Stride, colorInt, points[i], points[++i], widthZone, heightZone);
                }
                DrawLine(rgbBitmap, bitmapData.Stride, colorInt, points[vertexIndex.Length - 1], points[0], widthZone, heightZone);
            }
        }

        public static unsafe void DrawLineTriangles(Point[] points, int[] vertexIndex, int* rgbBitmap, BitmapData bitmapData, int colorInt, int widthZone, int heightZone)
        {

        }
    }
}