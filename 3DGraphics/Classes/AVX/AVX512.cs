using System.Runtime.Intrinsics;

namespace _3DGraphics.Classes.AVX
{
    internal class AVX512 : AVXBase
    {
        public float[] MultiplyVectoMatriWithAVX(float[] vector, float[] matrix)
        {
            const int VECTOR_LENGTH = 3;

            // Создаем вектор и матрицу
            var vectorAVX = Vector512.Create<float>(new float[] { vector[0], vector[1], vector[2], vector[0], vector[1], vector[2], vector[0], vector[1], vector[2], 0, 0, 0, 0, 0, 0, 0 });
            var matrixAVX = Vector512.Create<float>(matrix);

            // Умножаем вектор на матрицу
            var resultMatrixAVX = Vector512.Multiply(vectorAVX, matrixAVX);

            var result = new float[4];

            int index;
            for (var i = 0; i < VECTOR_LENGTH; i++)
            {
                index = i * VECTOR_LENGTH;
                for (var j = 0; j < VECTOR_LENGTH; j++)
                {
                    result[i] += resultMatrixAVX[index + j];
                }
            }

            return result;
        }
    }
}
