using System.Collections.Generic;

namespace FileSystemProject
{
    public interface IFileSystemService
    {
        IList<string> GetDirectories(string path);
        IList<string> GetFiles(string path);
        void RemoveFile(string filePath);
        void RemoveDirectory(string path);
    }
}