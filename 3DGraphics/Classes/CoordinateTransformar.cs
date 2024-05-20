using System.Numerics;
using static _3DGraphics.Classes.BaseGraphisStructs;

namespace _3DGraphics.Classes
{
    internal static class CoordinateTransformar
    {
        private static Matrix4x4? invertMatrix = null;

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

            var finalMatrix1 = worldMatrix;
            var finalMatrix2 = viewMatrix * projectionMatrix * viewportMatrix;

            // Преобразование координат вершин
            Parallel.For(0, modelData.GeometricVertexCoordinates.Length, i =>
            {
                var vect = new Vector4(modelData.GeometricVertexCoordinates[i].Coordinates, 1);
                vect = Vector4.Transform(vect, finalMatrix1);
                modelData.GeometricVertexCoordinates[i] = new BaseGraphisStructs.CoordinateVector(vect.X, vect.Y, vect.Z);

                modelData.GeometricVertexWorldCoordinates[i] = modelData.GeometricVertexCoordinates[i];

                vect = new Vector4(modelData.GeometricVertexCoordinates[i].Coordinates, 1);
                vect = Vector4.Transform(vect, finalMatrix2);
                vect = Vector4.Divide(vect, vect.W);
                modelData.GeometricVertexCoordinates[i] = new BaseGraphisStructs.CoordinateVector(vect.X, vect.Y, vect.Z);
            });

            Matrix4x4.Invert(viewportMatrix, out Matrix4x4 invViewportMatrix);

            Matrix4x4.Invert(projectionMatrix, out Matrix4x4 invProjectionMatrix);

            Matrix4x4.Invert(viewMatrix, out Matrix4x4 invViewMatrix);

            // Получаем обратную матрицу для исходной составной матрицы
            invertMatrix = invViewportMatrix * invProjectionMatrix * invViewMatrix;
        }

        public static BaseGraphisStructs.CoordinateVector RevercePoint(BaseGraphisStructs.CoordinateVector vector)
        {
            if (invertMatrix != null)
            {
                var vect = new Vector4(vector.Coordinates, 1);
                vect = Vector4.Transform(vect, invertMatrix.Value);
                vect = Vector4.Divide(vect, vect.W);
                return new BaseGraphisStructs.CoordinateVector(vect.X, vect.Y, vect.Z);
            }
            return vector;
        }
    }
}