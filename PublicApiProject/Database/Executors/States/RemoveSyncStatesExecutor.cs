using System.Linq;
using Common.DatabaseProject._Interfaces_;

namespace PublicProject.Database.Actions.States
{
    public class RemoveSyncStatesExecutor
    {
        private readonly IDataBaseFactory _dataBaseFactory;

        public RemoveSyncStatesExecutor(IDataBaseFactory dataBaseFactory)
        {
            _dataBaseFactory = dataBaseFactory;
        }

        public void Handler(string login, string[] filePaths)
        {
            using (var dataBase = _dataBaseFactory.Create())
            {
                var syncState = from b in dataBase.SyncStates
                    where b.Login == login && filePaths.Contains(b.FilePath)
                    select b;

                dataBase.SyncStates.RemoveRange(syncState);
                dataBase.ApplyChanges();
            }
        }
    }
}