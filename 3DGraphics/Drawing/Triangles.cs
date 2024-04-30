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

            Array.Sort(points, (p1, p2) => p1.X.CompareTo(p2.X));
            Array.Sort(points, (p1, p2) => p1.Y.CompareTo(p2.Y));

            var listPoints = new List<BaseGraphisStructs.CoordinateVector>();

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


                        listPoints.Add(new BaseGraphisStructs.CoordinateVector(
                            (@params.Coordinate[down].X + ((@params.Coordinate[up].X - @params.Coordinate[down].X) / (@params.Coordinate[up].Y - @params.Coordinate[down].Y) * (points[i].Y - @params.Coordinate[down].Y))),
                            points[i].Y,
                            @params.Coordinate[down].Z + ((@params.Coordinate[up].Z != @params.Coordinate[down].Z) ? ((@params.Coordinate[up].Z - @params.Coordinate[down].Z) / (@params.Coordinate[up].Z - @params.Coordinate[down].Z) * (points[i].Z - @params.Coordinate[down].Z)) : 0)
                            ));

                    }
                    i++;
                }
            }

            var tmpPoints = listPoints.ToArray();

            Array.Sort(tmpPoints, (p1, p2) => p1.X.CompareTo(p2.X));
            Array.Sort(tmpPoints, (p1, p2) => p1.Y.CompareTo(p2.Y));

            var tmpColourInt = GetTriangleLight(@params);
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

                z2 = @params.Coordinate[0].Z;
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

        private static int? GetTriangleLight(DrawingParams @params)
        {
            var cos = CalculateCos(@params);

            if (cos >= 0)
            {
                if (cos > 1)
                {
                    return Color.FromArgb(255, 255, 255, 255).ToArgb();
                }
                var light = Convert.ToInt32(255 * cos);
                return Color.FromArgb(255, int.Abs(light), int.Abs(light), int.Abs(light)).ToArgb();
            }
            else
            {
                return null;
            }
        }

        private static float CalculateCos(DrawingParams @params)
        {

            // Вычисление вектора от точки к полигону
            var vector = @params.Coordinate[0].Coordinates - Camera.Light;
            var normalizedVector = Vector3.Normalize(vector);

            // Вычисление скалярного произведения нормализованного вектора и нормали полигона
            var norm = @params.Normal[0].Coordinates;
            var normalizedNorm = Vector3.Normalize(norm);

            var cosAngle = Vector3.Dot(normalizedVector, normalizedNorm);

            if (cosAngle > 1)
            {

            }

            return cosAngle;
        }

/*        private static Vector3 GetPolygonNormal(Vector3[] polygon)
        {
            var side1 = polygon[1] - polygon[0];
            var side2 = polygon[2] - polygon[0];
            return Vector3.Cross(side1, side2);
        }*/

    }
}
