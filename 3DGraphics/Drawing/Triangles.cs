﻿using System.Drawing.Imaging;
using static _3DGraphics.Drawing.DrawingModel;

namespace _3DGraphics.Drawing
{
    internal static class Triangles
    {
        public static unsafe void Draw(Point3DF[] points, int* rgbBitmap, BitmapData bitmapData, int colorInt, int widthZone, int heightZone)
        {
            if (points != null)
            {
                var pointsOriginal = new Point3DF[points.Length];
                points.CopyTo(pointsOriginal, 0);

                Array.Sort(points, (p1, p2) => p1.X.CompareTo(p2.X));
                Array.Sort(points, (p1, p2) => p1.Y.CompareTo(p2.Y));

                var listPoints = new List<Point3DF>();

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


                            listPoints.Add(new Point3DF((
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

                for (var i = 0; i < tmpPoints.Length - 2; i++)
                {
                    try
                    {
                        DrawTriangle([tmpPoints[i], tmpPoints[i + 1], tmpPoints[i + 2]], rgbBitmap, bitmapData, colorInt, widthZone, heightZone);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        public static unsafe void DrawRGB(Point3DF[] points, int* rgbBitmap, BitmapData bitmapData, int colorInt, int widthZone, int heightZone)
        {
            if (points != null)
            {
                colorInt = DrawingModel.GetRGBColor(points);
                Draw(points, rgbBitmap, bitmapData, colorInt, widthZone, heightZone);
            }
        }

        private static unsafe void DrawTriangle(Point3DF[] points, int* rgbBitmap, BitmapData bitmapData, int colorInt, int widthZone, int heightZone)
        {
            colorInt = GetRGBLight(points);

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


            var p1Line = new Point3DF();
            var p2Line = new Point3DF();

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
    }
}