namespace Service
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class FileManager
    {
        public List<string> GetFileList(string folderPath)
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
                baseFileInfos.AddRange(GetFileList(folder));
            }
            return baseFileInfos;
        }

        public void CompairFolders(
            ICollection<string> observableCollection,
            List<string> sourceFiles,
            List<string> targetFiles)
        {
            bool IsTrue(string sourceFile, string x)
            {
                if (!sourceFile.Equals(x))
                    return false;

                return true;
            }

            foreach (var sourceFile in sourceFiles)
            {
                if (!targetFiles.Any(x => IsTrue(sourceFile, x)))
                    observableCollection.Add(sourceFile);
            }
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
