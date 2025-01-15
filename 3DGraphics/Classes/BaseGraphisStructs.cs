using System.Numerics;

namespace _3DGraphics.Classes
{
    internal static class BaseGraphisStructs
    {
        public abstract class Vector
        {
            public Vector3 Coordinates
            {
                get => coordinates;
                set => coordinates = value;
            }

            protected Vector3 coordinates;

            public Vector()
            {
                coordinates = new Vector3();
            }

            public Vector(float a, float b, float c)
            {
                coordinates = new Vector3(a, b, c);
            }

            public Vector(Vector3 vector)
            {
                coordinates = vector;
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

            public CoordinateVector(Vector3 vector) : base(vector) { }
        }

        public sealed class TextureVector : Vector
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

            public TextureVector() : base() { }

            public TextureVector(float u, float v, float w) : base(u, v, w) { }

            public TextureVector(Vector3 vector) : base(vector) { }
        }

        public sealed class NormalVector : Vector
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

            public NormalVector() : base() { }

            public NormalVector(float i, float j, float k) : base(i, j, k) { }

            public NormalVector(Vector3 vector) : base(vector) { }
        }
    }
}
