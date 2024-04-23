using System.Drawing.Imaging;

namespace _3DGraphics.Drawing
{
    internal static class Lines
    {
        public static unsafe void Draw(PointF[] points, int* rgbBitmap, BitmapData bitmapData, int colorInt, int widthZone, int heightZone)
        {
            if (points != null)
            {
                for (var i = 0; i < points.Length - 1;)
                {
                    DrawLine(rgbBitmap, bitmapData.Stride, colorInt, points[i], points[++i], widthZone, heightZone);
                }
                DrawLine(rgbBitmap, bitmapData.Stride, colorInt, points[points.Length - 1], points[0], widthZone, heightZone);
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

        public static unsafe void DrawLine(int* rgbBitmap, int stride, int colorInt, PointF point1, PointF point2, int widthZone, int heightZone)
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

                index = (int)Math.Ceiling(x) + (int)Math.Ceiling(y) * strideInt;

                rgbBitmap[index] = colorInt;

                x += XIncrement;
                y += YIncrement;
            }
        }
    }
}
