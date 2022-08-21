using System.Linq;
using Common.DatabaseProject._Interfaces_;

namespace PublicProject.Database.Actions.States
{
    public class RemoveSyncStatesByAvailableFolderExecutor
    {
        private readonly IDataBaseFactory _dataBaseFactory;

        public RemoveSyncStatesByAvailableFolderExecutor(IDataBaseFactory dataBaseFactory)
        {
            _dataBaseFactory = dataBaseFactory;
        }

        public void Handler(string login, string availableFolder)
        {
            using (var dataBase = _dataBaseFactory.Create())
            {
                var syncState = from b in dataBase.SyncStates
                    where b.Login == login && b.FilePath.StartsWith(availableFolder)
                    select b;

                dataBase.SyncStates.RemoveRange(syncState);
                dataBase.ApplyChanges();
            }
        }
    }
}