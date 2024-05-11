using _3DGraphics.Classes;
using System.Numerics;

namespace _3DGraphics.Drawing
{
    internal static class Triangles
    {
        public static void Draw(DrawingParams @params)
        {
            var points = new BaseGraphisStructs.CoordinateVector[@params.Coordinate.Length];
            @params.Coordinate.CopyTo(points, 0);
            @params.CoordinateOriginal  = new BaseGraphisStructs.CoordinateVector[@params.Coordinate.Length];
            @params.Coordinate.CopyTo(@params.CoordinateOriginal, 0);

            Array.Sort(points, (p1, p2) => p1.X.CompareTo(p2.X));
            Array.Sort(points, (p1, p2) => p1.Y.CompareTo(p2.Y));

            var listPointsCoordinate = new List<BaseGraphisStructs.CoordinateVector>();

            int up;
            int down;

            for (int i = 0; i < points.Length;)
            {
                if ((i + 1 != points.Length) && (points[i].Y == points[i + 1].Y))
                {
                    listPointsCoordinate.Add(points[i]);
                    listPointsCoordinate.Add(points[i + 1]);

                    i += 2;
                }
                else
                {
                    listPointsCoordinate.Add(points[i]);
                    if (i != 0 && i != points.Length - 1)
                    {
                        up = 0;
                        down = 0;
                        for (var j = 0; j < @params.Coordinate.Length - 1; j++)
                        {
                            if (@params.Coordinate[j].Y > points[i].Y && @params.Coordinate[j + 1].Y < points[i].Y)
                            {
                                up = j;
                                down = j + 1;
                            }
                            if (@params.Coordinate[j + 1].Y > points[i].Y && @params.Coordinate[j].Y < points[i].Y)
                            {
                                up = j + 1;
                                down = j;
                            }
                        }
                        if (@params.Coordinate[@params.Coordinate.Length - 1].Y > points[i].Y && @params.Coordinate[0].Y < points[i].Y)
                        {
                            up = @params.Coordinate.Length - 1;
                            down = 0;
                        }
                        if (@params.Coordinate[0].Y > points[i].Y && @params.Coordinate[@params.Coordinate.Length - 1].Y < points[i].Y)
                        {
                            up = 0;
                            down = @params.Coordinate.Length - 1;
                        }


                        listPointsCoordinate.Add(new BaseGraphisStructs.CoordinateVector(
                            (@params.Coordinate[down].X + ((@params.Coordinate[up].X - @params.Coordinate[down].X) / (@params.Coordinate[up].Y - @params.Coordinate[down].Y) * (points[i].Y - @params.Coordinate[down].Y))),
                            points[i].Y,
                            @params.Coordinate[down].Z + ((@params.Coordinate[up].Z != @params.Coordinate[down].Z) ? ((@params.Coordinate[up].Z - @params.Coordinate[down].Z) / (@params.Coordinate[up].Z - @params.Coordinate[down].Z) * (points[i].Z - @params.Coordinate[down].Z)) : 0)
                            ));
                    }
                    i++;
                }
            }

            var tmpPoints = listPointsCoordinate.ToArray();

            Array.Sort(tmpPoints, (p1, p2) => p1.X.CompareTo(p2.X));
            Array.Sort(tmpPoints, (p1, p2) => p1.Y.CompareTo(p2.Y));

            if (SettingLab.LightModelFullPolygon)
            {
                var tmpColourInt = GetPolygonLight(@params);
                if (tmpColourInt != null)
                {
                    @params.ColorInt = tmpColourInt.Value;
                    for (var i = 0; i < tmpPoints.Length - 2; i++)
                    {
                        @params.Coordinate = [tmpPoints[i], tmpPoints[i + 1], tmpPoints[i + 2]];
                        DrawTriangle(@params);
                    }
                }
            }
            else
            {
                var cosEye = CalculateCosForKillPolygon(@params, Camera.Eye);
                if (cosEye <= 0)
                {
                    return;
                }

                for (var i = 0; i < tmpPoints.Length - 2; i++)
                {
                    @params.Coordinate = [tmpPoints[i], tmpPoints[i + 1], tmpPoints[i + 2]];
                    DrawTriangle(@params);
                }

            }
        }

        private static float CalculateCosForKillPolygon(DrawingParams @params, Vector3 pointObject)
        {
            float cos = 0;

            for (var i = 0; i < @params.Normal.Length; i++)
            {
                // Вычисление вектора от точки к полигону
                var vector = @params.CoordinateToNormal[i].Coordinates - pointObject;
                var normalizedVector = Vector3.Normalize(vector);

                // Вычисление скалярного произведения нормализованного вектора и нормали полигона
                var norm = @params.Normal[i].Coordinates;
                var normalizedNorm = Vector3.Normalize(norm);

                cos += Vector3.Dot(normalizedVector, normalizedNorm);
            }
            cos /= @params.Normal.Length;

            return -cos;
        }

        public static void DrawRGB(DrawingParams @params)
        {
            @params.ColorInt = DrawingModel.GetRGBColor(@params.Coordinate);
            Draw(@params);
        }

        private static void DrawTriangle(DrawingParams @params)
        {
            const int YIncrement = 1;

            var steps1 = Math.Abs(Convert.ToInt32(@params.Coordinate[0].Y - @params.Coordinate[1].Y));
            var steps2 = Math.Abs(Convert.ToInt32(@params.Coordinate[0].Y - @params.Coordinate[2].Y));

            steps1 = steps1 == 0 ? 1 : steps1;
            steps2 = steps2 == 0 ? 1 : steps2;

            var steps = steps1 > steps2 ? steps1 : steps2;

            float y = @params.Coordinate[0].Y;

            float x1 = @params.Coordinate[0].X;
            float x2;
            float z1 = @params.Coordinate[0].Z;
            float z2;
            float XIncrement1;
            float XIncrement2;
            float ZIncrement1;
            float ZIncrement2;
            if (@params.Coordinate[0].Y == @params.Coordinate[1].Y)
            {
                float dx1 = @params.Coordinate[0].X - @params.Coordinate[2].X;
                float dx2 = @params.Coordinate[1].X - @params.Coordinate[2].X;

                x2 = @params.Coordinate[1].X;
                XIncrement1 = -dx1 / steps;
                XIncrement2 = -dx2 / steps;

                float dz1 = @params.Coordinate[0].Z - @params.Coordinate[2].Z;
                float dz2 = @params.Coordinate[1].Z - @params.Coordinate[2].Z;

                z2 = @params.Coordinate[1].Z;
                ZIncrement1 = -dz1 / steps;
                ZIncrement2 = -dz2 / steps;
            }
            else
            {
                float dx1 = @params.Coordinate[1].X - @params.Coordinate[0].X;
                float dx2 = @params.Coordinate[2].X - @params.Coordinate[0].X;

                x2 = @params.Coordinate[0].X;
                XIncrement1 = dx1 / steps;
                XIncrement2 = dx2 / steps;

                float dz1 = @params.Coordinate[1].Z - @params.Coordinate[0].Z;
                float dz2 = @params.Coordinate[2].Z - @params.Coordinate[0].Z;

                z2 = @params.Coordinate[2].Z;
                ZIncrement1 = dz1 / steps;
                ZIncrement2 = dz2 / steps;

            }

            var p1Line = new BaseGraphisStructs.CoordinateVector();
            var p2Line = new BaseGraphisStructs.CoordinateVector();

            for (var i = 0; i <= steps; i++)
            {
                p1Line.X = x1;
                p1Line.Y = y;
                p1Line.Z = z1;


                p2Line.X = x2;
                p2Line.Y = y;
                p2Line.Z = z2;

                @params.P1 = p1Line;
                @params.P2 = p2Line;
                Lines.DrawLineWithZBuffer(@params);

                x1 += XIncrement1;
                x2 += XIncrement2;
                y += YIncrement;
                z1 += ZIncrement1;
                z2 += ZIncrement2;

            }
        }

        private static int? GetPolygonLight(DrawingParams @params)
        {
            var light = 0;

            var cosEye = CalculateCos(@params, Camera.Eye);

            if (cosEye <= 0)
            {
                return null;
            }


            //Diffuse
            var cosLight = CalculateCos(@params, Camera.Light);
            var diffuseLight = Convert.ToInt32(255 * cosLight) < 0 ? 0 : Convert.ToInt32(255 * cosLight);
            light = diffuseLight - 100 < 0 ? 0 : diffuseLight - 100;

            //Ambient
            var ambientLight = 50;
            //var ambientLight = 0;

            //Specular
            int specularLight;
            if (diffuseLight != 0)
            {
                var cosSpecular = CalculateSpecularCos(@params);
                specularLight = Convert.ToInt32(255 * cosSpecular) < 0 ? 0 : Convert.ToInt32(255 * cosSpecular);
            }
            else
            {
                specularLight = 0;
            }

            //Finish
            light = light + ambientLight + specularLight > 255 ? 255 : light + ambientLight + specularLight;

            return Color.FromArgb(255, specularLight > 255 ? 255 : specularLight, specularLight > 255 ? 255 : specularLight, light).ToArgb();
        }

        private static float CalculateCos(DrawingParams @params, Vector3 pointObject)
        {
            float cos = 0;

            for (var i = 0; i < @params.Normal.Length; i++)
            {
                // Вычисление вектора от точки к полигону
                var vector = @params.CoordinateToNormal[i].Coordinates - pointObject;
                var normalizedVector = Vector3.Normalize(vector);

                // Вычисление скалярного произведения нормализованного вектора и нормали полигона
                var norm = @params.Normal[i].Coordinates;
                var normalizedNorm = Vector3.Normalize(norm);

                cos += Vector3.Dot(normalizedVector, normalizedNorm);
            }
            cos /= @params.Normal.Length;

            return -cos;
        }

        private static float CalculateSpecularCos(DrawingParams @params)
        {
            float cos = 0;

            for (var i = 0; i < @params.Normal.Length; i++)
            {
                //Свет
                var vectorLight = @params.CoordinateToNormal[i].Coordinates - Camera.Light;
                var normalizedVectorLight = Vector3.Normalize(vectorLight);

                //Нормаль
                var norm = @params.Normal[i].Coordinates;
                var normalizedNorm = Vector3.Normalize(norm);

                //Отражение
                var vectorSpecular = normalizedNorm - 2 * (normalizedVectorLight * normalizedNorm) * normalizedNorm;
                var normalizedVectorSpecular = Vector3.Normalize(vectorSpecular);

                //Камера 
                var vectorCamera = @params.CoordinateToNormal[i].Coordinates - Camera.Eye;
                var normalizedVectorCamera = Vector3.Normalize(vectorCamera);

                var cosSpeg = Vector3.Dot(normalizedVectorSpecular, normalizedVectorCamera);
                cosSpeg = float.Pow(cosSpeg, 100);

                cos += cosSpeg;
            }

            cos /= @params.Normal.Length;

            return cos;
        }
    }
}
