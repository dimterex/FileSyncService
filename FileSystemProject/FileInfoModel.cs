namespace FileSystemProject
{
    public class FileInfoModel
    {
        public string Path { get; }
        public long Size { get; }

        public FileInfoModel(string path, long size)
        {
            Path = path;
            Size = size;
        }
    }
}