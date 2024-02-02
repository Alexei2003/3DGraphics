namespace _3DGraphics.Classes
{
    internal class BaseGraphisStructs
    {
        public struct Vector
        {
            public readonly float X
            {
                get => coordinates[0];
                set => coordinates[0] = value;
            }
            public readonly float Y
            {
                get => coordinates[1];
                set => coordinates[1] = value;
            }
            public readonly float Z
            {
                get => coordinates[2];
                set => coordinates[2] = value;
            }
            public readonly float this[int index]
            {
                get { return coordinates[index]; }
                set { coordinates[index] = value; }
            }

            private float[] coordinates;

            public Vector()
            {
                coordinates = [0, 0, 0, 0];
            }

            public Vector(float x, float y, float z)
            {
                coordinates = [x, y, z, 1];
            }

            public Vector(float x, float y, float z, float w)
            {
                coordinates = [x, y, z, w];
            }
        }
    }
}
