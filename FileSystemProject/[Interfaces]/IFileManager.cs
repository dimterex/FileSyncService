using System.Collections.Generic;

namespace FileSystemProject
{
    public interface IFileManager
    {
        IList<string> CompairFolders(IList<string> syncFiles, IList<string> fileActionFiles);
        IList<string> GetFiles(string folder);
        void RemoveFiles(IList<string> toRemove);
    }
}