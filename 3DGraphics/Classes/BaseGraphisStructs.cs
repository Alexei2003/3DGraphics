namespace _3DGraphics.Classes
{
    internal static class BaseGraphisStructs
    {
        public abstract class Vector
        {
            public int length;
            public float this[int index]
            {
                get { return Coordinates[index]; }
                set { Coordinates[index] = value; }
            }

            public float[] Coordinates { get; set; }


            public Vector()
            {
                Coordinates = [0, 0, 0];
                this.length = Coordinates.Length;
            }

            public Vector(float a, float b, float c)
            {
                Coordinates = [a, b, c];
                this.length = Coordinates.Length;
            }

            public Vector(Vector vector)
            {
                Coordinates = vector.Coordinates;
                this.length = vector.length;
            }
        }

        public class CoordinateVector : Vector
        {
            public float X
            {
                get => Coordinates[0];
                set => Coordinates[0] = value;
            }
            public float Y
            {
                get => Coordinates[1];
                set => Coordinates[1] = value;
            }
            public float Z
            {
                get => Coordinates[2];
                set => Coordinates[2] = value;
            }


            public CoordinateVector() : base() { }

            public CoordinateVector(float x, float y, float z) : base(x, y, z) { }
            public CoordinateVector(CoordinateVector vector) : base(vector) { }
        }

        public sealed class GeometricVertex : CoordinateVector
        {
            public float W
            {
                get => Coordinates[3];
                set => Coordinates[3] = value;
            }
            public float TranslateX
            {
                get => Coordinates[4];
                set => Coordinates[4] = value;
            }
            public float TranslateY
            {
                get => Coordinates[5];
                set => Coordinates[5] = value;
            }
            public float TranslateZ
            {
                get => Coordinates[6];
                set => Coordinates[6] = value;
            }


            public GeometricVertex()
            {
                Coordinates = [0, 0, 0, 0, 0, 0, 0];
                this.length = Coordinates.Length;
            }

            public GeometricVertex(float x, float y, float z, float w)
            {
                Coordinates = [x, y, z, w, 0, 0, 0];
                length = Coordinates.Length;
            }

            public GeometricVertex(GeometricVertex vector) : base(vector) { }
        }

        public sealed class TextureVertice : Vector
        {
            public float U
            {
                get => Coordinates[0];
                set => Coordinates[0] = value;
            }
            public float V
            {
                get => Coordinates[1];
                set => Coordinates[1] = value;
            }
            public float W
            {
                get => Coordinates[2];
                set => Coordinates[2] = value;
            }


            public TextureVertice() : base() { }
            public TextureVertice(float u, float v, float w) : base(u, v, w) { }
            public TextureVertice(TextureVertice vector) : base(vector) { }
        }

        public sealed class NormalVertice : Vector
        {
            public float I
            {
                get => Coordinates[0];
                set => Coordinates[0] = value;
            }
            public float J
            {
                get => Coordinates[1];
                set => Coordinates[1] = value;
            }
            public float K
            {
                get => Coordinates[2];
                set => Coordinates[2] = value;
            }


            public NormalVertice() : base() { }
            public NormalVertice(float i, float j, float k) : base(i, j, k) { }
            public NormalVertice(NormalVertice vector) : base(vector) { }
        }
    }
}
