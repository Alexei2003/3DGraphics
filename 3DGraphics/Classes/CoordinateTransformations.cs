using System.Numerics;
using static _3DGraphics.Classes.BaseGraphisStructs;

namespace _3DGraphics.Classes
{
    internal static class CoordinateTransformations
    {
        public static void TranslateVectors(CoordinateVector vector, CoordinateVector translation)
        {
            var matrixVect = new Vector3(translation.X, translation.Y, translation.Z);

            vector.Coordinates += matrixVect;
        }

        public static void ScaleVectors(GeometricVertex[] vectors, CoordinateVector scale)
        {
            var matrixVect = new Vector3(scale.X, scale.Y, scale.Z);

            Parallel.For(0, vectors.Length, i =>
            {
                vectors[i].Coordinates *= matrixVect;
            });
        }

        public static void RotateVectorsAroundX(CoordinateVector eye, double angle)
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);

            var matrixVectY = new Vector3(0, cos, sin);
            var matrixVectZ = new Vector3(0, -sin, cos);


            var tmpVectY = eye.Coordinates * matrixVectY;
            var tmpVectZ = eye.Coordinates * matrixVectZ;

            eye.Y = tmpVectY.Y + tmpVectY.Z;
            eye.Z = tmpVectZ.Y + tmpVectZ.Z;
        }

        public static void RotateVectorsAroundY(CoordinateVector eye, double angle)
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);

            var matrixVectX = new Vector3(cos, 0, -sin);
            var matrixVectZ = new Vector3(sin, 0, cos);

            var tmpVectX = eye.Coordinates * matrixVectX;
            var tmpVectZ = eye.Coordinates * matrixVectZ;

            eye.X = tmpVectX.X + tmpVectX.Z;
            eye.Z = tmpVectZ.X + tmpVectZ.Z;
        }

        public static void RotateVectorsAroundZ(CoordinateVector eye, double angle)
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);

            var matrixVectX = new Vector3(cos, sin, 0);
            var matrixVectY = new Vector3(-sin, cos, 0);

            var tmpVectX = eye.Coordinates * matrixVectX;
            var tmpVectY = eye.Coordinates * matrixVectY;

            eye.X = tmpVectX.X + tmpVectX.Y;
            eye.Y = tmpVectY.X + tmpVectY.Y;
        }

        public static GeometricVertex[] GetViewVectors(GeometricVertex[] vectors, CoordinateVector eye)
        {
            var target = new CoordinateVector(0, 0, 0);
            var up = new CoordinateVector(0, -1, 0);

            var zAxis = Vector3.Normalize(eye.Coordinates - target.Coordinates);
            var xAxis = Vector3.Normalize(Vector3.Cross(up.Coordinates, zAxis));
            var yAxis = up.Coordinates;

            var matrixVectX = new Vector3(xAxis.X, yAxis.X, zAxis.X);
            var matrixVectY = new Vector3(xAxis.Y, yAxis.Y, zAxis.Y);
            var matrixVectZ = new Vector3(xAxis.Z, yAxis.Z, zAxis.Z);
            //var matrixVectTranslate = new Vector3(-(Vector3.Dot(xAxis, eye.Coordinates)), -(Vector3.Dot(yAxis, eye.Coordinates)), -(Vector3.Dot(zAxis, eye.Coordinates)));

            var result = new GeometricVertex[vectors.Length];

            Parallel.For(0, vectors.Length, i =>
            {
                var tmpVectX = vectors[i].Coordinates * matrixVectX;
                var tmpVectY = vectors[i].Coordinates * matrixVectY;
                var tmpVectZ = vectors[i].Coordinates * matrixVectZ;

                result[i] = new GeometricVertex(tmpVectX[0] + tmpVectX[1] + tmpVectX[2], tmpVectY[0] + tmpVectY[1] + tmpVectY[2], tmpVectZ[0] + tmpVectZ[1] + tmpVectZ[2], vectors[i].W);
            });

            return result;
        }

        public static void GetProjectionVectors(GeometricVertex[] vectors)
        {
            float aspect = 600 / 600;
            float fov = 70.0f * ((float)Math.PI / 180.0f);
            float zFar = 10f;
            float zNear = 0.1f;


            var matr = new Matrix4x4(1 / (aspect * (float)Math.Tan(fov / 2)), 0, 0, 0,
                                 0, 1 / ((float)Math.Tan(fov / 2)), 0, 0,
                                 0, 0, zFar / (zNear - zFar), (zNear * zFar) / (zNear - zFar),
                                 0, 0, -1, 0);

            Parallel.For(0, vectors.Length, i =>
            {
                var vect = new Vector4(vectors[i].X, vectors[i].Y, vectors[i].Z, 0);
                vect = Vector4.Transform(vect, matr);
                vectors[i].X = vect.X;
                vectors[i].Y = vect.Y;
                vectors[i].Z = vect.Z;
            });
        }

        public static void GetViewWindowVectors(GeometricVertex[] vectors)
        {
            int width = 10;
            int height = 10;



            var matr = new Matrix4x4(width >> 2, 0, 0, width >> 2,
                                 0, -(height >> 2), 0, height >> 2,
                                 0, 0, 1, 0,
                                 0, 0, 0, 1);

            Parallel.For(0, vectors.Length, i =>
            {
                var vect = new Vector4(vectors[i].X, vectors[i].Y, vectors[i].Z, 0);
                vect = Vector4.Transform(vect, matr);
                vectors[i].X = vect.X;
                vectors[i].Y = vect.Y;
                vectors[i].Z = vect.Z;
            });

        }
    }
}
