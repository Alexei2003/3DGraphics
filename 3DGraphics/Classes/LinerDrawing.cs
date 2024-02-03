using static _3DGraphics.Classes.BaseGraphisStructs;

namespace _3DGraphics.Classes
{
    internal class LinerDrawing
    {
        public static void DrawLines(Bitmap bitmap, GeometricVertex[] GeometricVertexСoordinates, int[][] GeometricVertexIndexs)
        {
            const int xShift = 600 / 2 + 100;
            const int yShift = 600 / 2 + 500;
            const int scale = 5;
            using var g = Graphics.FromImage(bitmap);
            foreach (var vertexIndex in GeometricVertexIndexs)
            {
                var points = new PointF[vertexIndex.Length];
                for (var i = 0; i < vertexIndex.Length; i++)
                {
                    points[i] = new PointF(GeometricVertexСoordinates[vertexIndex[i]].X * scale + xShift, GeometricVertexСoordinates[vertexIndex[i]].Y * scale + yShift);
                }

                for (var i = 0; i < vertexIndex.Length - 1; i++)
                {
                    g.DrawLine(new Pen(Color.Black), points[i], points[i + 1]);
                }
                g.DrawLine(new Pen(Color.Black), points[vertexIndex.Length - 1], points[0]);
            }
        }
    }
}
