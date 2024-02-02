namespace _3DGraphics.Classes
{
    internal class BaseGraphisStructs
    {
        public class Vector
        {
            public int length;
            public float this[int index]
            {
                get { return coordinates[index]; }
                set { coordinates[index] = value; }
            }

            protected float[] coordinates;
            public Vector() : this(3) { }

            public Vector(int length)
            {
                coordinates = new float[length];
                this.length = coordinates.Length;
            }
        }

        public class CoordinateVector : Vector
        {
            public float X
            {
                get => coordinates[0];
                set => coordinates[0] = value;
            }
            public float Y
            {
                get => coordinates[1];
                set => coordinates[1] = value;
            }
            public float Z
            {
                get => coordinates[2];
                set => coordinates[2] = value;
            }

            public CoordinateVector():base(3){}
            public CoordinateVector(int length) : base(length) { }

            public CoordinateVector(float x, float y, float z)
            {
                coordinates = [x, y, z];
                length = coordinates.Length;
            }
        }

        public class GeometricVertex : CoordinateVector
        {
            public float W
            {
                get => coordinates[3];
                set => coordinates[3] = value;
            }

            public GeometricVertex() : base(4) { }

            public GeometricVertex(float x, float y, float z)
            {
                coordinates = [x, y, z, 1];
                length = coordinates.Length;
            }

            public GeometricVertex(float x, float y, float z, float w)
            {
                coordinates = [x, y, z, w];
                length = coordinates.Length;
            }
        }

        public class TextureVertices : Vector
        {
            public float U
            {
                get => coordinates[0];
                set => coordinates[0] = value;
            }
            public float V
            {
                get => coordinates[1];
                set => coordinates[1] = value;
            }
            public float W
            {
                get => coordinates[2];
                set => coordinates[2] = value;
            }
            public TextureVertices(float u, float v, float w)
            {
                coordinates = [u, v, w];
                length = coordinates.Length;
            }
        }

        public class NormalVertices : Vector
        {
            public float I
            {
                get => coordinates[0];
                set => coordinates[0] = value;
            }
            public float J
            {
                get => coordinates[1];
                set => coordinates[1] = value;
            }
            public float K
            {
                get => coordinates[2];
                set => coordinates[2] = value;
            }
            public NormalVertices(float i, float j, float k)
            {
                coordinates = [i, j, k];
                length = coordinates.Length;
            }
        }
    }
}
