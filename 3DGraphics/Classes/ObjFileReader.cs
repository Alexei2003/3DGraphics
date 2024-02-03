using static _3DGraphics.Classes.BaseGraphisStructs;

namespace _3DGraphics.Classes
{
    internal class ObjFileReader
    {
        public struct FileReaderResult
        {
            public GeometricVertex[] GeometricVertexs { get; set; }
            public TextureVertice[] TextureVertexs { get; set; }
            public NormalVertice[] NormalVertexs { get; set; }
            public int[][] GeometricVertexIndexs { get; set; }
            public int[][] TextureVertexIndexs { get; set; }
            public int[][] NormalVertexIndexs { get; set; }
        }

        public static FileReaderResult Read(string str)
        {
            var fileStrs = File.ReadAllLines(str);

            var result = new FileReaderResult();

            var geometricVertexsList = new List<GeometricVertex>();
            var textureVerticesList = new List<TextureVertice>();
            var normalVerticesList = new List<NormalVertice>();
            var geometricVertexIndexsList = new List<int[]>();
            var textureVertexIndexsList = new List<int[]>();
            var normalVertexIndexsList = new List<int[]>();

            foreach (var line in fileStrs)
            {
                var tmpStr = line.Replace(".", ",");
                var subStr = tmpStr.Split(' ');
                // Координаты геометрических вершин
                /// Дописать проверку
                if (line.Length > 0 && line[0] == 'v' && line[1] == ' ')
                {
                    switch (subStr.Length)
                    {
                        case 4:
                            geometricVertexsList.Add(new GeometricVertex(float.Parse(subStr[1]), float.Parse(subStr[2]), float.Parse(subStr[3]), 1));
                            break;
                        case 5:
                            geometricVertexsList.Add(new GeometricVertex(float.Parse(subStr[1]), float.Parse(subStr[2]), float.Parse(subStr[3]), float.Parse(subStr[4])));
                            break;
                    }
                }

                // Координаты текстурных вершин
                if (line.Length > 0 && line[0] == 'v' && line[1] == 't')
                {
                    switch (subStr.Length)
                    {
                        case 2:
                            textureVerticesList.Add(new TextureVertice(float.Parse(subStr[1]), 0, 0));
                            break;
                        case 3:
                            textureVerticesList.Add(new TextureVertice(float.Parse(subStr[1]), float.Parse(subStr[2]), 0));
                            break;
                        case 4:
                            textureVerticesList.Add(new TextureVertice(float.Parse(subStr[1]), float.Parse(subStr[2]), float.Parse(subStr[3])));
                            break;
                    }
                }

                // Координаты текстурных вершин
                if (line.Length > 0 && line[0] == 'v' && line[1] == 'n')
                {
                    textureVerticesList.Add(new TextureVertice(float.Parse(subStr[1]), float.Parse(subStr[2]), float.Parse(subStr[3])));
                }

                if (line.Length > 0 && line[0] == 'f' && line[1] == ' ')
                {
                    int[] indexs;
                    switch (subStr.Length)
                    {
                        case 4:
                            indexs = new int[3];
                            break;
                        case 5:
                            indexs = new int[4];
                            break;
                    }

                    var tmpList1 = new List<int>();
                    var tmpList2 = new List<int>();
                    var tmpList3 = new List<int>();

                    for (var i = 1; i < subStr.Length; i++)
                    {
                        var indexStr = subStr[i].Split('/');
                        switch (indexStr.Length)
                        {
                            case 3:
                                tmpList3.Add(Convert.ToInt32(indexStr[2]) - 1);
                                goto case 2;
                            case 2:
                                if (indexStr[1] != "")
                                {
                                    tmpList2.Add(Convert.ToInt32(indexStr[1]) - 1);
                                }
                                goto case 1;
                            case 1:
                                tmpList1.Add(Convert.ToInt32(indexStr[0]) - 1);
                                break;
                        }
                    }
                    if (tmpList1.Count > 0)
                    {
                        geometricVertexIndexsList.Add([.. tmpList1]);
                    }
                    if (tmpList2.Count > 0)
                    {
                        textureVertexIndexsList.Add([.. tmpList2]);
                    }
                    if (tmpList3.Count > 0)
                    {
                        normalVertexIndexsList.Add([.. tmpList3]);
                    }
                }
            }

            result.GeometricVertexs = [.. geometricVertexsList];
            result.TextureVertexs = [.. textureVerticesList];
            result.NormalVertexs = [.. normalVerticesList];
            result.GeometricVertexIndexs = [.. geometricVertexIndexsList];
            result.TextureVertexIndexs = [.. textureVertexIndexsList];
            result.NormalVertexIndexs = [.. normalVertexIndexsList];

            return result;
        }
    }
}
