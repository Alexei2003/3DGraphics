using static _3DGraphics.Classes.BaseGraphisStructs;

namespace _3DGraphics.Classes
{
    internal class ObjFileReader
    {
        public struct FileReaderResult
        {
            public Vector[] Vertexs { get; set; }
        }

        public static FileReaderResult Read(string str)
        {
            var fileStrs = File.ReadAllLines(str);

            var result = new FileReaderResult();

            var vertexList = new List<Vector>();

            foreach (var line in fileStrs)
            {
                // Координаты вершин
                /// Дописать проверку
                if (line.Length > 0 && line[0] == 'v' && line[1] == ' ')
                {
                    var tmpStr = line.Replace(".", ",");
                    var subStrings = tmpStr.Split(' ');
                    vertexList.Add(new Vector(float.Parse(subStrings[1]), float.Parse(subStrings[2]), float.Parse(subStrings[3]), 1)); /// 4вертая координата не парсится
                }
            }

            result.Vertexs = vertexList.ToArray();

            return result;
        }
    }
}
