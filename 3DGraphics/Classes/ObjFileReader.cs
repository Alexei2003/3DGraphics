using static _3DGraphics.Classes.BaseGraphisStructs;

namespace _3DGraphics.Classes
{
    internal static class ObjFileReader
    {
        private const int VERTEXS_COUNT = 10000;
        private const int VERTEX_INDEXS_COUNT = 10000;
        private const int VERTEX_POINTS_COUNT = 3;

        public class ModelData
        {
            public CoordinateVector[] GeometricVertexCoordinates { get; set; }
            public TextureVector[] TextureVertexCoordinates { get; set; }
            public NormalVertice[] NormalVertexCoordinates { get; set; }
            public int[][] GeometricVertexIndexs { get; set; }
            public int[][] TextureVertexIndexs { get; set; }
            public int[][] NormalVertexIndexs { get; set; }

            public ModelData()
            {

            }

            public ModelData(ModelData modelData)
            {
                GeometricVertexCoordinates = new CoordinateVector[modelData.GeometricVertexCoordinates.Length];
                TextureVertexCoordinates = new TextureVector[modelData.TextureVertexCoordinates.Length];
                NormalVertexCoordinates = new NormalVertice[modelData.NormalVertexCoordinates.Length];

                Parallel.For(0, modelData.GeometricVertexCoordinates.Length, (Action<int>)(i =>
                {
                    GeometricVertexCoordinates[i] = new CoordinateVector(modelData.GeometricVertexCoordinates[i].Coordinates);
                }));

                Parallel.For(0, modelData.TextureVertexCoordinates.Length, i =>
                {
                    TextureVertexCoordinates[i] = new TextureVector(modelData.TextureVertexCoordinates[i].Coordinates);
                });

                Parallel.For(0, modelData.NormalVertexCoordinates.Length, i =>
                {
                    NormalVertexCoordinates[i] = new NormalVertice(modelData.NormalVertexCoordinates[i].Coordinates);
                });

                GeometricVertexIndexs = (int[][])modelData.GeometricVertexIndexs.Clone();
                TextureVertexIndexs = (int[][])modelData.TextureVertexIndexs.Clone();
                NormalVertexIndexs = (int[][])modelData.NormalVertexIndexs.Clone();
            }

            public void SetCopyValue(ModelData modelData)
            {
                Parallel.For(0, modelData.GeometricVertexCoordinates.Length, i =>
                {
                    GeometricVertexCoordinates[i].Coordinates = modelData.GeometricVertexCoordinates[i].Coordinates;
                });

                Parallel.For(0, modelData.TextureVertexCoordinates.Length, i =>
                {
                    TextureVertexCoordinates[i].Coordinates = modelData.TextureVertexCoordinates[i].Coordinates;
                });

                Parallel.For(0, modelData.NormalVertexCoordinates.Length, i =>
                {
                    NormalVertexCoordinates[i].Coordinates = modelData.NormalVertexCoordinates[i].Coordinates;
                });
            }
        }

        public static ModelData Read(string str)
        {
            var fileStrs = File.ReadAllLines(str);

            var geometricVertexsList = new List<CoordinateVector>(VERTEXS_COUNT);
            var textureVerticesList = new List<TextureVector>(VERTEXS_COUNT);
            var normalVerticesList = new List<NormalVertice>(VERTEXS_COUNT);
            var geometricVertexIndexsList = new List<int[]>(VERTEX_INDEXS_COUNT);
            var textureVertexIndexsList = new List<int[]>(VERTEX_INDEXS_COUNT);
            var normalVertexIndexsList = new List<int[]>(VERTEX_INDEXS_COUNT);

            foreach (var line in fileStrs)
            {
                var tmpStr = line.Replace('.', ',');
                var parametersStr = tmpStr.Split(' ');
                // Координаты геометрических вершин
                if (line.Length > 0 && line[0] == 'v' && line[1] == ' ')
                {
                    switch (parametersStr.Length)
                    {
                        case 4:
                            geometricVertexsList.Add(new CoordinateVector(float.Parse(parametersStr[1]), float.Parse(parametersStr[2]), float.Parse(parametersStr[3])));
                            break;
                        case 5:
                            geometricVertexsList.Add(new CoordinateVector(float.Parse(parametersStr[1]), float.Parse(parametersStr[2]), float.Parse(parametersStr[3])));
                            break;
                    }
                }

                // Координаты текстурных вершин
                if (line.Length > 0 && line[0] == 'v' && line[1] == 't')
                {
                    switch (parametersStr.Length)
                    {
                        case 2:
                            textureVerticesList.Add(new TextureVector(float.Parse(parametersStr[1]), 0, 0));
                            break;
                        case 3:
                            textureVerticesList.Add(new TextureVector(float.Parse(parametersStr[1]), float.Parse(parametersStr[2]), 0));
                            break;
                        case 4:
                            textureVerticesList.Add(new TextureVector(float.Parse(parametersStr[1]), float.Parse(parametersStr[2]), float.Parse(parametersStr[3])));
                            break;
                    }
                }

                // Координаты текстурных вершин
                if (line.Length > 0 && line[0] == 'v' && line[1] == 'n')
                {
                    textureVerticesList.Add(new TextureVector(float.Parse(parametersStr[1]), float.Parse(parametersStr[2]), float.Parse(parametersStr[3])));
                }

                if (line.Length > 0 && line[0] == 'f' && line[1] == ' ')
                {
                    // Полигоны
                    var geometricVertexPointsList = new List<int>(VERTEX_POINTS_COUNT);
                    var textureVertexPointsList = new List<int>(VERTEX_POINTS_COUNT);
                    var normalVertexPointsList = new List<int>(VERTEX_POINTS_COUNT);

                    for (var i = 1; i < parametersStr.Length; i++)
                    {
                        var indexStr = parametersStr[i].Split('/');
                        switch (indexStr.Length)
                        {
                            case 3:
                                normalVertexPointsList.Add(Convert.ToInt32(indexStr[2]) - 1);
                                goto case 2;
                            case 2:
                                if (indexStr[1]?.Length != 0)
                                {
                                    textureVertexPointsList.Add(Convert.ToInt32(indexStr[1]) - 1);
                                }
                                goto case 1;
                            case 1:
                                geometricVertexPointsList.Add(Convert.ToInt32(indexStr[0]) - 1);
                                break;
                        }
                    }
                    if (geometricVertexPointsList.Count > 0)
                    {
                        geometricVertexIndexsList.Add([.. geometricVertexPointsList]);
                    }
                    if (textureVertexPointsList.Count > 0)
                    {
                        textureVertexIndexsList.Add([.. textureVertexPointsList]);
                    }
                    if (normalVertexPointsList.Count > 0)
                    {
                        normalVertexIndexsList.Add([.. normalVertexPointsList]);
                    }
                }
            }

            var result = new ModelData
            {
                GeometricVertexCoordinates = [.. geometricVertexsList],
                TextureVertexCoordinates = [.. textureVerticesList],
                NormalVertexCoordinates = [.. normalVerticesList],
                GeometricVertexIndexs = [.. geometricVertexIndexsList],
                TextureVertexIndexs = [.. textureVertexIndexsList],
                NormalVertexIndexs = [.. normalVertexIndexsList]
            };

            return result;
        }
    }
}
