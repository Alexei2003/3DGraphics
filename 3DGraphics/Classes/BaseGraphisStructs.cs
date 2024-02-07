namespace _3DGraphics.Classes
{
    internal static class BaseGraphisStructs
    {
        public abstract class Vector
        {
            public int length;
            public float this[int index]
            {
                get { return coordinates[index]; }
                set { coordinates[index] = value; }
            }

            protected float[] coordinates;


            public Vector()
            {
                coordinates = [0, 0, 0];
                this.length = coordinates.Length;
            }

            public Vector(float a, float b, float c)
            {
                coordinates = [a, b, c];
                this.length = coordinates.Length;
            }

            public Vector(Vector vector)
            {
                coordinates = vector.coordinates;
                this.length = vector.length;
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


            public CoordinateVector() : base() { }

            public CoordinateVector(float x, float y, float z) : base(x, y, z) { }
            public CoordinateVector(CoordinateVector vector) : base(vector) { }
        }

        public sealed class GeometricVertex : CoordinateVector
        {
            public float W
            {
                get => coordinates[3];
                set => coordinates[3] = value;
            }


            public GeometricVertex()
            {
                coordinates = [0, 0, 0, 0];
                this.length = coordinates.Length;
            }

            public GeometricVertex(float x, float y, float z, float w)
            {
                coordinates = [x, y, z, w];
                length = coordinates.Length;
            }

            public GeometricVertex(GeometricVertex vector) : base(vector) { }
        }

        public sealed class TextureVertice : Vector
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


            public TextureVertice() : base() { }
            public TextureVertice(float u, float v, float w) : base(u, v, w) { }
            public TextureVertice(TextureVertice vector) : base(vector) { }
        }

        public sealed class NormalVertice : Vector
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


            public NormalVertice() : base() { }
            public NormalVertice(float i, float j, float k) : base(i, j, k) { }
            public NormalVertice(NormalVertice vector) : base(vector) { }
        }
    }
}
