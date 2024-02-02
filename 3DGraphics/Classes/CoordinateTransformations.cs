using static _3DGraphics.Classes.BaseGraphisStructs;

namespace _3DGraphics.Classes
{
    internal class CoordinateTransformations
    {
        /// Добавить проверку на размеры
        private static Vector MultiplyingVectorAndMatrix(Vector vector, float[,] matrix)
        {
            var result = new Vector();

            // Строки
            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                float sum = 0;
                // Столбцы
                for (var j = 0; j < matrix.GetLength(1); j++)
                {
                    sum += vector[j] * matrix[i, j];
                }
            }

            return result;
        }

        public static Vector TranslateVector(Vector vector, Vector translation)
        {
            var matrix = new float[,]
            {
                { 1, 0, 0, translation.X},
                { 0, 1, 0, translation.Y},
                { 0, 0, 1, translation.Z},
                { 0, 0, 0, 1            },
            };

            return MultiplyingVectorAndMatrix(vector, matrix);
        }
        public static Vector ScaleVector(Vector vector, Vector scale)
        {
            var matrix = new float[,]
            {
                { scale.X, 0      , 0, 0      },
                { 0      , scale.Y, 0, 0      },
                { 0      , 0      , 0, scale.Z},
                { 0      , 0      , 0, 1      },
            };

            return MultiplyingVectorAndMatrix(vector, matrix);
        }
        public static Vector RotateVectorAroundX(Vector vector, double angle)
        {
            var matrix = new float[,]
            {
                { 0, 0                       , 0                        , 0},
                { 0, ((float)Math.Cos(angle)), -((float)Math.Sin(angle)), 0},
                { 0, ((float)Math.Sin(angle)), ((float)Math.Cos(angle)) , 0},
                { 0, 0                       , 0                        , 1},
            };

            return MultiplyingVectorAndMatrix(vector, matrix);
        }
        public static Vector RotateVectorAroundY(Vector vector, double angle)
        {
            var matrix = new float[,]
            {
                { ((float)Math.Cos(angle)) , 0, ((float)Math.Sin(angle)), 0},
                { 0                        , 0, 0                       , 0},
                { -((float)Math.Sin(angle)), 0, ((float)Math.Cos(angle)), 0},
                { 0                        , 0, 0                       , 1},
            };

            return MultiplyingVectorAndMatrix(vector, matrix);
        }
        public static Vector RotateVectorAroundZ(Vector vector, double angle)
        {
            var matrix = new float[,]
            {
                { ((float)Math.Cos(angle)), -((float)Math.Sin(angle)), 0, 0},
                { ((float)Math.Sin(angle)), ((float)Math.Cos(angle)) , 0, 0},
                { 0                       , 0                        , 0, 0},
                { 0                       , 0                        , 0, 1},
            };

            return MultiplyingVectorAndMatrix(vector, matrix);
        }
    }
}
