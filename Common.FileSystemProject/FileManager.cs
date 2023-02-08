namespace FileSystemProject
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using NLog;

    public class FileManager : IFileManager
    {
        private const string TAG = nameof(FileManager);
        private readonly IFileInfoModelFactory _fileInfoModelFactory;
        private readonly IFileSystemService _fileSystemService;
        private readonly ILogger _logger;

        public FileManager(IFileSystemService fileSystemService, IFileInfoModelFactory fileInfoModelFactory)
        {
            _fileSystemService = fileSystemService;
            _fileInfoModelFactory = fileInfoModelFactory;
            _logger = LogManager.GetLogger(TAG);
        }

        public IList<FileInfoModel> GetFiles(string folderPath)
        {
            IList<FileInfo> ls = GetFileData(folderPath);

            List<FileInfoModel> result = ls.OrderBy(info => info.LastWriteTimeUtc).Select(info => _fileInfoModelFactory.Create(info)).ToList();

            return result;
        }

        public void RemoveFile(string filePath)
        {
            _logger.Debug(() => $"Remove {filePath}");
            _fileSystemService.RemoveFile(filePath);
        }

        public IList<string> RemoveEmptyDirectories(IList<string> directories)
        {
            var result = new List<string>();

            foreach (string directory in directories)
            {
                if (string.IsNullOrWhiteSpace(directory))
                    continue;
                processDirectory(directory, result);
            }

            return result;
        }

        public FileInfoModel GetFileInfo(string path)
        {
            return _fileInfoModelFactory.Create(new FileInfo(path));
        }

        private IList<FileInfo> GetFileData(string folderPath)
        {
            var ls = new List<FileInfo>();

            if (string.IsNullOrEmpty(folderPath))
                return ls;

            IList<string> directories = _fileSystemService.GetDirectories(folderPath);
            IList<string> files = _fileSystemService.GetFiles(folderPath);

            foreach (string file in files)
            {
                var fileInfo = new FileInfo(file);
                ls.Add(fileInfo);
            }

            foreach (string folder in directories)
            {
                ls.AddRange(GetFileData(folder));
            }

            return ls;
        }

        private void processDirectory(string startLocation, IList<string> directories)
        {
            foreach (string directory in Directory.GetDirectories(startLocation))
            {
                processDirectory(directory, directories);
                if (Directory.GetFiles(directory).Length != 0)
                    continue;

                if (Directory.GetDirectories(directory).Length != 0)
                    continue;

                _fileSystemService.RemoveDirectory(directory);

                directories.Add(directory);
            }
        }
    }
}
