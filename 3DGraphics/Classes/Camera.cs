using System.Numerics;

namespace _3DGraphics.Classes
{
    internal class Camera
    {
        public Vector3 Scale = new(1, 1, 1);
        public Vector3 AngelsRotate = new(0, 0, 0);
        public Vector3 Translate = new(0, 0, 0);

        public Vector3 Eye = new(0, 0, 100);
        public Vector3 Up = new(0, 1, 0);
        public Vector3 Target = new(0, 0, 0);
        public float Aspect;

        public Size Size
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
        private Size size;

        public float FovAngle
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

        public float FovRadian { get; set; } = 30.0f * ((float)Math.PI / 180.0f);

        public float ZFar { get; set; } = 10f;
        public float ZNear { get; set; } = 1f;

        public Camera(Size size)
        {
            Size = size;
        }

        public Camera(int width, int height)
        {
            Size = new(width, height);
        }

        public void IncFovAngle(float angle)
        {
            FovAngle += angle;
        }

        public void IncEyeZ(int Z)
        {
            if (Eye.Z + Z > 20)
            {
                Eye.Z += Z;
            }
        }
    }
}
