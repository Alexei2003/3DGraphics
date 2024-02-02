using static _3DGraphics.Classes.BaseGraphisStructs;

namespace _3DGraphics.Classes
{
    internal class ObjFileReader
    {
        public struct FileReaderResult
        {
            public Vertex[] VertexCoordinates {  get; set; }
        }

        public static FileReaderResult Read(string str)
        {
            var fileStrs = File.ReadAllLines(str);

            var result = new FileReaderResult();

            var vertexList = new List<Vertex>();

            foreach (var line in fileStrs)
            {
                // Координаты вершин
                /// Дописать проверку
                if (line.Length > 0 && line[0] == 'v' && line[0] == ' ')
                {
                    var tmpStr = line.Replace(".", ",");
                    var subStrings = tmpStr.Split(' ');
                    vertexList.Add(new Vertex { X = float.Parse(subStrings[1]), Y = float.Parse(subStrings[2]), Z = float.Parse(subStrings[3]) });
                }
            }

            result.VertexCoordinates = vertexList.ToArray();

            return result;
        }
    }
}
