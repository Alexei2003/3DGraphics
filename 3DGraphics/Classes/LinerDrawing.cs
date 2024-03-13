using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using static _3DGraphics.Classes.BaseGraphisStructs;

namespace _3DGraphics.Classes
{
    internal static class LinerDrawing
    {
        public static unsafe void DrawLines(Bitmap bitmap, GeometricVertex[] GeometricVertexСoordinates, int[][] GeometricVertexIndexs)
        {
            var widthZone = bitmap.Width;
            var heightZone = bitmap.Height;

            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var ptr = bitmapData.Scan0;
            var bytes = Math.Abs(bitmapData.Stride) * bitmap.Height;

            var rgbBitmap = (int*)ptr;

            // Цвет в байтах
            var colorInt = Color.Black.ToArgb();

            Parallel.ForEach(GeometricVertexIndexs, vertexIndex =>
            {
                var points = new Point[vertexIndex.Length];
                for (var i = 0; i < vertexIndex.Length; i++)
                {
                    ref var coordinate = ref GeometricVertexСoordinates[vertexIndex[i]];
                    points[i] = new Point(Convert.ToInt32(coordinate.X), Convert.ToInt32(coordinate.Y));
                }

                for (var i = 0; i < vertexIndex.Length - 1;)
                {
                    DrawLine(rgbBitmap, bitmapData.Stride, colorInt, points[i], points[++i], widthZone, heightZone);
                }
                DrawLine(rgbBitmap, bitmapData.Stride, colorInt, points[vertexIndex.Length - 1], points[0], widthZone, heightZone);
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
                if (X > widthZone - 1 || X < 0 || Y > heightZone - 1 || Y < 0)
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
    }
}