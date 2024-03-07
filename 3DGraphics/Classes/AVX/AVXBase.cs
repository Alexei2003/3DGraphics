namespace _3DGraphics.Classes.AVX
{
    internal interface AVXBase
    {
        /// <summary>
        /// Матрицу задавать в строчку сначала стольбец, потом строка оставшееся до 16 заполнить 0
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public float[] MultiplyVectoMatriWithAVX(float[] vector, float[] matrix);
    }
}
