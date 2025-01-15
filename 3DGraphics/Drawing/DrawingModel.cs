using _3DGraphics.Classes;
using System.Drawing.Imaging;

namespace _3DGraphics.Drawing
{
    internal static class DrawingModel
    {
        private static List<int> colorInts = [Color.White.ToArgb(), Color.Red.ToArgb()];

        public static unsafe void Draw(Bitmap bitmap, ObjFileReader.ModelData modelData)
        {
            ZBuffer.Clear();

            var widthZone = bitmap.Width - 1;
            var heightZone = bitmap.Height - 1;

            const int RENDER_ARRAY_OUT_WINDOW = 10;

            var widthMaxReder = bitmap.Width + bitmap.Width / RENDER_ARRAY_OUT_WINDOW;
            var heightMaxReder = bitmap.Height + bitmap.Height / RENDER_ARRAY_OUT_WINDOW;

            var widthMinReder = -bitmap.Width / RENDER_ARRAY_OUT_WINDOW;
            var heightMinReder = -bitmap.Height / RENDER_ARRAY_OUT_WINDOW;


            var rgbBitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var ptr = rgbBitmapData.Scan0;
            var rgbBitmap = (int*)ptr;

            var textureBitmapData = modelData.TextureBitmap.LockBits(new Rectangle(0, 0, modelData.TextureBitmap.Width, modelData.TextureBitmap.Height), ImageLockMode.ReadOnly, modelData.TextureBitmap.PixelFormat);
            ptr = textureBitmapData.Scan0;
            var textureBitmap = (int*)ptr;

            var normalBitmapData = modelData.NormalBitmap.LockBits(new Rectangle(0, 0, modelData.NormalBitmap.Width, modelData.NormalBitmap.Height), ImageLockMode.ReadOnly, modelData.NormalBitmap.PixelFormat);
            ptr = normalBitmapData.Scan0;
            var normalBitmap = (int*)ptr;

            var mraoBitmapData = modelData.MraoBitmap.LockBits(new Rectangle(0, 0, modelData.MraoBitmap.Width, modelData.MraoBitmap.Height), ImageLockMode.ReadOnly, modelData.MraoBitmap.PixelFormat);
            ptr = mraoBitmapData.Scan0;
            var mraoBitmap = (int*)ptr;

            for (var j = 0; j < SettingLab.DrawModelFuncList.Count; j++)
            {

                for (var index = 0; index < modelData.GeometricVertexIndexs.Length; index++)
                //Parallel.For(0, modelData.GeometricVertexIndexs.Length, index =>
                {
                    var geometricPoints = new BaseGraphisStructs.CoordinateVector[modelData.GeometricVertexIndexs[index].Length];
                    var geometricToNormalPoints = new BaseGraphisStructs.CoordinateVector[modelData.GeometricVertexIndexs[index].Length];
                    BaseGraphisStructs.NormalVector[] normalPoints;
                    if (modelData.NormalVertexIndexs.Length > 0)
                    {
                        normalPoints = new BaseGraphisStructs.NormalVector[modelData.NormalVertexIndexs[index].Length];
                    }
                    else
                    {
                        normalPoints = null;
                    }
                    BaseGraphisStructs.TextureVector[] texturePoints;
                    if (modelData.TextureVertexIndexs.Length > 0)
                    {
                        texturePoints = new BaseGraphisStructs.TextureVector[modelData.TextureVertexIndexs[index].Length];
                    }
                    else
                    {
                        texturePoints = null;
                    }
                    for (var i = 0; i < modelData.GeometricVertexIndexs[index].Length; i++)
                    {
                        ref var geometricCoordinate = ref modelData.GeometricVertexCoordinates[modelData.GeometricVertexIndexs[index][i]];
                        if (geometricCoordinate.X > widthMaxReder || widthMinReder > geometricCoordinate.X || geometricCoordinate.Y > heightMaxReder || heightMinReder > geometricCoordinate.Y)
                        {
                            geometricPoints = null;
                            break;
                        }
                        geometricPoints[i] = geometricCoordinate;
                        geometricToNormalPoints[i] = modelData.GeometricVertexWorldCoordinates[modelData.GeometricVertexIndexs[index][i]];
                        if (normalPoints != null)
                        {
                            normalPoints[i] = modelData.NormalVertexCoordinates[modelData.NormalVertexIndexs[index][i]];
                        }
                        if (texturePoints != null)
                        {
                            texturePoints[i] = modelData.TextureVertexCoordinates[modelData.TextureVertexIndexs[index][i]];
                        }
                    }

                    if (geometricPoints != null)
                    {
                        SettingLab.DrawModelFuncList[j](new DrawingParams()
                        {
                            Coordinate = geometricPoints,
                            CoordinateWorld = geometricToNormalPoints,
                            Normal = normalPoints,
                            Texture = texturePoints,

                            TextureBitmap = textureBitmap,
                            TextureStride = textureBitmapData.Stride / 4,

                            NormalBitmap = normalBitmap,
                            NormalStride = normalBitmapData.Stride / 4,

                            MraoBitmap = mraoBitmap,
                            MraoStride = mraoBitmapData.Stride / 4,

                            RgbBitmap = rgbBitmap,
                            RgbStride = rgbBitmapData.Stride / 4,
                            ColorInt = colorInts[j],
                            WidthZone = widthZone,
                            HeightZone = heightZone,
                        });
                    }
                //});
                }
            }

            modelData.MraoBitmap.UnlockBits(mraoBitmapData);
            modelData.NormalBitmap.UnlockBits(normalBitmapData);
            modelData.TextureBitmap.UnlockBits(textureBitmapData);
            bitmap.UnlockBits(rgbBitmapData);
        }

        public static int GetRGBColor(BaseGraphisStructs.CoordinateVector[] points)
        {
            int r = 0;
            int g = 0;
            int b = 0;

            for (var i = 0; i < points.Length - 1; i++)
            {
                r += Convert.ToInt32(points[i].X);
                g += Convert.ToInt32(points[i].Y);
                b += Convert.ToInt32(points[i].Z);
            }
            return Color.FromArgb(255, Math.Abs(r % 255), Math.Abs(g % 255), Math.Abs(b % 255)).ToArgb();
        }
    }
}