using System.Drawing.Imaging;
using static _3DGraphics.Classes.BaseGraphisStructs;

namespace _3DGraphics.Classes
{
    internal class LinerDrawing
    {
        public static void DrawLines(Bitmap bitmap, GeometricVertex[] GeometricVertexСoordinates, int[][] GeometricVertexIndexs)
        {
            const int xShift = 600 / 2 + 100;
            const int yShift = 600 / 2 + 300;
            const int scale = 2;

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            IntPtr ptr = bitmapData.Scan0;
            int bytes = Math.Abs(bitmapData.Stride) * bitmap.Height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            Parallel.ForEach(GeometricVertexIndexs, vertexIndex =>
            {
                var points = new Point[vertexIndex.Length];
                for (var i = 0; i < vertexIndex.Length; i++)
                {
                    ref var coordinate = ref GeometricVertexСoordinates[vertexIndex[i]];
                    points[i] = new Point(Convert.ToInt32((coordinate.X * scale) + xShift), Convert.ToInt32((coordinate.Y * scale) + yShift));
                }

                for (var i = 0; i < vertexIndex.Length - 1; i++)
                {
                    DrawLine(rgbValues, bitmapData.Stride, Color.Black, points[i], points[i + 1]);
                }
                DrawLine(rgbValues, bitmapData.Stride, Color.Black, points[vertexIndex.Length - 1], points[0]);
            });

            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            bitmap.UnlockBits(bitmapData);
        }

        private static void DrawLine(byte[] rgbValues, int stride, Color color, Point point1, Point point2)
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
        }
    }
}
