using System.Drawing.Imaging;

namespace _3DGraphics.Drawing
{
    internal static class Triangles
    {
        public static unsafe void Draw(PointF[] points, int* rgbBitmap, BitmapData bitmapData, int colorInt, int widthZone, int heightZone)
        {
            if (points != null)
            {
                Array.Sort(points, (p1, p2) => p2.Y.CompareTo(p1.Y));


                for (var j = 0; j < points.Length - 2; j++)
                {
                    DrawTriangle([points[j], points[j + 1], points[j + 2]], rgbBitmap, bitmapData, colorInt, widthZone, heightZone);
                }
            }
        }

        public static unsafe void DrawRGB(PointF[] points, int* rgbBitmap, BitmapData bitmapData, int colorInt, int widthZone, int heightZone)
        {
            if (points != null)
            {
                colorInt = DrawingModel.GetRGBColor(points);
                Draw(points, rgbBitmap, bitmapData, colorInt, widthZone, heightZone);
            }
        }

        // 1-short, 2-long
        private static unsafe void DrawTriangle(PointF[] points, int* rgbBitmap, BitmapData bitmapData, int colorInt, int widthZone, int heightZone)
        {
            const int YIncrement = 1;

            float y = points[0].Y;
            float x1 = points[0].X;
            float x2 = points[0].X;
            float yChangeX = points[1].Y;

            var dx1 = points[1].X - points[0].X;
            var dx2 = points[2].X - points[0].X;

            var steps1 = Convert.ToInt32(points[0].Y - points[1].Y);
            var steps2 = Convert.ToInt32(points[0].Y - points[2].Y);

            steps1 = steps1 == 0 ? 1 : steps1;
            steps2 = steps2 == 0 ? 1 : steps2;

            var XIncrement1 = dx1 / steps1;
            var XIncrement2 = dx2 / steps2;

            var steps = steps1 > steps2 ? steps1 : steps2;

            var p1Line = new PointF();
            var p2Line = new PointF();

            for (var i = 0; i <= steps; i++)
            {
                if (y <= yChangeX)
                {
                    x1 = points[1].X;
                    dx1 = points[2].X - points[1].X;
                    steps1 = Convert.ToInt32(points[1].Y - points[2].Y);
                    XIncrement1 = dx1 / steps1;
                    yChangeX = points[2].Y - 1;
                }

                if (x1 == float.PositiveInfinity || x2 == float.PositiveInfinity || y == float.PositiveInfinity)
                {
                    continue;
                }

                p1Line.X = x1;
                p1Line.Y = y;
                p2Line.X = x2;
                p2Line.Y = y;

                Lines.DrawLine(rgbBitmap, bitmapData.Stride, colorInt, p1Line, p2Line, widthZone, heightZone);

                x1 += XIncrement1;
                x2 += XIncrement2;
                y -= YIncrement;
            }
        }
    }
}
