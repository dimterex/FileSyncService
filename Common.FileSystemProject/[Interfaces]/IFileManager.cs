namespace FileSystemProject
{
    using System.Collections.Generic;

    public interface IFileManager
    {
        IList<FileInfoModel> GetFiles(string folder);
        void RemoveFile(string filePath);
        IList<string> RemoveEmptyDirectories(IList<string> directories);
        bool TryGetFileInfo(string path, out FileInfoModel fileInfoModel);
    }
}
