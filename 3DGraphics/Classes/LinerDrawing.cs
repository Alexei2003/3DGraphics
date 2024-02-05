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
            using var g = Graphics.FromImage(bitmap);
            foreach (var vertexIndex in GeometricVertexIndexs)
            {
                var points = new Point[vertexIndex.Length];
                for (var i = 0; i < vertexIndex.Length; i++)
                {
                    ref var coordinate = ref GeometricVertexСoordinates[vertexIndex[i]];
                    points[i] = new Point(Convert.ToInt32(coordinate.X * scale + xShift), Convert.ToInt32(coordinate.Y * scale + yShift));
                }

                for (var i = 0; i < vertexIndex.Length - 1; i++)
                {
                    DrawLine(bitmap, Color.Black, points[i], points[i + 1]);
                }
                DrawLine(bitmap, Color.Black, points[vertexIndex.Length - 1], points[0]);
            }
        }



        private static void DrawLine(Bitmap bitmap, Color color, Point point1, Point point2)
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

            while (true)
            {
                bitmap.SetPixel(x1, y1, color);

                if (x1 == x2 && y1 == y2)
                    break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x1 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y1 += sy;
                }
            }
        }
    }
}
