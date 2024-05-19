using _3DGraphics.Drawing;
using System.Numerics;

namespace _3DGraphics
{
    internal static class SettingLab
    {
        public  delegate void DrawObjectDelegate(DrawingParams @params);

        private static DrawObjectDelegate[] drawObjectFuncs = [Lines.Draw, Lines.DrawRGB, Triangles.Draw, Triangles.DrawRGB];
        public static List<DrawObjectDelegate> DrawModelFuncList = [Triangles.Draw];


        public delegate int GetColorPointDelegate(DrawingParams @params, Vector3 point);

        private static GetColorPointDelegate[] GetColorPointFuncs = [Lines.GetPointLightUseOneColourForPolygon, Lines.GetPointLightUseInterpolation];
        public static GetColorPointDelegate GetColorPointFunc = Lines.GetPointLightUseInterpolation;

    }
}
