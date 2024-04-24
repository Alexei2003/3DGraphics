namespace _3DGraphics.Drawing
{
    internal static class ZBuffer
    {
        private static float[,] zBuffer = new float[1, 1];
        private static int width;
        private static int height;

        public static void Resaze(int width, int height)
        {
            zBuffer = new float[width, height];
            ZBuffer.width = width;
            ZBuffer.height = height;
        }

        public static bool CheckAndSetDistance(int x, int y, float value)
        {
            if (zBuffer[x, y] > value)
            {
                zBuffer[x, y] = value;
                return true;
            }
            return false;
        }

        public static void SetDistance(int x, int y, float value)
        {
            zBuffer[x, y] = value;
        }

        public static bool CanAddDistance(int x, int y, float value)
        {
            if (zBuffer[x, y] > value)
            {
                return true;
            }
            return false;
        }

        public static void Clear()
        {

            for (int i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    zBuffer[i, j] = 10000;
                }
            }
        }
    }
}
