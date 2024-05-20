using _3DGraphics.Classes;
using System.Numerics;

namespace _3DGraphics.Drawing
{
    internal static class Lines
    {
        private delegate int GetColorDelegate(DrawingParams @params, Vector3 point);

        public static void Draw(DrawingParams @params)
        {
            for (var i = 0; i < @params.Coordinate.Length - 1;)
            {
                @params.P1 = @params.Coordinate[i];
                @params.P2 = @params.Coordinate[++i];
                DrawLineWithZBuffer(@params);
            }
            @params.P1 = @params.Coordinate[@params.Coordinate.Length - 1];
            @params.P2 = @params.Coordinate[0];
            DrawLineWithZBuffer(@params);
        }

        public static void DrawRGB(DrawingParams @params)
        {
            @params.ColorInt = DrawingModel.GetRGBColor(@params.Coordinate);
            Draw(@params);
        }

        public static unsafe void DrawLineWithZBuffer(DrawingParams @params)
        {
            var dx = @params.P2.X - @params.P1.X;
            var dy = @params.P2.Y - @params.P1.Y;
            var steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

            float XIncrement = dx / (float)steps;
            float YIncrement = dy / (float)steps;

            float x = (int)@params.P1.X;
            float y = (int)@params.P1.Y;
            int index;

            var strideInt = @params.Stride / 4;

            var dz = @params.P2.Z - @params.P1.Z;
            float ZIncrement = dz / (float)steps;

            var z = @params.P1.Z;

            int tmpColorInt;

            if (steps > 1)
            {
                steps++;
            }

            int xRound;
            int yRound;

            for (var i = 0; i <= steps; i++)
            {
                if (x > @params.WidthZone || x < 0 || y > @params.HeightZone || y < 0)
                {
                    x += XIncrement;
                    y += YIncrement;
                    z += ZIncrement;
                    continue;
                }

                xRound = (int)Math.Round(x);
                yRound = (int)Math.Round(y);

                if (ZBuffer.CheckAndSetDepth(xRound, yRound, z))
                {
                    index = xRound + yRound * strideInt;

                    try
                    {
                        tmpColorInt = SettingLab.GetColorPointFunc(@params, new BaseGraphisStructs.CoordinateVector(x, y, z));
                    }
                    catch
                    {
                        tmpColorInt = 0;
                    }

                    @params.RgbBitmap[index] = tmpColorInt;
                }

                x += XIncrement;
                y += YIncrement;
                z += ZIncrement;
            }
        }

        public static int GetPointLightUseInterpolation(DrawingParams @params, BaseGraphisStructs.CoordinateVector point)
        {
            int light;

            point = CoordinateTransformar.RevercePoint(point);

            var normal = InterpolateNormal(point, @params.CoordinateToNormal[@params.IndexesPointTriangle[0]], @params.CoordinateToNormal[@params.IndexesPointTriangle[1]], @params.CoordinateToNormal[@params.IndexesPointTriangle[2]], @params.Normal[@params.IndexesPointTriangle[0]], @params.Normal[@params.IndexesPointTriangle[1]], @params.Normal[@params.IndexesPointTriangle[2]]);

            //Diffuse
            var cosLight = CalculateCos(point, normal, Camera.Light);
            int diffuseLight = Convert.ToInt32(255 * cosLight) - 100;
            if (diffuseLight < 0)
            {
                light = 0;
            }
            else
            {
                light = diffuseLight;
            }

            //Ambient
            var ambientLight = 50;

            //Specular
            int specularLight;
            if (light != 0)
            {
                var cosSpecular = CalculateSpecularCos(point, normal);
                specularLight = Convert.ToInt32(255 * cosSpecular);
                if (specularLight < 0)
                {
                    specularLight = 0;
                }
                else
                {
                    if (specularLight > 255)
                    {
                        specularLight = 255;
                    }
                }
            }
            else
            {
                specularLight = 0;
            }

            //Finish
            light = light + ambientLight + specularLight;

            if (light > 255)
            {
                light = 255;
            }

            return Color.FromArgb(255, specularLight, specularLight, light).ToArgb();
        }

        public static int GetPointLightUseOneColourForPolygon(DrawingParams @params, BaseGraphisStructs.CoordinateVector point)
        {
            return @params.ColorInt;
        }

        private static float CalculateCos(BaseGraphisStructs.CoordinateVector pointModel, BaseGraphisStructs.NormalVector normalModel, Vector3 pointObject)
        {
            // Вычисление вектора от точки к полигону
            var vector = pointModel.Coordinates - pointObject;
            var normalizedVector = Vector3.Normalize(vector);

            // Вычисление скалярного произведения нормализованного вектора и нормали полигона
            var normalizedNorm = Vector3.Normalize(normalModel.Coordinates);

            return -Vector3.Dot(normalizedVector, normalizedNorm);
        }

        private static float CalculateSpecularCos(BaseGraphisStructs.CoordinateVector pointModel, BaseGraphisStructs.NormalVector normalModel)
        {
            //Свет
            var vectorLight = pointModel.Coordinates - Camera.Light;
            var normalizedVectorLight = Vector3.Normalize(vectorLight);

            //Нормаль
            var normalizedNorm = Vector3.Normalize(normalModel.Coordinates);

            //Отражение
            var vectorSpecular = normalizedNorm - 2 * (normalizedVectorLight * normalizedNorm) * normalizedNorm;
            var normalizedVectorSpecular = Vector3.Normalize(vectorSpecular);

            //Камера 
            var vectorCamera = pointModel.Coordinates - Camera.Eye;
            var normalizedVectorCamera = Vector3.Normalize(vectorCamera);

            var cosSpeg = Vector3.Dot(normalizedVectorSpecular, normalizedVectorCamera);
            cosSpeg = float.Pow(cosSpeg, 50);

            return cosSpeg;
        }

        public static BaseGraphisStructs.NormalVector InterpolateNormal(BaseGraphisStructs.CoordinateVector point, BaseGraphisStructs.CoordinateVector a, BaseGraphisStructs.CoordinateVector b, BaseGraphisStructs.CoordinateVector c, BaseGraphisStructs.NormalVector normalA, BaseGraphisStructs.NormalVector normalB, BaseGraphisStructs.NormalVector normalC)
        {
            // Вычисляем вектора
            Vector3 v0 = b.Coordinates - a.Coordinates;
            Vector3 v1 = c.Coordinates - a.Coordinates;
            Vector3 v2 = point.Coordinates - a.Coordinates;

            // Вычисляем скалярные произведения
            float d00 = Vector3.Dot(v0, v0);
            float d01 = Vector3.Dot(v0, v1);
            float d11 = Vector3.Dot(v1, v1);
            float d20 = Vector3.Dot(v2, v0);
            float d21 = Vector3.Dot(v2, v1);

            // Вычисляем барицентрические координаты
            float denom = d00 * d11 - d01 * d01;
            float v = (d11 * d20 - d01 * d21) / denom;
            float w = (d00 * d21 - d01 * d20) / denom;
            float u = 1.0f - v - w;

            // Интерполируем нормали
            Vector3 normal = u * normalA.Coordinates + v * normalB.Coordinates + w * normalC.Coordinates;

            // Нормализуем результирующую нормаль
            normal = Vector3.Normalize(normal);

            return new BaseGraphisStructs.NormalVector(normal);
        }
    }
}