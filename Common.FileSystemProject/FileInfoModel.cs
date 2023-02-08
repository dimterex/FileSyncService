namespace FileSystemProject
{
    using System;

    public class FileInfoModel
    {
        public FileInfoModel(string path, long size, DateTime lastWriteTimeUtc)
        {
            Path = path;
            Size = size;
            LastWriteTimeUtc = lastWriteTimeUtc;
        }

        public string Path { get; }
        public long Size { get; }
        public DateTime LastWriteTimeUtc { get; }
    }
}
