using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;

namespace FileSystemProject
{
    public class FileManager : IFileManager
    {
        private readonly ILogger _logger;

        public FileManager()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }
            
        public IList<string> GetFiles(string folderPath)
        {
            var ls = GetFileDatas(folderPath);

            var result = ls.OrderBy(info => info.LastWriteTime).Select(info => info.FullName).ToList();
            
            return result;
        }

        private IList<FileInfo> GetFileDatas(string folderPath)
        {
            var ls = new List<FileInfo>();

            if (string.IsNullOrEmpty(folderPath))
                return ls;

            var folders = Directory.GetDirectories(folderPath);
            var files = Directory.GetFiles(folderPath).ToList();

            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                ls.Add(fileInfo);
            }

            foreach (var folder in folders)
            {
                ls.AddRange(GetFileDatas(folder));
            }

            return ls;
        }

        public IList<string> CompairFolders(IList<string> sourceFiles, IList<string> targetFiles)
        {
            var result = new List<string>();
            
            foreach (var sourceFile in sourceFiles)
            {
                if (targetFiles.All(x => sourceFile != x))
                    result.Add(sourceFile);
            }

            return result;
        }

        public void RemoveFile(string filePath)
        {
            _logger.Debug(() => $"Remove {filePath}");
            if (File.Exists(filePath))
                File.Delete(filePath);
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
        
        private void processDirectory(string startLocation, IList<string> directories)
        {
            foreach (var directory in Directory.GetDirectories(startLocation))
            {
                processDirectory(directory, directories);
                if (Directory.GetFiles(directory).Length != 0)
                    continue;
                        
                if (Directory.GetDirectories(directory).Length != 0)
                    continue;
                        
                // Directory.Delete(directory, false);
                directories.Add(directory);
            }
        }
    }
}