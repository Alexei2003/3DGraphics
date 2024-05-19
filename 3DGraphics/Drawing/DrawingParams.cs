using _3DGraphics.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3DGraphics.Drawing
{
    internal unsafe class DrawingParams
    {
        public BaseGraphisStructs.CoordinateVector[] Coordinate { get; set; } = null;
        public BaseGraphisStructs.CoordinateVector[] CoordinateToNormal { get; set; } = null;
        public BaseGraphisStructs.CoordinateVector[] CoordinatePolygonOriginal{ get; set; } = null;
        public BaseGraphisStructs.TextureVector[] Texture { get; set; } = null;
        public BaseGraphisStructs.NormalVector[] Normal { get; set; } = null;

        public int* RgbBitmap { get; set; } = null;

        public int Stride { get; set; } = 0;

        public int ColorInt { get; set; } = 0;

        public int WidthZone { get; set; } = 0;

        public int HeightZone { get; set; } = 0;

        public BaseGraphisStructs.CoordinateVector P1 { get; set; } = null;

        public BaseGraphisStructs.CoordinateVector P2 { get; set; } = null;

        public BaseGraphisStructs.CoordinateVector[] WNormal { get; set; } = null;
    }
}
