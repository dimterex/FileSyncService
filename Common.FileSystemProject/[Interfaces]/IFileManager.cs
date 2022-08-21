using System.Collections.Generic;

namespace FileSystemProject
{
    public interface IFileManager
    {
        IList<FileInfoModel> GetFiles(string folder);
        void RemoveFile(string filePath);
        IList<string> RemoveEmptyDirectories(IList<string> directories);
    }
}