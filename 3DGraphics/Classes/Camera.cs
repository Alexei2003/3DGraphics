using System.Numerics;

namespace _3DGraphics.Classes
{
    internal static class Camera
    {
        public static Vector3 Scale = new(1, 1, 1);
        public static Vector3 AngelsRotate = new(0, 0, 0);
        public static Vector3 Translate = new(0, 0, 0);

        public static Vector3 Eye = new(0, 0, 100);
        public static Vector3 Light = new(0, 0, 100);
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
                Light.X = size.Width / 2;
                Light.Y = size.Height / 2;
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

        public static void IncEyeZ(int Z)
        {
            if (Eye.Z + Z > 20)
            {
                Eye.Z += Z;
                Light.Z = Eye.Z /10;
            }
        }
    }
}
