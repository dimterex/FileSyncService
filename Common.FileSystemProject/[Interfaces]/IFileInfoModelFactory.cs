namespace FileSystemProject
{
    using System.IO;

    public interface IFileInfoModelFactory
    {
        FileInfoModel Create(FileInfo info);
        FileInfoModel Create(string path, long size, long lastWriteTimeUtc);
    }
}
