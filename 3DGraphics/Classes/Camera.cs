using System.Numerics;

namespace _3DGraphics.Classes
{
    internal static class Camera
    {
        public static Vector3 Scale = new(1, 1, 1);
        public static Vector3 Translate = new(0, 0, 0);

        public static Vector3 Light = new(0, 1000, 1000);
        public static Vector3 Eye = new(0, 0, 25);
        public static Vector3 Up = new(0, 1, 0);
        public static Vector3 Target = new(0, 0, 0);
        public static float Aspect;

        public static Size Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
                Aspect = (float)size.Width / size.Height;
            }
        }
        private static Size size;

        public static float FovAngle
        {
            get
            {
                return FovRadian / (float)Math.PI * 180.0f;
            }
            set
            {
                if (value > 30 && value < 120)
                {
                    FovRadian = value * (float)Math.PI / 180.0f;
                }
            }
        }

        public static float FovRadian { get; set; } = 30.0f * ((float)Math.PI / 180.0f);

        public static float ZFar { get; set; } = 10f;
        public static float ZNear { get; set; } = 1f;

        public static void Resize(Size size)
        {
            Size = size;
        }

        public static void Resize(int width, int height)
        {
            Size = new(width, height);
        }

        public static void IncFovAngle(float angle)
        {
            FovAngle += angle;
        }

        public static void IncEyeDistance(int change)
        {
            if (change < 0 && Vector3.Distance(new Vector3(0, 0, 0), Vector3.Add(Camera.Eye, new Vector3(0, 0, change))) < 10)
            {
                return;
            }
            var sum = float.Abs(Camera.Eye.X) + float.Abs(Camera.Eye.Y) + float.Abs(Camera.Eye.Z);

            var xPart = Camera.Eye.X / sum;
            var yPart = Camera.Eye.Y / sum;
            var zPart = Camera.Eye.Z / sum;

            Eye.X += change * xPart;
            Eye.Y += change * yPart;
            Eye.Z += change * zPart;
        }
    }
}
