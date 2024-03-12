using System;
using System.Numerics;
using Windows.Perception.People;
using static _3DGraphics.Classes.BaseGraphisStructs;

namespace _3DGraphics.Classes
{
    internal static class CoordinateTransformations
    {
        public static void TranslateCoordinate(CoordinateVector vector, CoordinateVector translation)
        {
            var matrixVect = new Vector3(translation.X, translation.Y, translation.Z);

            vector.Coordinates += matrixVect;
        }
        public static void TranslateVectors(GeometricVertex[] vectors, CoordinateVector translation)
        {
            var matrixVect = new Vector3(translation.X, translation.Y, translation.Z);

            Parallel.For(0, vectors.Length, i =>
            {
                vectors[i].Coordinates += matrixVect;
            });
        }

        public static void RotateAroundX(GeometricVertex[] vectors, double angle)
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);

            var matrixVectY = new Vector3(0, cos, sin);
            var matrixVectZ = new Vector3(0, -sin, cos);

            Parallel.For(0, vectors.Length, i =>
            {
                var tmpVectY = vectors[i].Coordinates * matrixVectY;
                var tmpVectZ = vectors[i].Coordinates * matrixVectZ;

                vectors[i].Y = tmpVectY.Y + tmpVectY.Z;
                vectors[i].Z = tmpVectZ.Y + tmpVectZ.Z;
            });
        }

        public static void RotateAroundY(GeometricVertex[] vectors, double angle)
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);

            var matrixVectX = new Vector3(cos, 0, -sin);
            var matrixVectZ = new Vector3(sin, 0, cos);

            Parallel.For(0, vectors.Length, i =>
            {
                var tmpVectX = vectors[i].Coordinates * matrixVectX;
                var tmpVectZ = vectors[i].Coordinates * matrixVectZ;

                vectors[i].X = tmpVectX.X + tmpVectX.Z;
                vectors[i].Z = tmpVectZ.X + tmpVectZ.Z;
            });
        }

        public static void RotateAroundZ(GeometricVertex[] vectors, double angle)
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);

            var matrixVectX = new Vector3(cos, sin, 0);
            var matrixVectY = new Vector3(-sin, cos, 0);

            Parallel.For(0, vectors.Length, i =>
            {
                var tmpVectX = vectors[i].Coordinates * matrixVectX;
                var tmpVectY = vectors[i].Coordinates * matrixVectY;

                vectors[i].X = tmpVectX.X + tmpVectX.Y;
                vectors[i].Y = tmpVectY.X + tmpVectY.Y;
            });
        }

        public static void GetViewVectors(GeometricVertex[] GeometricVertexCoordinates, Camera camera)
        {
            var zAxis = Vector3.Normalize(camera.Eye.Coordinates - camera.Target.Coordinates);
            var xAxis = Vector3.Normalize(Vector3.Cross(camera.Up.Coordinates, zAxis));
            var yAxis = camera.Up.Coordinates;


            var matr = new Matrix4x4(
                     xAxis.X, xAxis.Y, xAxis.Z, -Vector3.Dot(xAxis, camera.Eye.Coordinates),
                     yAxis.X, yAxis.Y, yAxis.Z, -Vector3.Dot(yAxis, camera.Eye.Coordinates),
                     zAxis.X, zAxis.Y, zAxis.Z, -Vector3.Dot(zAxis, camera.Eye.Coordinates),
                     0, 0, 0, 1);

            Parallel.For(0, GeometricVertexCoordinates.Length, i =>
            {
                var vect = new Vector4(GeometricVertexCoordinates[i].X, GeometricVertexCoordinates[i].Y, GeometricVertexCoordinates[i].Z, 0);
                vect = Vector4.Transform(vect, matr);
                GeometricVertexCoordinates[i].X = vect.X;
                GeometricVertexCoordinates[i].Y = vect.Y;
                GeometricVertexCoordinates[i].Z = vect.Z;
            });
        }


        public static void GetProjectionVectors(GeometricVertex[] GeometricVertexCoordinates, Camera camera)
        {
            var matr = new Matrix4x4(
                2f/camera.Width,               0, 0                               , 0,
                0             , 2f/camera.Height, 0                               , 0,
                0             , 0              , 1f / (camera.ZNear - camera.ZFar), camera.ZNear / (camera.ZNear - camera.ZFar),
                0             , 0              , 0                               , 1
            );

            Parallel.For(0, GeometricVertexCoordinates.Length, i =>
            {
                var vect = new Vector4(GeometricVertexCoordinates[i].X, GeometricVertexCoordinates[i].Y, GeometricVertexCoordinates[i].Z, 1);
                vect = Vector4.Transform(vect, matr);
                GeometricVertexCoordinates[i].X = vect.X;
                GeometricVertexCoordinates[i].Y = vect.Y;
                GeometricVertexCoordinates[i].Z = vect.Z;
            });
        }


        public static void GetViewWindowVectors(GeometricVertex[] GeometricVertexCoordinates, Camera camera)
        {
            int width = camera.Width;
            int height = camera.Height;

            var matr = new Matrix4x4(width / 2, 0, 0, width / 2,
                                 0, -(height / 2), 0, height / 2,
                                 0, 0, 1, 0,
                                 0, 0, 0, 1);

            Parallel.For(0, GeometricVertexCoordinates.Length, i =>
            {
                var vect = new Vector4(GeometricVertexCoordinates[i].X, GeometricVertexCoordinates[i].Y, GeometricVertexCoordinates[i].Z, 0);
                vect = Vector4.Transform(vect, matr);
                GeometricVertexCoordinates[i].X = vect.X;
                GeometricVertexCoordinates[i].Y = vect.Y;
                GeometricVertexCoordinates[i].Z = vect.Z;
            });
        }
    }
}
