using static _3DGraphics.Classes.BaseGraphisStructs;

namespace _3DGraphics.Classes
{
    internal static class CoordinateTransformations
    {
        private const int MAX_SIZE_MATRIX = 4;

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
                            {    vector.X,    vector.X,    vector.X,           0},
                            {    vector.Y,    vector.Y,    vector.Y,           0},
                            {    vector.Z,    vector.Z,    vector.Z,           0},
                            {           0,           0,           0,           1},
            };

            var matrixResult = MultiplyMatrices(matrix1, matrix2);

            result.X = matrixResult[0, 0];
            result.Y = matrixResult[1, 1];
            result.Z = matrixResult[2, 2];
            result.W = vector.W;

            return result;
        }

        public static GeometricVertex TranslateVector(GeometricVertex vector, CoordinateVector translation)
        {


            return vector;
        }

        public static GeometricVertex ScaleVector(GeometricVertex vector, CoordinateVector scale)
        {
            var matrix = new float[,]
            {
                {     scale.X,           0,           0,           0},
                {           0,     scale.Y,           0,           0},
                {           0,           0,     scale.Z,           0},
                {           0,           0,           0,           1},
            };

            return MultiplyVectorAndMatrixAsMatrices(vector, matrix);
        }

        public static GeometricVertex RotateVectorAroundX(GeometricVertex vector, double angle)
        {
            var matrix = new float[,]
            {
                {                         1,                         0,                         0,                         0},
                {                         0,  ((float)Math.Cos(angle)), -((float)Math.Sin(angle)),                         0},
                {                         0,  ((float)Math.Sin(angle)),  ((float)Math.Cos(angle)),                         0},
                {                         0,                         0,                         0,                         1},
            };

            return MultiplyVectorAndMatrixAsMatrices(vector, matrix);
        }

        public static GeometricVertex RotateVectorAroundY(GeometricVertex vector, double angle)
        {
            var matrix = new float[,]
            {
                {  ((float)Math.Cos(angle)),                         0,  ((float)Math.Sin(angle)),                         0},
                {                         0,                         1,                         0,                         0},
                { -((float)Math.Sin(angle)),                         0,  ((float)Math.Cos(angle)),                         0},
                {                         0,                         0,                         0,                         1},
            };

            return MultiplyVectorAndMatrixAsMatrices(vector, matrix);
        }

        public static GeometricVertex RotateVectorAroundZ(GeometricVertex vector, double angle)
        {
            var matrix = new float[,]
            {
                {  ((float)Math.Cos(angle)), -((float)Math.Sin(angle)),                         0,                         0},
                {  ((float)Math.Sin(angle)),  ((float)Math.Cos(angle)),                         0,                         0},
                {                         0,                         0,                         1,                         0},
                {                         0,                         0,                         0,                         1},
            };

            return MultiplyVectorAndMatrixAsMatrices(vector, matrix);
        }
    }
}
