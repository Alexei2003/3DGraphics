using _3DGraphics.Classes;
using _3DGraphics.Drawing;

namespace _3DGraphics
{
    internal static class SettingLab
    {
        public delegate void DrawObjectDelegate(DrawingParams @params);

        private static DrawObjectDelegate[] drawObjectFuncs = [Lines.Draw, Lines.DrawRGB, Triangles.Draw, Triangles.DrawRGB];
        public static List<DrawObjectDelegate> DrawModelFuncList = [Triangles.Draw];


        public delegate int GetColorPointDelegate(DrawingParams @params, BaseGraphisStructs.CoordinateVector point);

        private static GetColorPointDelegate[] GetColorPointFuncs = [Lines.GetPointLightUseOneColourForPolygon, Lines.GetPointLightUseInterpolation, Lines.GetPointLightUseMaps];
        public static GetColorPointDelegate GetColorPointFunc = Lines.GetPointLightUseMaps;

    }
}
