using _3DGraphics.Classes;
using System.Drawing.Imaging;
using static _3DGraphics.Classes.BaseGraphisStructs;

namespace _3DGraphics.Drawing
{
    internal static class Triangles
    {
        public static unsafe void Draw(BaseGraphisStructs.Point3DF[] points, int* rgbBitmap, BitmapData bitmapData, int colorInt, int widthZone, int heightZone)
        {
            if (points != null)
            {
                var pointsOriginal = new BaseGraphisStructs.Point3DF[points.Length];
                points.CopyTo(pointsOriginal, 0);

                Array.Sort(points, (p1, p2) => p1.X.CompareTo(p2.X));
                Array.Sort(points, (p1, p2) => p1.Y.CompareTo(p2.Y));

                var listPoints = new List<BaseGraphisStructs.Point3DF>();

                int up;
                int down;

                for (int i = 0; i < points.Length;)
                {
                    if ((i + 1 != points.Length) && (points[i].Y == points[i + 1].Y))
                    {
                        listPoints.Add(points[i]);
                        listPoints.Add(points[i + 1]);

                        i += 2;
                    }
                    else
                    {
                        listPoints.Add(points[i]);
                        if (i != 0 && i != points.Length - 1)
                        {
                            up = 0;
                            down = 0;
                            for (var j = 0; j < pointsOriginal.Length - 1; j++)
                            {
                                if (pointsOriginal[j].Y > points[i].Y && pointsOriginal[j + 1].Y < points[i].Y)
                                {
                                    up = j;
                                    down = j + 1;
                                }
                                if (pointsOriginal[j + 1].Y > points[i].Y && pointsOriginal[j].Y < points[i].Y)
                                {
                                    up = j + 1;
                                    down = j;
                                }
                            }
                            if (pointsOriginal[pointsOriginal.Length - 1].Y > points[i].Y && pointsOriginal[0].Y < points[i].Y)
                            {
                                up = pointsOriginal.Length - 1;
                                down = 0;
                            }
                            if (pointsOriginal[0].Y > points[i].Y && pointsOriginal[pointsOriginal.Length - 1].Y < points[i].Y)
                            {
                                up = 0;
                                down = pointsOriginal.Length - 1;
                            }


                            listPoints.Add(new BaseGraphisStructs.Point3DF((
                                pointsOriginal[down].X + ((pointsOriginal[up].X - pointsOriginal[down].X) / (pointsOriginal[up].Y - pointsOriginal[down].Y) * (points[i].Y - pointsOriginal[down].Y))),
                                points[i].Y,
                                pointsOriginal[down].Z + ((pointsOriginal[up].Z != pointsOriginal[down].Z) ? ((pointsOriginal[up].Z - pointsOriginal[down].Z) / (pointsOriginal[up].Z - pointsOriginal[down].Z) * (points[i].Z - pointsOriginal[down].Z)) : 0))
                                );

                        }
                        i++;
                    }
                }

                var tmpPoints = listPoints.ToArray();

                Array.Sort(tmpPoints, (p1, p2) => p1.X.CompareTo(p2.X));
                Array.Sort(tmpPoints, (p1, p2) => p1.Y.CompareTo(p2.Y));

                var tmpColorInt = GetTriangleLight(pointsOriginal);
                if (tmpColorInt != null)
                {
                    for (var i = 0; i < tmpPoints.Length - 2; i++)
                    {

                        DrawTriangle([tmpPoints[i], tmpPoints[i + 1], tmpPoints[i + 2]], rgbBitmap, bitmapData, tmpColorInt.Value, widthZone, heightZone);
                    }
                }
            }
        }

        public static unsafe void DrawRGB(BaseGraphisStructs.Point3DF[] points, int* rgbBitmap, BitmapData bitmapData, int colorInt, int widthZone, int heightZone)
        {
            if (points != null)
            {
                colorInt = DrawingModel.GetRGBColor(points);
                Draw(points, rgbBitmap, bitmapData, colorInt, widthZone, heightZone);
            }
        }

        private static unsafe void DrawTriangle(BaseGraphisStructs.Point3DF[] points, int* rgbBitmap, BitmapData bitmapData, int colorInt, int widthZone, int heightZone)
        {
            const int YIncrement = 1;

            var steps1 = Math.Abs(Convert.ToInt32(points[0].Y - points[1].Y));
            var steps2 = Math.Abs(Convert.ToInt32(points[0].Y - points[2].Y));

            steps1 = steps1 == 0 ? 1 : steps1;
            steps2 = steps2 == 0 ? 1 : steps2;

            var steps = steps1 > steps2 ? steps1 : steps2;

            float y = points[0].Y;

            float x1 = points[0].X;
            float x2;
            float z1 = points[0].Z;
            float z2;
            float XIncrement1;
            float XIncrement2;
            float ZIncrement1;
            float ZIncrement2;
            if (points[0].Y == points[1].Y)
            {
                float dx1 = points[0].X - points[2].X;
                float dx2 = points[1].X - points[2].X;

                x2 = points[1].X;
                XIncrement1 = -dx1 / steps;
                XIncrement2 = -dx2 / steps;

                float dz1 = points[0].Z - points[2].Z;
                float dz2 = points[1].Z - points[2].Z;

                z2 = points[1].Z;
                ZIncrement1 = -dz1 / steps;
                ZIncrement2 = -dz2 / steps;
            }
            else
            {
                float dx1 = points[1].X - points[0].X;
                float dx2 = points[2].X - points[0].X;

                x2 = points[0].X;
                XIncrement1 = dx1 / steps;
                XIncrement2 = dx2 / steps;

                float dz1 = points[1].Z - points[0].Z;
                float dz2 = points[2].Z - points[0].Z;

                z2 = points[0].Z;
                ZIncrement1 = dz1 / steps;
                ZIncrement2 = dz2 / steps;

            }

            var p1Line = new BaseGraphisStructs.Point3DF();
            var p2Line = new BaseGraphisStructs.Point3DF();

            for (var i = 0; i <= steps; i++)
            {
                p1Line.X = x1;
                p1Line.Y = y;
                p1Line.Z = z1;

                p2Line.X = x2;
                p2Line.Y = y;
                p2Line.Z = z2;

                Lines.DrawLineWithZBuffer(rgbBitmap, bitmapData.Stride, colorInt, p1Line, p2Line, widthZone, heightZone);

                x1 += XIncrement1;
                x2 += XIncrement2;
                y += YIncrement;
                z1 += ZIncrement1;
                z2 += ZIncrement2;

            }
        }

        private static int? GetTriangleLight(Point3DF[] points)
        {
            var cos = CalculateCos(points);

            if (cos > 0)
            {
                var light = Convert.ToInt32(255 * cos.Value);
                return Color.FromArgb(255, int.Abs(light), int.Abs(light), int.Abs(light)).ToArgb();
            }
            else
            {
                return null;
            }
        }

        private static float? CalculateCos(Point3DF[] points)
        {
            var normals = new Point3DF[points.Length];

            for ( var i = 0; i<normals.Length-2; i++)
            {
                normals[i] = CalculatePointNormal(points[i], points[i+1], points[i+2]);
            }

            var max = normals.Length - 1;

            normals[max-1] = CalculatePointNormal(points[max-1], points[max], points[0]);
            normals[max] = CalculatePointNormal(points[max], points[0], points[1]);

            // Вычисляем вектор от камеры к каждой вершине
            var vectors = new Point3DF[points.Length];

            for( var i = 0; i < vectors.Length; i++)
            {
                vectors[i] = new Point3DF(points[i].X - Camera.Light.X, points[i].Y - Camera.Light.Y, points[i].Z - Camera.Light.Z);
            }

            float cos = 0;

            for(var i = 0; i< points.Length; i++)
            {
                cos = CalculateCosBetweenVectors(vectors[i], normals[i]);
            }

            // Возвращаем среднее значение косинусов углов для каждой вершины
            return cos / points.Length;
        }

        private static float CalculateCosBetweenVectors(Point3DF vector, Point3DF normal)
        {
            // Вычисляем длины вектора и нормали
            var vectorLength = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
            var normalLength = (float)Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);

            // Вычисляем скалярное произведение вектора и нормали
            var dotProduct = vector.X * normal.X + vector.Y * normal.Y + vector.Z * normal.Z;

            // Вычисляем косинус угла
            return dotProduct / (vectorLength * normalLength);
        }

        private static Point3DF CalculatePointNormal(Point3DF p1, Point3DF p2, Point3DF p3)
        {
            // Вычисляем векторы p1p2 и p1p3
            var p1p2X = p2.X - p1.X;
            var p1p2Y = p2.Y - p1.Y;
            var p1p2Z = p2.Z - p1.Z;

            var p1p3X = p3.X - p1.X;
            var p1p3Y = p3.Y - p1.Y;
            var p1p3Z = p3.Z - p1.Z;

            // Вычисляем векторное произведение p1p2 и p1p3
            var normalX = p1p2Y * p1p3Z - p1p2Z * p1p3Y;
            var normalY = p1p2Z * p1p3X - p1p2X * p1p3Z;
            var normalZ = p1p2X * p1p3Y - p1p2Y * p1p3X;

            // Нормализуем полученный вектор нормали
            var length = (float)Math.Sqrt(normalX * normalX + normalY * normalY + normalZ * normalZ);
            if (length != 0)
            {
                normalX /= length;
                normalY /= length;
                normalZ /= length;
            }

            return new Point3DF(normalX, normalY, normalZ);
        }

    }
}
