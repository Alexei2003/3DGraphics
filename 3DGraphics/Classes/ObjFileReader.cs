using static _3DGraphics.Classes.BaseGraphisStructs;

namespace _3DGraphics.Classes
{
    internal class ObjFileReader
    {
        public struct FileReaderResult
        {
            public GeometricVertex[] GeometricVertexs { get; set; }
            public TextureVertice[] TextureVertices { get; set; }
            public NormalVertice[] NormalVertices { get; set; }
        }

        public static FileReaderResult Read(string str)
        {
            var fileStrs = File.ReadAllLines(str);

            var result = new FileReaderResult();

            var geometricVertexsList = new List<GeometricVertex>();
            var textureVerticesList = new List<TextureVertice>();
            var normalVerticesList = new List<NormalVertice>();

            foreach (var line in fileStrs)
            {
                var tmpStr = line.Replace(".", ",");
                var subStrings = tmpStr.Split(' ');
                // Координаты геометрических вершин
                /// Дописать проверку
                if (line.Length > 0 && line[0] == 'v' && line[1] == ' ')
                {
                    switch (subStrings.Length)
                    {
                        case 4:
                            geometricVertexsList.Add(new GeometricVertex(float.Parse(subStrings[1]), float.Parse(subStrings[2]), float.Parse(subStrings[3]), 1));
                            break;
                        case 5:
                            geometricVertexsList.Add(new GeometricVertex(float.Parse(subStrings[1]), float.Parse(subStrings[2]), float.Parse(subStrings[3]), float.Parse(subStrings[4])));
                            break;
                    }
                }

                // Координаты текстурных вершин
                if (line.Length > 0 && line[0] == 'v' && line[1] == 't')
                {
                    switch (subStrings.Length)
                    {
                        case 2:
                            textureVerticesList.Add(new TextureVertice(float.Parse(subStrings[1]), 0, 0));
                            break;
                        case 3:
                            textureVerticesList.Add(new TextureVertice(float.Parse(subStrings[1]), float.Parse(subStrings[2]), 0));
                            break;
                        case 4:
                            textureVerticesList.Add(new TextureVertice(float.Parse(subStrings[1]), float.Parse(subStrings[2]), float.Parse(subStrings[3])));
                            break;
                    }
                }

                // Координаты текстурных вершин
                if (line.Length > 0 && line[0] == 'v' && line[1] == 'n')
                {
                    textureVerticesList.Add(new TextureVertice(float.Parse(subStrings[1]), float.Parse(subStrings[2]), float.Parse(subStrings[3])));
                }

            }

            result.GeometricVertexs = geometricVertexsList.ToArray();
            result.TextureVertices = textureVerticesList.ToArray();
            result.NormalVertices = normalVerticesList.ToArray();

            return result;
        }
    }
}
