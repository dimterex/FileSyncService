namespace Service
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Service.Api.Message;

    public class FileManager
    {
        /// <summary>
        /// Загзурить файлы из папки и подпапок.
        /// </summary>
        public List<BaseFileInfo> GetFileList(string folderPath)
        {
            var baseFileInfos = new List<BaseFileInfo>();
            if (string.IsNullOrEmpty(folderPath))
                return baseFileInfos;

            var folders = Directory.GetDirectories(folderPath);
            var files = Directory.GetFiles(folderPath).ToList();

            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                string[] directory = fileInfo.Directory?.FullName.Split(Path.DirectorySeparatorChar);
                var baseFileInfo = new BaseFileInfo();
                baseFileInfo.FileName = fileInfo.Name;
                baseFileInfo.FilePath.AddRange(directory);
                baseFileInfos.Add(baseFileInfo);
            }

            foreach (var folder in folders)
            {
                baseFileInfos.AddRange(GetFileList(folder));
            }
            return baseFileInfos;
        }

        public void CompairFolders(
            ICollection<BaseFileInfo> observableCollection,
            List<BaseFileInfo> sourceFiles,
            List<BaseFileInfo> targetFiles,
            string root_path)
        {
            bool IsTrue(BaseFileInfo sourceFile, BaseFileInfo x)
            {
                if (!sourceFile.FileName.Equals(x.FileName))
                    return false;

                if (!sourceFile.FilePath.Equals(x.FilePath))
                    return false;

                return true;
            }

            foreach (var sourceFile in sourceFiles)
            {
                if (!targetFiles.Any(x => IsTrue(sourceFile, x)))
                    observableCollection.Add(sourceFile);
            }
        }
    }
}
