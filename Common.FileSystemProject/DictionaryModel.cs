using System.Collections.Generic;

namespace FileSystemProject
{
    public class DictionaryModel
    {
        public DictionaryModel(string path)
        {
            Path = path;
            Files = new List<FileInfoModel>();
        }

        public string Path { get; }
        public IList<FileInfoModel> Files { get; }
    }
}