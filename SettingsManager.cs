using System.Collections.Generic;
using System.Linq;
using Service.DataBase;
using Service.DataBase.Dto;

namespace Service
{
    public class SettingsManager
    {
        private readonly DataBaseFactory _dataBaseFactory;

        public SettingsManager(DataBaseFactory dataBaseFactory)
        {
            _dataBaseFactory = dataBaseFactory;
        }

        public List<string> GetFoldersForClient(string login)
        {
            var result = new List<string>();
            
            using (var dataBase = _dataBaseFactory.Create())
            {
                // получаем объекты из бд и выводим на консоль
                var users = dataBase.Users.ToList().FirstOrDefault(x => x.Login == login);
            
                if (users != null)
                    result.AddRange(users.AvailableFolders);
            }
            
            return result;
        }

        public List<string> GetFilesForClient(string login)
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

        public void SaveSyncState(string login, string filePath)
        {
            using (var dataBase = _dataBaseFactory.Create())
            {
                dataBase.SyncStates.Add(new SyncState()
                {
                    Login = login,
                    FilePath = filePath
                });

                dataBase.SaveChangesAsync();
            }
        }

        public void RemoveSyncState(string login, IList<string> filesPath)
        {
            using (var dataBase = _dataBaseFactory.Create())
            {
                var syncState = from b in dataBase.SyncStates
                    where b.Login == login && filesPath.Contains(b.FilePath)
                    select b;
              

                dataBase.SyncStates.RemoveRange(syncState);
                dataBase.SaveChangesAsync();
            }
        }
    }
}