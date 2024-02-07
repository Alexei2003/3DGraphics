using System.Numerics;
using static _3DGraphics.Classes.BaseGraphisStructs;

namespace _3DGraphics.Classes
{
    internal static class CoordinateTransformations
    {
        /*        private const int MAX_SIZE_MATRIX = 4;

                private static float[,] MultiplyMatrices(float[,] matrix1, float[,] matrix2)
                {
                    var result = new float[MAX_SIZE_MATRIX, MAX_SIZE_MATRIX];

                    for (var i = 0; i < MAX_SIZE_MATRIX; i++)
                    {
                        for (var j = 0; j < MAX_SIZE_MATRIX; j++)
                        {
                            result[i, j] = matrix1[i, 0] * matrix2[0, j] + matrix1[i, 1] * matrix2[1, j] + matrix1[i, 2] * matrix2[2, j] + matrix1[i, 3] * matrix2[3, j];
                        }
                    }

                    return result;
                }

                private static GeometricVertex MultiplyVectorAndMatrixAsMatrices(GeometricVertex vector, float[,] matrix1)
                {
                    var result = new GeometricVertex();

                    var matrix2 = new float[,]
                    {
                        {    vector.X,    vector.Y,    vector.Z,           0},
                        {    vector.X,    vector.Y,    vector.Z,           0},
                        {    vector.X,    vector.Y,    vector.Z,           0},
                        {           0,           0,           0,           1},
                    };

                    var matrixResult = MultiplyMatrices(matrix2, matrix1);

                    result.X = matrixResult[0, 0];
                    result.Y = matrixResult[1, 1];
                    result.Z = matrixResult[2, 2];
                    result.W = vector.W;

                    return result;
                }*/

        public static GeometricVertex[] TranslateVectors(GeometricVertex[] vectors, CoordinateVector translation)
        {
/*            var result = new GeometricVertex[vectors.Length];

            Parallel.For(0, vectors.Length, i =>
            {
                result[i] = new GeometricVertex
                {
                    X = vectors[i].X,
                    Y = vectors[i].Y,
                    Z = vectors[i].Z
                };
            });*/

            return vectors;
        }

        public static GeometricVertex[] ScaleVectors(GeometricVertex[] vectors, CoordinateVector scale)
        {
            Parallel.For(0, vectors.Length, i =>
            {
                vectors[i].X *= scale.X;
                vectors[i].Y *= scale.Y;
                vectors[i].Z *= scale.Z;
            });

            return vectors;
        }

        public static GeometricVertex[] RotateVectorsAroundX(GeometricVertex[] vectors, double angle)
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);

            Parallel.For(0, vectors.Length, i =>
            {
                var y = vectors[i].Y;
                var z = vectors[i].Z;

                vectors[i].Y = y * cos + vectors[i].Z * sin;
                vectors[i].Z = y * -sin + z * cos;
            });

            return vectors;

        }

        public static GeometricVertex[] RotateVectorsAroundY(GeometricVertex[] vectors, double angle)
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);

            Parallel.For(0, vectors.Length, i =>
            {
                var x = vectors[i].X;
                var z = vectors[i].Z;

                vectors[i].X = x * cos + vectors[i].Z * -sin;
                vectors[i].Z = x * sin + z * cos;
            });

            return vectors;
        }

        public static GeometricVertex[] RotateVectorsAroundZ(GeometricVertex[] vectors, double angle)
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);

            Parallel.For(0, vectors.Length, i =>
            {
                var x = vectors[i].X;
                var y = vectors[i].Y;

                vectors[i].X = x * cos + vectors[i].Y * sin;
                vectors[i].Y = x * -sin + y * cos;
            });

            return vectors;
        }
    }
}
