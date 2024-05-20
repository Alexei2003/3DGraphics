using _3DGraphics.Classes;
using System.Numerics;

namespace _3DGraphics.Drawing
{
    internal static class Triangles
    {
        public static void Draw(DrawingParams @params)
        {
            var cosEye = CalculateCos(@params, Camera.Eye);
            if (cosEye <= 0)
            {
                return;
            }

            @params.CoordinatePolygonOriginal = @params.Coordinate;
            @params.TextureOriginal = @params.Texture;

            @params.Coordinate = new BaseGraphisStructs.CoordinateVector[3];


            if (SettingLab.GetColorPointFunc == Lines.GetPointLightUseOneColourForPolygon)
            {
                @params.ColorInt = GetPolygonLight(@params);
                for (var i = 1; i < @params.CoordinatePolygonOriginal.Length - 1; i++)
                {
                    @params.Coordinate[0] = @params.CoordinatePolygonOriginal[0];
                    @params.Coordinate[1] = @params.CoordinatePolygonOriginal[i];
                    @params.Coordinate[2] = @params.CoordinatePolygonOriginal[i + 1];

                    DrawTriangle(@params);
                }
            }
            else
            {
                @params.IndexesPointTriangle = new int[3];

                for (var i = 1; i < @params.CoordinatePolygonOriginal.Length - 1; i++)
                {
                    @params.IndexesPointTriangle[0] = 0;
                    @params.IndexesPointTriangle[1] = i;
                    @params.IndexesPointTriangle[2] = i + 1;

                    @params.Coordinate[0] = @params.CoordinatePolygonOriginal[0];
                    @params.Coordinate[1] = @params.CoordinatePolygonOriginal[i];
                    @params.Coordinate[2] = @params.CoordinatePolygonOriginal[i + 1];

                    DrawTriangle(@params);
                }
            }
        }

        public static void DrawRGB(DrawingParams @params)
        {
            @params.ColorInt = DrawingModel.GetRGBColor(@params.Coordinate);
            Draw(@params);
        }

        private static void DrawTriangle(DrawingParams @params)
        {
            const int YIncrement = 1;

            Array.Sort(@params.Coordinate, (p1, p2) => p1.X.CompareTo(p2.X));
            Array.Sort(@params.Coordinate, (p1, p2) => p1.Y.CompareTo(p2.Y));

            var steps1 = Math.Abs(@params.Coordinate[0].Y - @params.Coordinate[1].Y);
            var steps2 = Math.Abs(@params.Coordinate[0].Y - @params.Coordinate[2].Y);

            if (steps1 < 1)
            {
                steps1 = 1;
            }

            if (steps2 < 1)
            {
                steps2 = 1;
            }

            var steps = steps1 > steps2 ? steps1 : steps2;

            float y = @params.Coordinate[0].Y;
            float x1 = @params.Coordinate[0].X;
            float x2 = @params.Coordinate[0].X;
            float z1 = @params.Coordinate[0].Z;
            float z2 = @params.Coordinate[0].Z;
            float XIncrement1;
            float XIncrement2;
            float ZIncrement1;
            float ZIncrement2;

            XIncrement1 = (@params.Coordinate[1].X - @params.Coordinate[0].X) / steps1;
            XIncrement2 = (@params.Coordinate[2].X - @params.Coordinate[0].X) / steps;

            ZIncrement1 = (@params.Coordinate[1].Z - @params.Coordinate[0].Z) / steps1;
            ZIncrement2 = (@params.Coordinate[2].Z - @params.Coordinate[0].Z) / steps;

            var p1Line = new BaseGraphisStructs.CoordinateVector();
            var p2Line = new BaseGraphisStructs.CoordinateVector();

            bool change = false;

            if (steps > 1)
            {
                steps++;
            }

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

                if (!change && @params.Coordinate[1].Y - y < 0)
                {
                    change = true;
                    var tmpSteps = steps - steps1;
                    if (tmpSteps != 0)
                    {
                        XIncrement1 = (@params.Coordinate[2].X - @params.Coordinate[1].X) / tmpSteps;
                        ZIncrement1 = (@params.Coordinate[2].Z - @params.Coordinate[1].Z) / tmpSteps;
                    }
                }

                x1 += XIncrement1;
                x2 += XIncrement2;
                y += YIncrement;
                z1 += ZIncrement1;
                z2 += ZIncrement2;
            }
        }

        public static int GetPolygonLight(DrawingParams @params)
        {
            int light;

            //Diffuse
            var cosLight = CalculateCos(@params, Camera.Light);
            var diffuseLight = Convert.ToInt32(255 * cosLight) < 0 ? 0 : Convert.ToInt32(255 * cosLight);
            light = diffuseLight - 100 < 0 ? 0 : diffuseLight - 100;

            //Ambient
            var ambientLight = 50;

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
                var vector = @params.CoordinateWorld[i].Coordinates - pointObject;
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
                var vectorLight = @params.CoordinateWorld[i].Coordinates - Camera.Light;
                var normalizedVectorLight = Vector3.Normalize(vectorLight);

                //Нормаль
                var norm = @params.Normal[i].Coordinates;
                var normalizedNorm = Vector3.Normalize(norm);

                //Отражение
                var vectorSpecular = normalizedNorm - 2 * (normalizedVectorLight * normalizedNorm) * normalizedNorm;
                var normalizedVectorSpecular = Vector3.Normalize(vectorSpecular);

                //Камера 
                var vectorCamera = @params.CoordinateWorld[i].Coordinates - Camera.Eye;
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
