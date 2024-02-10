using System.Drawing.Imaging;
using static _3DGraphics.Classes.BaseGraphisStructs;

namespace _3DGraphics.Classes
{
    internal static class LinerDrawing
    {
        public static void DrawLines(Bitmap bitmap, GeometricVertex[] GeometricVertexСoordinates, int[][] GeometricVertexIndexs)
        {
            const int scale = 2;
            int widthZone = bitmap.Width;
            int heightZone = bitmap.Height;

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            IntPtr ptr = bitmapData.Scan0;
            int bytes = Math.Abs(bitmapData.Stride) * bitmap.Height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            int colorArgb = Color.Black.ToArgb();
            byte[] colorBytes = BitConverter.GetBytes(colorArgb);

            Parallel.ForEach(GeometricVertexIndexs, vertexIndex =>
            {
                var points = new Point[vertexIndex.Length];
                for (var i = 0; i < vertexIndex.Length; i++)
                {
                    ref var coordinate = ref GeometricVertexСoordinates[vertexIndex[i]];
                    points[i] = new Point(Convert.ToInt32((coordinate.X + coordinate.TranslateX) * scale), Convert.ToInt32((coordinate.Y + coordinate.TranslateY) * scale));
                }

                for (var i = 0; i < vertexIndex.Length - 1;)
                {
                    DrawLine(rgbValues, bitmapData.Stride, colorBytes, points[i], points[++i], widthZone, heightZone);
                }
                DrawLine(rgbValues, bitmapData.Stride, colorBytes, points[vertexIndex.Length - 1], points[0], widthZone, heightZone);
            });

            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            bitmap.UnlockBits(bitmapData);
        }

        /*private static void DrawLine(byte[] rgbValues, int stride, Color color, Point point1, Point point2)
        {
            int x1 = point1.X;
            int y1 = point1.Y;
            int x2 = point2.X;
            int y2 = point2.Y;

            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = (x1 < x2) ? 1 : -1;
            int sy = (y1 < y2) ? 1 : -1;
            int err = dx - dy;
            int index;
            int e2;

            while (true)
            {
                index = (x1 * 4) + (y1 * stride);
                Buffer.BlockCopy(BitConverter.GetBytes(color.ToArgb()), 0, rgbValues, index, 4);

                if (x1 == x2 && y1 == y2)
                    break;

                e2 = err << 1;
                if (e2 > -dy)
                {
                    err -= dy;
                    x1 += sx;
                }
                else
                {
                    if (e2 < dx)
                    {
                        err += dx;
                        y1 += sy;
                    }
                }
            }
        }*/

        private static void DrawLine(byte[] rgbValues, int stride, byte[] colorBytes, Point point1, Point point2, int widthZone, int hightZone)
        {
            int dx = point2.X - point1.X;
            int dy = point2.Y - point1.Y;
            int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

            float Xincrement = dx / (float)steps;
            float Yincrement = dy / (float)steps;

            float X = point1.X;
            float Y = point1.Y;
            int index;

            for (int i = 0; i <= steps; i++)
            {
                if (X > widthZone - 1 || X < 0 || Y > hightZone - 1 || Y < 0)
                {
                    X += Xincrement;
                    Y += Yincrement;
                    continue;
                }

                index = ((int)Math.Round(X) * 4) + ((int)Math.Round(Y) * stride);
                Buffer.BlockCopy(colorBytes, 0, rgbValues, index, 4);

                X += Xincrement;
                Y += Yincrement;
            }
        }
    }
}

