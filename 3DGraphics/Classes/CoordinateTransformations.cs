using static _3DGraphics.Classes.BaseGraphisStructs;

namespace _3DGraphics.Classes
{
    internal class CoordinateTransformations
    {
        /// Добавить проверку на размеры
        private static Vector MultiplyingVectorAndMatrix(Vector vector, float[,] matrix)
        {
            var result = new Vector(4);

            // Строки
            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                float sum = 0;
                // Столбцы
                for (var j = 0; j < matrix.GetLength(1); j++)
                {
                    sum += vector[j] * matrix[i, j];
                }
                result[i] = sum;
            }

            return result;
        }

        private static Vector SubtractVectors(Vector vector1, Vector vector2)
        {
            var result = new Vector(3);

            for (var i = 0; i < vector1.length; i++)
            {
                result[i] = vector1[i] - vector2[i];
            }

            return result;
        }

        private static Vector MultiplyVectors(Vector vector1, Vector vector2)
        {
            var result = new Vector(3);

            result[0] = vector1[1] * vector2[2] - vector1[2] * vector2[1];
            result[1] = vector1[2] * vector2[0] - vector1[0] * vector2[2];
            result[2] = vector1[0] * vector2[1] - vector1[1] * vector2[0];

            return result;
        }

        private static Vector NormalizeVector(Vector vector)
        {
            var result = new Vector(vector.length);

            float length = ((float)Math.Sqrt(vector[0] * vector[0] + vector[1] * vector[1] + vector[2] * vector[2]));

            for (var i = 0; i < vector.length; i++)
            {
                result[0] = vector[i] / length;
            }

            return result;
        }

        private static float ScalarProductVector(Vector vector1, Vector vector2)
        {
            float result = 0;


            for (var i = 0; i < vector1.length; i++)
            {
                result += vector1[i] * vector2[i];
            }


            return result;
        }

        public static Vector TranslateVector(Vector vector, CoordinateVector translation)
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
        public static Vector ScaleVector(Vector vector, CoordinateVector scale)
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

        private static Vector TransformСoordinatesModelToWorld(Vector vector, CoordinateVector eye, CoordinateVector target, CoordinateVector up)
        {
            var tmpZ = NormalizeVector(SubtractVectors(eye, target));
            var tmpX = NormalizeVector(MultiplyVectors(up, tmpZ));
            var z = new CoordinateVector(tmpZ[0], tmpZ[1], tmpZ[2]);
            var x = new CoordinateVector(tmpX[0], tmpX[1], tmpX[2]);
            var y = up;

            var matrix = new float[,]
            {
                { x.X, x.Y , x.Z, -ScalarProductVector(x,eye)},
                { y.X, y.Y , x.Z, -ScalarProductVector(y,eye)},
                { z.X, z.Y , z.Z, -ScalarProductVector(z,eye)},
                { 0  , 0   , 0  , 1},
            };

            return MultiplyingVectorAndMatrix(vector, matrix);
        }

        private static Vector TransformСoordinatesWorldToProjection(Vector vector, float width, float height, float near, float far)
        {
            var matrix = new float[,]
            {
                { 2/width, 0       , 0           , 0              },
                { 0      , 2/height, 0           , 0              },
                { 0      , 0       , 1/(near-far), near/(near-far)},
                { 0      , 0       , 0           , 1              },
            };

            return MultiplyingVectorAndMatrix(vector, matrix);
        }

        private static Vector TransformСoordinatesProjectionToViewport(Vector vector, float width, float height, float xMin, float yMin)
        {
            var matrix = new float[,]
            {
                { width/2, 0        , 0, xMin + width/2 },
                { 0      , -height/2, 0, yMin + height/2},
                { 0      , 0        , 1, 0              },
                { 0      , 0        , 0, 1              },
            };

            return MultiplyingVectorAndMatrix(vector, matrix);
        }

        public static Vector Transform(Vector vector)
        {
            var reuslt = TransformСoordinatesModelToWorld(vector, new CoordinateVector(0,0,0), new CoordinateVector(12,13,14), new CoordinateVector(0,1,0));
            reuslt = TransformСoordinatesWorldToProjection(reuslt, 600, 600, 10,100);
            reuslt = TransformСoordinatesProjectionToViewport(reuslt,600,600,10,100);
            return reuslt;
        }

    }
}
