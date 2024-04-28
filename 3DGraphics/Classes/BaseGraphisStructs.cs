using System.Numerics;

namespace _3DGraphics.Classes
{
    internal static class BaseGraphisStructs
    {
        public abstract class Vector
        {
            public Vector4 Coordinates
            {
                get => coordinates;
                set => coordinates = value;
            }

            protected Vector4 coordinates;

            public Vector()
            {
                coordinates = new Vector4();
            }

            public Vector(float a, float b, float c, float d = 0)
            {
                coordinates = new Vector4(a, b, c, d);
            }

            public Vector(Vector4 vector)
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

            public CoordinateVector(float x, float y, float z, float d = 0) : base(x, y, z, d) { }

            public CoordinateVector(Vector4 vector) : base(vector) { }
        }

        public sealed class GeometricVertex : CoordinateVector
        {
            public float W
            {
                get => coordinates[3];
                set => coordinates[3] = value;
            }

            public GeometricVertex() : base() { }

            public GeometricVertex(float x, float y, float z, float w) : base(x, y, z, w) { }

            public GeometricVertex(Vector4 vector) : base(vector) { }
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

            public TextureVertice(Vector4 vector) : base(vector) { }
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

            public NormalVertice(Vector4 vector) : base(vector) { }
        }

        public struct Point3DF
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }

            public Point3DF(float x, float y, float z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public Point3DF(PointF p, float z)
            {
                X = p.X;
                Y = p.Y;
                Z = z;
            }
        }
    }
}
