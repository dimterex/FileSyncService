using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;

namespace FileSystemProject
{
    public class FileManager : IFileManager
    {
        private const string TAG = nameof(FileManager);
        private readonly IFileSystemService _fileSystemService;
        private readonly ILogger _logger;

        public FileManager(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
            _logger = LogManager.GetLogger(TAG);
        }

        public IList<FileInfoModel> GetFiles(string folderPath)
        {
            var ls = GetFileData(folderPath);

            var result = ls.OrderBy(info => info.LastWriteTime)
                .Select(info => new FileInfoModel(info.FullName, info.Length)).ToList();

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

            foreach (var directory in directories)
            {
                if (string.IsNullOrWhiteSpace(directory))
                    continue;
                processDirectory(directory, result);
            }

            return result;
        }

        private IList<FileInfo> GetFileData(string folderPath)
        {
            var ls = new List<FileInfo>();

            if (string.IsNullOrEmpty(folderPath))
                return ls;

            var directories = _fileSystemService.GetDirectories(folderPath);
            var files = _fileSystemService.GetFiles(folderPath);

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                ls.Add(fileInfo);
            }

            foreach (var folder in directories) ls.AddRange(GetFileData(folder));

            return ls;
        }

        private void processDirectory(string startLocation, IList<string> directories)
        {
            foreach (var directory in Directory.GetDirectories(startLocation))
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