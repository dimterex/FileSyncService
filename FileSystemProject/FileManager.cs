using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileSystemProject
{
    public class FileManager : IFileManager
    {
        public IList<string> GetFiles(string folderPath)
        {
            var baseFileInfos = new List<string>();
            if (string.IsNullOrEmpty(folderPath))
                return baseFileInfos;

            var folders = Directory.GetDirectories(folderPath);
            var files = Directory.GetFiles(folderPath).ToList();

            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                baseFileInfos.Add(fileInfo.FullName);
            }

            foreach (var folder in folders)
            {
                baseFileInfos.AddRange(GetFiles(folder));
            }
            return baseFileInfos;
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


        public void RemoveFiles(IList<string> toRemoveList)
        {
            foreach (var filePath in toRemoveList)
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }
    }
}