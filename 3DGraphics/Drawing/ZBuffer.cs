using _3DGraphics.Classes;
using System.Numerics;

namespace _3DGraphics.Drawing
{
    internal static class ZBuffer
    {
        private static float[,] zBuffer = new float[1, 1];
        private static float[,] originaZBuffer = new float[1, 1];
        private static int width;
        private static int height;

        public static void Resaze(int width, int height)
        {
            originaZBuffer = new float[width, height];

            for (int i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    originaZBuffer[i, j] = -10000;
                }
            }
            ZBuffer.width = width;
            ZBuffer.height = height;
        }

        public static bool CheckAndSetDepth(int x, int y, float value)
        {
            if (CanAddDepth(x, y, value))
            {
                SetDepth(x, y, value);
                return true;
            }
            return false;
        }

        public static void SetDepth(int x, int y, float value)
        {
            zBuffer[x, y] = value;
        }

        public static bool CanAddDepth(int x, int y, float value)
        {
            if (zBuffer[x, y] < value)
            {
                return true;
            }
            return false;
        }

        public static void Clear()
        {
            zBuffer = (float[,])originaZBuffer.Clone();
        }
    }
}
