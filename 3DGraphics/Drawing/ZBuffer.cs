namespace _3DGraphics.Drawing
{
    internal static class ZBuffer
    {
        private static float[][] zBuffer = new float[1][];
        private static float[][] originaZBuffer = new float[1][];
        private static int width;
        private static int height;

        public static void Resaze(int width, int height)
        {
            originaZBuffer = new float[width][];

            for (int i = 0; i < width; i++)
            {
                originaZBuffer[i] = new float[height];
                for (var j = 0; j < height; j++)
                {
                    originaZBuffer[i][j] = -10000000;
                }
            }
            ZBuffer.width = width;
            ZBuffer.height = height;
        }

        public static bool CheckAndSetDistance(int x, int y, float value)
        {
            if (CanAddDistance(x, y, value))
            {
                SetDistance(x, y, value);
                return true;
            }
            return false;
        }

        public static void SetDistance(int x, int y, float value)
        {
            zBuffer[x][y] = value;
        }

        public static bool CanAddDistance(int x, int y, float value)
        {
            if (zBuffer[x][y] < value)
            {
                return true;
            }
            return false;
        }

        public static void Clear()
        {
            zBuffer = new float[width][];

            for (var i = 0; i < width; i++)
            {
                zBuffer[i] = new float[height];
                originaZBuffer[i].CopyTo(zBuffer[i],0);
            }         
        }
    }
}
