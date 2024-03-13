using System.Numerics;
using static _3DGraphics.Classes.BaseGraphisStructs;

namespace _3DGraphics.Classes
{
    internal static class CoordinateTransformations
    {
        public static void TranslateVectors(GeometricVertex[] vectors, CoordinateVector translation)
        {
            var matrixVect = new Vector3(translation.X, translation.Y, translation.Z);

            Parallel.For(0, vectors.Length, i =>
            {
                vectors[i].Coordinates += matrixVect;
            });
        }

        public static void GetFinalVectors(GeometricVertex[] GeometricVertexCoordinates, Camera camera)
        {
            // Создаем матрицу масштабирования
            var scaleMatrix = Matrix4x4.CreateScale(camera.Scale.Coordinates);

            // Создаем матрицу поворота
            var rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(camera.AngelsRotate.Y, camera.AngelsRotate.X, camera.AngelsRotate.Z);

            // Создаем матрицу переноса
            var translationMatrix = Matrix4x4.CreateTranslation(camera.Translate.Coordinates);

            // Собираем мировую матрицу
            var worldMatrix = scaleMatrix * rotationMatrix * translationMatrix;
            //var worldMatrix = translationMatrix * rotationMatrix * scaleMatrix;

            // Создание матрицы просмотра (View Matrix)
            var viewMatrix = Matrix4x4.CreateLookAt(camera.Eye.Coordinates, camera.Target.Coordinates, camera.Up.Coordinates);

            // Создание матрицы проекции (Projection Matrix)
            var projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(camera.FovRadian, camera.Aspect, camera.ZNear, camera.ZFar);

            // Создание матрицы вида экрана (Viewport Matrix)
            var viewportMatrix = Matrix4x4.CreateViewport(0, 0, camera.Size.Width, camera.Size.Height, camera.ZNear, camera.ZFar);

            // Комбинирование матриц
            var finalMatrix = worldMatrix * viewMatrix * projectionMatrix * viewportMatrix;

            // Преобразование координат вершин
            Parallel.For(0, GeometricVertexCoordinates.Length, i =>
            {
                var vect = new Vector4(GeometricVertexCoordinates[i].Coordinates, 1);
                vect = Vector4.Transform(vect, finalMatrix);
                vect = Vector4.Divide(vect, vect.W);
                GeometricVertexCoordinates[i].X = vect.X;
                GeometricVertexCoordinates[i].Y = vect.Y;
                GeometricVertexCoordinates[i].Z = vect.Z;
            });
        }

    }
}