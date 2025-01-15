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
                    index = xRound + yRound * @params.RgbStride;

                    tmpColorInt = SettingLab.GetColorPointFunc(@params, new BaseGraphisStructs.CoordinateVector(x, y, z));

                    @params.RgbBitmap[index] = tmpColorInt;
                }

                x += XIncrement;
                y += YIncrement;
                z += ZIncrement;
            }
        }

        private const int MAX_SIZE_ARRAY = 4096 * 4096;

        public static unsafe int GetPointLightUseMaps(DrawingParams @params, BaseGraphisStructs.CoordinateVector point)
        {
            int light;

            point = CoordinateTransformar.RevercePoint(point);

            var texture = LinerInterpolateTexture(point, @params.CoordinateWorld[@params.IndexesPointTriangle[0]], @params.CoordinateWorld[@params.IndexesPointTriangle[1]], @params.CoordinateWorld[@params.IndexesPointTriangle[2]], @params.Texture[@params.IndexesPointTriangle[0]], @params.Texture[@params.IndexesPointTriangle[1]], @params.Texture[@params.IndexesPointTriangle[2]]);

            int x = (int)Math.Round((texture.U % 1) * @params.TextureStride);
            int y = (int)Math.Round((1 - (texture.V % 1)) * @params.TextureStride);

            int index = x + y * @params.TextureStride;

            if (index < 0 || index >= MAX_SIZE_ARRAY)
            {
                return 0;
            }

            // texture
            var textureInt = @params.TextureBitmap[index];

            var textureColor = Color.FromArgb(textureInt);

            // light

            var normalInt = @params.NormalBitmap[index];

            var normalColor = Color.FromArgb(normalInt);

            var normal = new Vector3(normalColor.R / 255f, normalColor.G / 255f, normalColor.B / 255f);
            normal = normal * 2 - Vector3.One;

            // deffuce
            var cosDeffuceLight = CalculateCos(point, new BaseGraphisStructs.NormalVector(normal), Camera.Light);
            cosDeffuceLight -= 0.3f;
            if (cosDeffuceLight < 0)
            {
                cosDeffuceLight = 0;
            }

            // ambient
            var cosAmbientLight = 0.1f;

            //Specular

            var specularInt = @params.MraoBitmap[index];

            var specularColor = Color.FromArgb(specularInt);


            var cosSpecular = CalculateSpecularCos(point, new BaseGraphisStructs.NormalVector(normal));
            cosSpecular *= specularColor.A / 255;
            if (cosSpecular < 0)
            {
                cosSpecular = 0;
            }

            // final light
            var cosLight = cosAmbientLight + cosDeffuceLight + cosSpecular;
            if (cosLight > 1)
            {
                cosLight = 1;
            }

            light = Color.FromArgb(255, (int)(cosLight * textureColor.R), (int)(cosLight * textureColor.G), (int)(cosLight * textureColor.B)).ToArgb();
            return light;
        }

        public static BaseGraphisStructs.TextureVector LinerInterpolateTexture(BaseGraphisStructs.CoordinateVector point,
                                                                               BaseGraphisStructs.CoordinateVector a,
                                                                               BaseGraphisStructs.CoordinateVector b,
                                                                               BaseGraphisStructs.CoordinateVector c,
                                                                               BaseGraphisStructs.TextureVector textureA,
                                                                               BaseGraphisStructs.TextureVector textureB,
                                                                               BaseGraphisStructs.TextureVector textureC)
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
            Vector3 coordinates = u * textureA.Coordinates + v * textureB.Coordinates + w * textureC.Coordinates;

            return new BaseGraphisStructs.TextureVector(coordinates);
        }

        public static BaseGraphisStructs.TextureVector PerspectiveInterpolateTexture(BaseGraphisStructs.CoordinateVector point,
                                                                                     BaseGraphisStructs.CoordinateVector a,
                                                                                     BaseGraphisStructs.CoordinateVector b,
                                                                                     BaseGraphisStructs.CoordinateVector c,
                                                                                     BaseGraphisStructs.TextureVector textureA,
                                                                                     BaseGraphisStructs.TextureVector textureB,
                                                                                     BaseGraphisStructs.TextureVector textureC)
        {
            // Перспективное деление для получения экранных координат
            float invZa = 1.0f / a.Z;
            float invZb = 1.0f / b.Z;
            float invZc = 1.0f / c.Z;

            // Перспективно-корректированные текстурные координаты
            float uA = textureA.U * invZa;
            float vA = textureA.V * invZa;
            float uB = textureB.U * invZb;
            float vB = textureB.V * invZb;
            float uC = textureC.U * invZc;
            float vC = textureC.V * invZc;

            // Векторные операции для вычисления барицентрических координат
            Vector3 v0 = b.Coordinates - a.Coordinates;
            Vector3 v1 = c.Coordinates - a.Coordinates;
            Vector3 v2 = point.Coordinates - a.Coordinates;

            // Скалярные произведения для вычисления барицентрических координат
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

            // Перспективно-корректированное интерполированное значение
            float interpolatedInvZ = u * invZa + v * invZb + w * invZc;
            float interpolatedU = (u * uA + v * uB + w * uC) / interpolatedInvZ;
            float interpolatedV = (u * vA + v * vB + w * vC) / interpolatedInvZ;

            return new BaseGraphisStructs.TextureVector(interpolatedU, interpolatedV,0);
        }


        public static int GetPointLightUseInterpolation(DrawingParams @params, BaseGraphisStructs.CoordinateVector point)
        {
            int light;

            point = CoordinateTransformar.RevercePoint(point);

            var normal = LinerInterpolateNormal(point, @params.CoordinateWorld[@params.IndexesPointTriangle[0]], @params.CoordinateWorld[@params.IndexesPointTriangle[1]], @params.CoordinateWorld[@params.IndexesPointTriangle[2]], @params.Normal[@params.IndexesPointTriangle[0]], @params.Normal[@params.IndexesPointTriangle[1]], @params.Normal[@params.IndexesPointTriangle[2]]);

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
            var normalizedVector = CanculateVectBeetmeen2Point(pointModel.Coordinates, pointObject);

            // Вычисление скалярного произведения нормализованного вектора и нормали полигона
            var normalizedNorm = Vector3.Normalize(normalModel.Coordinates);

            return -Vector3.Dot(normalizedVector, normalizedNorm);
        }

        private static float CalculateSpecularCos(BaseGraphisStructs.CoordinateVector pointModel, BaseGraphisStructs.NormalVector normalModel)
        {
            //Свет
            var normalizedVectorLight = CanculateVectBeetmeen2Point(pointModel.Coordinates, Camera.Light);

            //Нормаль
            var normalizedNorm = Vector3.Normalize(normalModel.Coordinates);

            //Отражение
            var vectorSpecular = normalizedNorm - 2 * (normalizedVectorLight * normalizedNorm) * normalizedNorm;
            var normalizedVectorSpecular = Vector3.Normalize(vectorSpecular);

            //Камера 
            var normalizedVectorCamera = CanculateVectBeetmeen2Point(pointModel.Coordinates, Camera.Light);

            var cosSpeg = Vector3.Dot(normalizedVectorSpecular, normalizedVectorCamera);
            cosSpeg = float.Pow(cosSpeg, 100);


            // Проверяем, смотрит ли нормаль на источник света
            var dotProduct = Vector3.Dot(normalizedVectorLight, normalizedNorm);
            if (dotProduct > 0)
            {
                return 0;
            }
            else
            {
                return cosSpeg;
            }

        }

        private static Vector3 CanculateVectBeetmeen2Point(Vector3 a, Vector3 b)
        {
            // Вычисление вектора от точки к полигону
            var vector = a - b;
            return Vector3.Normalize(vector);
        }
        public static BaseGraphisStructs.NormalVector LinerInterpolateNormal(BaseGraphisStructs.CoordinateVector point, BaseGraphisStructs.CoordinateVector a, BaseGraphisStructs.CoordinateVector b, BaseGraphisStructs.CoordinateVector c, BaseGraphisStructs.NormalVector normalA, BaseGraphisStructs.NormalVector normalB, BaseGraphisStructs.NormalVector normalC)
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