using System.Collections.Generic;

namespace DataBaseProject
{
    public interface ISyncTableDataBase
    {
        IList<string> GetSyncStates(string login);
        void AddState(string login, string filePath);
        void RemoveSyncStates(string login, IList<string> paths);
        void RemoveSyncStatesByAvailableFolder(string login, string availableFolderPath);
    }
}