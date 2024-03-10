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

        public static GeometricVertex[] GetViewVectors(GeometricVertex[] vectors, Camera camera)
        {
            var zAxis = Vector3.Normalize(camera.Eye.Coordinates - camera.Target.Coordinates);
            var xAxis = Vector3.Normalize(Vector3.Cross(camera.Up.Coordinates, zAxis));
            var yAxis = camera.Up.Coordinates;

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

        public static void GetProjectionVectors(GeometricVertex[] vectors, Camera camera)
        {
            var matr = new Matrix4x4(1 / (camera.Aaspect * (float)Math.Tan(camera.Fov / 2)), 0, 0, 0,
                                 0, 1 / ((float)Math.Tan(camera.Fov / 2)), 0, 0,
                                 0, 0, camera.ZFar / (camera.ZNear - camera.ZFar), (camera.ZNear * camera.ZFar) / (camera.ZNear - camera.ZFar),
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
