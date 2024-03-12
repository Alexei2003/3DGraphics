namespace _3DGraphics.Classes
{
    internal class Camera
    {
        public double angleX { get; set; } = 0;

        public readonly double angle90 = Math.PI / 180 * 80;

        public BaseGraphisStructs.CoordinateVector Eye { get; set; } = new(0, 0, 1);
        public BaseGraphisStructs.CoordinateVector Up { get; set; } = new(0, 1, 0);
        public BaseGraphisStructs.CoordinateVector Target { get; set; } = new(0, 0, 0);
        public float Aaspect { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float FovAngle
        {
            get
            {
                return FovRadian / (float)Math.PI * 180.0f;    
            }

            set 
            {
                if(value > 0 && value < 180) 
                {
                    FovRadian = value * (float)Math.PI / 180.0f;
                }
            }
        }

        public float FovRadian { get; set; } = 70.0f * ((float)Math.PI / 180.0f);

        public float ZFar { get; set; } = 10f;
        public float ZNear { get; set; } = 0.1f;

        public Camera(int width, int height)
        {
            Aaspect = width / height;
            Width = width;
            Height = height;
        }

        public void IncFovAngle(float angle)
        {
            FovAngle += angle;
        }
    }
}
