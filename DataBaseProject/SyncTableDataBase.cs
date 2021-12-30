using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataBaseProject.Dto;

namespace DataBaseProject
{
    public class SyncTableDataBase : ISyncTableDataBase
    {
        private readonly IDataBaseFactory _dataBaseFactory;

        public SyncTableDataBase()
        {
            _dataBaseFactory = new DataBaseFactory();
        }

        public IList<string> GetSyncStates(string login)
        {
            var result = new List<string>();
            using (var dataBase = _dataBaseFactory.Create())
            {
                var syncStates = dataBase.SyncStates.ToList().Where(x => x.Login == login);
                foreach (var syncState in syncStates)
                {
                    result.Add(syncState.FilePath);
                }
            }

            return result;
        }

        public void AddState(string login, string filePath)
        {
            Task.Run(() =>
            {
                using (var dataBase = _dataBaseFactory.Create())
                {
                    AddSyncState(login, filePath, dataBase);
                    dataBase.ApplyChanges();
                }
            });
        }

        public void RemoveSyncStates(string login, IList<string> paths)
        {
            Task.Run(() =>
            {
                using (var dataBase = _dataBaseFactory.Create())
                {
                    var syncState = from b in dataBase.SyncStates
                        where b.Login == login && paths.Contains(b.FilePath)
                        select b;

                    dataBase.SyncStates.RemoveRange(syncState);
                    dataBase.ApplyChanges();
                }
            });
        }

        public void RemoveSyncStatesByAvailableFolder(string login, string availableFolderPath)
        {
            Task.Run(() =>
            {
                using (var dataBase = _dataBaseFactory.Create())
                {
                    
                    var syncState = from b in dataBase.SyncStates
                        where b.Login == login && b.FilePath.StartsWith(availableFolderPath)
                        select b;

                    dataBase.SyncStates.RemoveRange(syncState);
                    dataBase.ApplyChanges();
                }
            });
        }

        public void AddStates(string login, IList<string> filesPath)
        {
            Task.Run(() =>
            {
                using (var dataBase = _dataBaseFactory.Create())
                {
                    foreach (var filePath in filesPath)
                    {
                        AddSyncState(login, filePath, dataBase);
                    }
                    
                    dataBase.ApplyChanges();
                }
            });
        }
        
        private void AddSyncState(string login, string filePath, IDataBaseContext dataBase)
        {
            var syncStates = dataBase.SyncStates.ToList().Where(x => x.Login == login && x.FilePath == filePath).ToList();

            if (syncStates.Any())
                return;

            var state = new SyncState()
            {
                Login = login,
                FilePath = filePath
            };

            dataBase.SyncStates.Add(state);
        }
    }
}