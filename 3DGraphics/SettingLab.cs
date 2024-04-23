namespace _3DGraphics
{
    internal static class SettingLab
    {
        public enum DrawModelType { Lines, LinesRGB, Triangles, TrianglesRGB, TrianglesLines }

        //DrawModelType.Lines
        public static DrawModelType DrawModel = DrawModelType.TrianglesLines;
    }
}
