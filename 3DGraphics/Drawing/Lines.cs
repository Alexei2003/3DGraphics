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

            float x = @params.P1.X;
            float y = @params.P1.Y;
            int index;

            var strideInt = @params.Stride / 4;

            var dz = @params.P2.Z - @params.P1.Z;
            float ZIncrement = dz / (float)steps;

            var z = @params.P1.Z;

            int tmpColorInt;

            for (var i = 0; i <= steps; i++)
            {
                tmpColorInt = SettingLab.GetColorPointFunc(@params, new Vector3(x, y, z));

                if (x > @params.WidthZone || x < 0 || y > @params.HeightZone || y < 0)
                {
                    x += XIncrement;
                    y += YIncrement;
                    z += ZIncrement;
                    continue;
                }

                if (ZBuffer.CheckAndSetDepth((int)Math.Round(x), (int)Math.Round(y), z))
                {
                    index = (int)Math.Round(x) + (int)Math.Round(y) * strideInt;

                    @params.RgbBitmap[index] = tmpColorInt;
                }

                x += XIncrement;
                y += YIncrement;
                z += ZIncrement;
            }
        }

        public static int GetPointLightUseInterpolation(DrawingParams @params, Vector3 point)
        {
            int light;

            var wNormal = GetWNormal(@params.CoordinateOriginal, point);

            //Diffuse
            var cosLight = CalculateCos(@params, Camera.Light, wNormal);
            var diffuseLight = Convert.ToInt32(255 * cosLight) < 0 ? 0 : Convert.ToInt32(255 * cosLight);
            light = diffuseLight - 100 < 0 ? 0 : diffuseLight - 100;
            //light = diffuseLight - 50 < 0 ? 0 : diffuseLight - 50;

            //Ambient
            var ambientLight = 50;
            //var ambientLight = 0;

            //Specular
            int specularLight;
            if (diffuseLight != 0)
            {
                var cosSpecular = CalculateSpecularCos(@params, wNormal);
                //var cosSpecular = 0;
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

        public static int GetPointLightUseOneColourForPolygon(DrawingParams @params, Vector3 point)
        {
            return @params.ColorInt;
        }

        public static float[] GetWNormal(BaseGraphisStructs.CoordinateVector[] points, Vector3 pointObject)
        {
            var wNormal = new float[points.Length];

            float pointDistance;
            float sumWNormal = 0;
            for (var i = 0; i < wNormal.Length; i++)
            {
                pointDistance = Vector3.Distance(points[i].Coordinates, pointObject);
                if (pointDistance == 0)
                {
                    Array.Clear(wNormal, 0, wNormal.Length);
                    wNormal[i] = 1;
                    return wNormal;
                }
                else
                {
                    wNormal[i] = 1 / pointDistance;
                }
            }

            for (var i = 0; i < wNormal.Length; i++)
            {
                sumWNormal += wNormal[i];
            }

            for (var i = 0; i < wNormal.Length; i++)
            {
                wNormal[i] /= sumWNormal;
            }

            return wNormal;
        }

        private static float CalculateCos(DrawingParams @params, Vector3 pointObject, float[] WNormal)
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

                cos += Vector3.Dot(normalizedVector, normalizedNorm) * WNormal[i];
            }
            return -cos;
        }

        private static float CalculateSpecularCos(DrawingParams @params, float[] WNormal)
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
                cosSpeg = float.Pow(cosSpeg, 50) * WNormal[i];

                cos += cosSpeg;
            }
            return cos;
        }
    }
}