namespace FileSystemProject
{
    public class FileInfoModel
    {
        public FileInfoModel(string path, long size)
        {
            Path = path;
            Size = size;
        }

        public string Path { get; }
        public long Size { get; }
    }
}