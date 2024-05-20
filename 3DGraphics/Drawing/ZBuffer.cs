namespace _3DGraphics.Drawing
{
    internal static class ZBuffer
    {
        private static float[] zBuffer = new float[1];
        private static int width;
        private static int height;

        public static void Resize(int newWidth, int newHeight)
        {
            width = newWidth;
            height = newHeight;
            zBuffer = new float[width * height];
            Clear();
        }

        public static bool CheckAndSetDepth(int x, int y, float value)
        {
            int index = y * width + x;
            if (zBuffer[index] < value)
            {
                zBuffer[index] = value;
                return true;
            }
            return false;
        }

        public static void Clear()
        {
            Array.Fill(zBuffer, -10000);
        }
    }
}
