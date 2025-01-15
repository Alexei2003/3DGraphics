using _3DGraphics.Classes;

namespace _3DGraphics.Drawing
{
    internal unsafe class DrawingParams
    {
        public BaseGraphisStructs.CoordinateVector[] Coordinate { get; set; } = null;
        public BaseGraphisStructs.CoordinateVector[] CoordinateWorld { get; set; } = null;
        public BaseGraphisStructs.CoordinateVector[] CoordinatePolygonOriginal { get; set; } = null;
        public BaseGraphisStructs.TextureVector[] Texture { get; set; } = null;
        public BaseGraphisStructs.TextureVector[] TextureOriginal { get; set; } = null;
        public BaseGraphisStructs.NormalVector[] Normal { get; set; } = null;

        public int* TextureBitmap { get; set; } = null;

        public int TextureStride { get; set; } = 0;

        public int* NormalBitmap { get; set; } = null;

        public int NormalStride { get; set; } = 0;

        public int* MraoBitmap { get; set; } = null;

        public int MraoStride { get; set; } = 0;

        public int* RgbBitmap { get; set; } = null;

        public int RgbStride { get; set; } = 0;

        public int ColorInt { get; set; } = 0;

        public int WidthZone { get; set; } = 0;

        public int HeightZone { get; set; } = 0;

        public BaseGraphisStructs.CoordinateVector P1 { get; set; } = null;

        public BaseGraphisStructs.CoordinateVector P2 { get; set; } = null;

        public int[] IndexesPointTriangle = null;
    }
}
