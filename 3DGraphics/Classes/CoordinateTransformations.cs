using System.Numerics;
using static _3DGraphics.Classes.BaseGraphisStructs;
using static _3DGraphics.Classes.ObjFileReader;

namespace _3DGraphics.Classes
{
    internal static class CoordinateTransformations
    {
        public static void TranslateVectors(CoordinateVector[] vectors, CoordinateVector translation)
        {
            Parallel.For(0, vectors.Length, i =>
            {
                vectors[i].Coordinates += translation.Coordinates;
            });
        }

        public static void RotateCamera(Vector3 angle)
        {
            // Создаем матрицу поворота
            var rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(angle.Y, angle.X, angle.Z);

            var vect = new Vector4(Camera.Eye, 1);
            vect = Vector4.Transform(vect, rotationMatrix);

            Camera.Eye = new Vector3(vect.X, vect.Y, vect.Z);
        }

        public static void GetFinalVectors(ObjFileReader.ModelData modelData)
        {
            // Создаем матрицу масштабирования
            var scaleMatrix = Matrix4x4.CreateScale(Camera.Scale);

            // Создаем матрицу переноса
            var translationMatrix = Matrix4x4.CreateTranslation(Camera.Translate);

            // Собираем мировую матрицу
            var worldMatrix = scaleMatrix * translationMatrix;

            // Создание матрицы просмотра (View Matrix)
            var viewMatrix = Matrix4x4.CreateLookAt(Camera.Eye, Camera.Target, Camera.Up);

            // Создание матрицы проекции (Projection Matrix)
            var projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(Camera.FovRadian, Camera.Aspect, Camera.ZNear, Camera.ZFar);

            // Создание матрицы вида экрана (Viewport Matrix)
            var viewportMatrix = Matrix4x4.CreateViewport(0, 0, Camera.Size.Width, Camera.Size.Height, Camera.ZNear, Camera.ZFar);

            // Комбинирование матриц
            var finalMatrix = worldMatrix; // * viewMatrix//* projectionMatrix * viewportMatrix;

            // Преобразование координат вершин
            Parallel.For(0, modelData.GeometricVertexCoordinates.Length, i =>
            {
                var vect = new Vector4(modelData.GeometricVertexCoordinates[i].Coordinates, 1);
                vect = Vector4.Transform(vect, finalMatrix);
                //vect = Vector4.Divide(vect, vect.W);
                modelData.GeometricVertexCoordinates[i] = new BaseGraphisStructs.CoordinateVector(vect.X, vect.Y, vect.Z);
            });

            modelData.GeometricVertexToNormalCoordinates = new CoordinateVector[modelData.GeometricVertexCoordinates.Length];

            for (var i = 0; i < modelData.GeometricVertexCoordinates.Length; i++)
            {
                modelData.GeometricVertexToNormalCoordinates[i] = new BaseGraphisStructs.CoordinateVector(modelData.GeometricVertexCoordinates[i].Coordinates);
            }

            finalMatrix = viewMatrix * projectionMatrix * viewportMatrix;

            // Преобразование координат вершин
            Parallel.For(0, modelData.GeometricVertexCoordinates.Length, i =>
            {
                var vect = new Vector4(modelData.GeometricVertexCoordinates[i].Coordinates, 1);
                vect = Vector4.Transform(vect, finalMatrix);
                vect = Vector4.Divide(vect, vect.W);
                modelData.GeometricVertexCoordinates[i] = new BaseGraphisStructs.CoordinateVector(vect.X, vect.Y, vect.Z);
            });
        }
    }
}