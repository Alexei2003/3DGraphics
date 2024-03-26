using System.Numerics;
using static _3DGraphics.Classes.BaseGraphisStructs;

namespace _3DGraphics.Classes
{
    internal static class CoordinateTransformations
    {
        public static void TranslateVectors(GeometricVertex[] vectors, CoordinateVector translation)
        {
            Parallel.For(0, vectors.Length, i =>
            {
                vectors[i].Coordinates += translation.Coordinates;
            });
        }

        public static void GetFinalVectors(GeometricVertex[] GeometricVertexCoordinates, Camera camera)
        {
            // Создаем матрицу масштабирования
            var scaleMatrix = Matrix4x4.CreateScale(camera.Scale);

            // Создаем матрицу поворота
            var rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(camera.AngelsRotate.Y, camera.AngelsRotate.X, camera.AngelsRotate.Z);

            // Создаем матрицу переноса
            var translationMatrix = Matrix4x4.CreateTranslation(camera.Translate);

            // Собираем мировую матрицу
            var worldMatrix = scaleMatrix * rotationMatrix * translationMatrix;

            // Создание матрицы просмотра (View Matrix)
            var viewMatrix = Matrix4x4.CreateLookAt(camera.Eye, camera.Target, camera.Up);

            // Создание матрицы проекции (Projection Matrix)
            var projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(camera.FovRadian, camera.Aspect, camera.ZNear, camera.ZFar);

            // Создание матрицы вида экрана (Viewport Matrix)
            var viewportMatrix = Matrix4x4.CreateViewport(0, 0, camera.Size.Width, camera.Size.Height, camera.ZNear, camera.ZFar);

            // Комбинирование матриц
            var finalMatrix = worldMatrix * viewMatrix * projectionMatrix * viewportMatrix;

            // Преобразование координат вершин
            Parallel.For(0, GeometricVertexCoordinates.Length, i =>
            {
                GeometricVertexCoordinates[i].Coordinates = Vector4.Transform(GeometricVertexCoordinates[i].Coordinates, finalMatrix);
                GeometricVertexCoordinates[i].Coordinates = Vector4.Divide(GeometricVertexCoordinates[i].Coordinates, GeometricVertexCoordinates[i].W);
            });
        }
    }
}