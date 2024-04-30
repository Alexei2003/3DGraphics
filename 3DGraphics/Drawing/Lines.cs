namespace _3DGraphics.Drawing
{
    internal static class Lines
    {
        public static void Draw(DrawingParams @params)
        {
            for (var i = 0; i < @params.Coordinate.Length - 1;)
            {
                @params.P1 = @params.Coordinate[i];
                @params.P2 = @params.Coordinate[++i];
                DrawLineWithZBuffer(@params);
            }
            @params.P1 = @params.Coordinate[@params.Coordinate.Length-1];
            @params.P2 = @params.Coordinate[0];
            DrawLineWithZBuffer(@params);
        }

        public static void DrawRGB(DrawingParams @params)
        {
            @params.ColorInt = DrawingModel.GetRGBColor(@params.Coordinate);
            Draw(@params);
        }

        public static unsafe void DrawLineWithZBuffer(DrawingParams @params)
        {
            var dx = @params.P2.X - @params.P1.X;
            var dy = @params.P2.Y - @params.P1.Y;
            var steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

            var XIncrement = dx / (float)steps;
            var YIncrement = dy / (float)steps;

            float x = @params.P1.X;
            float y = @params.P1.Y;
            int index;

            var strideInt = @params.Stride / 4;

            var dz = @params.P2.Z - @params.P1.Z;
            var ZIncrement = dz / (float)steps;

            var z = @params.P1.Z;

            for (var i = 0; i <= steps; i++)
            {
                if (x > @params.WidthZone || x < 0 || y > @params.HeightZone || y < 0)
                {
                    x += XIncrement;
                    y += YIncrement;
                    z += ZIncrement;
                    continue;
                }

                if (ZBuffer.CheckAndSetDistance((int)Math.Round(x), (int)Math.Round(y), z))
                {
                    index = (int)Math.Round(x) + (int)Math.Round(y) * strideInt;

                    @params.RgbBitmap[index] = @params.ColorInt;
                }

                x += XIncrement;
                y += YIncrement;
                z += ZIncrement;
            }
        }
    }
}
