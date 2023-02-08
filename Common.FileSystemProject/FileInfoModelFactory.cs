namespace FileSystemProject
{
    using System;
    using System.IO;

    public class FileInfoModelFactory : IFileInfoModelFactory
    {
        private readonly DateTime _startDate;
        public FileInfoModelFactory()
        {
            _startDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }
        
        public FileInfoModel Create(FileInfo info)
        {
            return new FileInfoModel(info.FullName, info.Length, info.LastWriteTimeUtc);
        }

        public FileInfoModel Create(string path, long size, long lastWriteTimeUtc)
        {
            DateTime date = _startDate.AddMilliseconds(lastWriteTimeUtc);

            return new FileInfoModel(path, size, date);
        }
    }
}
