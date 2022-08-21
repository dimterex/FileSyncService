using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Logger;
using Core.Logger._Enums_;
using Core.Logger._Interfaces_;

namespace FileSystemProject
{
    public class FileManager : IFileManager
    {
        private const string TAG = nameof(FileManager);
        private readonly IFileSystemService _fileSystemService;
        private readonly ILoggerService _loggerService;

        public FileManager(IFileSystemService fileSystemService, ILoggerService loggerService)
        {
            _fileSystemService = fileSystemService;
            _loggerService = loggerService;
        }

        public IList<FileInfoModel> GetFiles(string folderPath)
        {
            var ls = GetFileDatas(folderPath);

            var result = ls.OrderBy(info => info.LastWriteTime)
                .Select(info => new FileInfoModel(info.FullName, info.Length)).ToList();

            return result;
        }

        public void RemoveFile(string filePath)
        {
            _loggerService.SendLog(LogLevel.Debug, TAG, () => $"Remove {filePath}");
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

        private IList<FileInfo> GetFileDatas(string folderPath)
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

            foreach (var folder in directories) ls.AddRange(GetFileDatas(folder));

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