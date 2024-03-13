namespace _3DGraphics.Classes
{
    internal class Camera
    {
        public BaseGraphisStructs.CoordinateVector Scale = new(1, 1, 1);
        public BaseGraphisStructs.CoordinateVector AngelsRotate = new(0, 0, 0);
        public BaseGraphisStructs.CoordinateVector Translate = new(0, 0, 0);

        public BaseGraphisStructs.CoordinateVector Eye { get; set; } = new (0, 0, 100);
        public BaseGraphisStructs.CoordinateVector Up { get; set; } = new(0, 1, 0);
        public BaseGraphisStructs.CoordinateVector Target { get; set; } = new(0, 0, 0);
        public float Aspect { get; set; }

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
