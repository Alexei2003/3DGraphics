using System.Drawing.Imaging;
using static _3DGraphics.Classes.BaseGraphisStructs;

namespace _3DGraphics.Classes
{
    internal static class LinerDrawing
    {
        public static void DrawLines(Bitmap bitmap, GeometricVertex[] GeometricVertexСoordinates, int[][] GeometricVertexIndexs, CoordinateVector coordinateTransformationlateVector)
        {
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
                    points[i] = new Point(Convert.ToInt32(coordinate.X + coordinateTransformationlateVector.X), Convert.ToInt32(coordinate.Y + coordinateTransformationlateVector.Y));
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

        private static void DrawLine(byte[] rgbValues, int stride, byte[] colorBytes, Point point1, Point point2, int widthZone, int heightZone)
        {
            int dx = point2.X - point1.X;
            int dy = point2.Y - point1.Y;
            int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

            float XIncrement = dx / (float)steps;
            float YIncrement = dy / (float)steps;

            float X = point1.X;
            float Y = point1.Y;
            int index;

            for (var i = 0; i <= steps; i++)
            {
                if (X > widthZone - 1 || X < 0 || Y > heightZone - 1 || Y < 0)
                {
                    X += XIncrement;
                    Y += YIncrement;
                    continue;
                }

                index = ((int)Math.Round(X) * 4) + ((int)Math.Round(Y) * stride);
                Buffer.BlockCopy(colorBytes, 0, rgbValues, index, 4);

                X += XIncrement;
                Y += YIncrement;
            }
        }
    }
}