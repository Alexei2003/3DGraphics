using System.Drawing.Imaging;
using Windows.UI.WebUI;
using static _3DGraphics.Drawing.DrawingModel;

namespace _3DGraphics.Drawing
{
    internal static class Lines
    {
        public static unsafe void Draw(Point3DF[] points, int* rgbBitmap, BitmapData bitmapData, int colorInt, int widthZone, int heightZone)
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

        public static unsafe void DrawRGB(Point3DF[] points, int* rgbBitmap, BitmapData bitmapData, int colorInt, int widthZone, int heightZone)
        {
            if (points != null)
            {
                colorInt = DrawingModel.GetRGBColor(points);
                Draw(points, rgbBitmap, bitmapData, colorInt, widthZone, heightZone);
            }
        }

        public static unsafe void DrawLine(int* rgbBitmap, int stride, int colorInt, Point3DF point1, Point3DF point2, int widthZone, int heightZone)
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

        public static unsafe void DrawLineWithZBuffer(int* rgbBitmap, int stride, int colorInt, Point3DF point1, Point3DF point2, int widthZone, int heightZone)
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

            var dz = point2.Z - point1.Z;
            var ZIncrement = dz / (float)steps;

            var z = point1.Z;

            for (var i = 0; i <= steps; i++)
            {
                if (x > widthZone || x < 0 || y > heightZone || y < 0)
                {
                    x += XIncrement;
                    y += YIncrement;
                    z += ZIncrement;
                    continue;
                }

                if(ZBuffer.CheckAndSetDistance((int)Math.Round(x), (int)Math.Round(y), z))
                {
                    index = (int)Math.Round(x) + (int)Math.Round(y) * strideInt;

                    rgbBitmap[index] = colorInt;
                }

                x += XIncrement;
                y += YIncrement;
                z += ZIncrement;
            }
        }
    }
}
