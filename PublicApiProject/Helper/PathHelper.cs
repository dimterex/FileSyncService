using System.IO;
using System.Text;

namespace PublicProject.Helper
{
    public class PathHelper
    {
        public static string[] GetListOfPath(string path)
        {
            return path.Split(Path.DirectorySeparatorChar);
        }

        public static string GetRawPath(string[] names)
        {
            var sb = new StringBuilder();
            foreach (var path in names) sb.Append($"{path}{Path.DirectorySeparatorChar}");

            var rawPath = sb.ToString();
            return rawPath.Substring(0, rawPath.Length - 1);
        }
    }
}