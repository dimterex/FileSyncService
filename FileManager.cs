namespace Service
{
    using Service.Api.Message.Common;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// TODO: Добавить модуль для управления настройками
    /// </summary>
    public class FileManager
    {
        public const string ROOT_PATH = "E:\\Downloads";

        /// <summary>
        /// Загзурить файлы из папки и подпапок.
        /// </summary>
        public List<BaseFileInfo> GetFileList()
        {
            return GetFileList(ROOT_PATH);
        }

        private List<BaseFileInfo> GetFileList(string folderPath)
        {
            var baseFileInfos = new List<BaseFileInfo>();
            if (string.IsNullOrEmpty(folderPath))
                return baseFileInfos;

            var folders = Directory.GetDirectories(folderPath);
            var files = Directory.GetFiles(folderPath).ToList();

            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                string[] directory = fileInfo.Directory?.FullName.Replace(ROOT_PATH, string.Empty).Split(Path.DirectorySeparatorChar);
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
            List<BaseFileInfo> targetFiles)
        {
            bool IsTrue(BaseFileInfo sourceFile, BaseFileInfo x)
            {
                if (!sourceFile.FileName.Equals(x.FileName))
                    return false;

                if (!string.Join(string.Empty, sourceFile.FilePath).Equals(string.Join(string.Empty, x.FilePath)))
                    return false;

                return true;
            }

            foreach (var sourceFile in sourceFiles)
            {
                if (!targetFiles.Any(x => IsTrue(sourceFile, x)))
                    observableCollection.Add(sourceFile);
            }
        }


        public string GetRealPath(BaseFileInfo baseFileInfo)
        {
            var path = Path.Combine(baseFileInfo.FilePath.ToArray());
            var filePath = Path.Combine(path, baseFileInfo.FileName);
            var realPath = Path.Combine(ROOT_PATH, filePath);
            return realPath;
        }

        public List<string> RemoveRootPath(FileInfo fileInfo)
        {
            List<string> directory = fileInfo.Directory.FullName.Split(Path.DirectorySeparatorChar).ToList();
            var path1 = ROOT_PATH.Split(Path.DirectorySeparatorChar).ToList();
            path1.ForEach(x => directory.Remove(x));
            return directory;
        }
    }
}
