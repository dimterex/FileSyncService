namespace FileSystemProject
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class FileSystemService : IFileSystemService
    {
        public IList<string> GetFiles(string path)
        {
            return Directory.GetFiles(path).ToList();
        }

        public void RemoveFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        public void RemoveDirectory(string path)
        {
            Directory.Delete(path, false);
        }

        IList<string> IFileSystemService.GetDirectories(string path)
        {
            return Directory.GetDirectories(path).ToList();
        }
    }
}
