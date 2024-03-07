using _3DGraphics.Classes.AVX;
using static _3DGraphics.Classes.BaseGraphisStructs;

namespace _3DGraphics.Classes
{
    internal class CoordinateTransformations
    {
        private readonly AVXBase avx;
        private readonly int maxThreads = Environment.ProcessorCount;

        public CoordinateTransformations()
        {
            avx = new AVX512();
        }

        public void TranslateVectors(GeometricVertex[] vectors, CoordinateVector translation)
        {
            Parallel.For(0, vectors.Length, i =>
            {
                vectors[i].TranslateX += translation.X;
                vectors[i].TranslateY += translation.Y;
                vectors[i].TranslateZ += translation.Z;
            });
        }

        public void ScaleVectors(GeometricVertex[] vectors, CoordinateVector scale)
        {
            /*            Parallel.For(0, vectors.Length, i =>
                        {
                            vectors[i].X *= scale.X;
                            vectors[i].Y *= scale.Y;
                            vectors[i].Z *= scale.Z;
                        });*/



            var matrix = new float[16] { scale.X, 0, 0, 0, scale.Y, 0, 0, 0, scale.Z, 0, 0, 0, 0, 0, 0, 0 };

            Parallel.For(0, vectors.Length, i =>
             {
                 var tmpVector = new float[3] { vectors[i].X, vectors[i].Y, vectors[i].Z };

                 avx.MultiplyVectoMatriWithAVX(tmpVector, matrix).CopyTo(vectors[i].Coordinates, 0);
             });
        }

        public void RotateVectorsAroundX(GeometricVertex[] vectors, double angle)
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);

            Parallel.For(0, vectors.Length, i =>
            {
                var y = vectors[i].Y;

                vectors[i].Y = y * cos + vectors[i].Z * sin;
                vectors[i].Z = y * -sin + vectors[i].Z * cos;
            });
        }

        public void RotateVectorsAroundY(GeometricVertex[] vectors, double angle)
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);

            Parallel.For(0, vectors.Length, i =>
            {
                var x = vectors[i].X;

                vectors[i].X = x * cos + vectors[i].Z * -sin;
                vectors[i].Z = x * sin + vectors[i].Z * cos;
            });
        }

        public void RotateVectorsAroundZ(GeometricVertex[] vectors, double angle)
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);

            Parallel.For(0, vectors.Length, i =>
            {
                var x = vectors[i].X;

                vectors[i].X = x * cos + vectors[i].Y * sin;
                vectors[i].Y = x * -sin + vectors[i].Y * cos;
            });
        }
    }
}
