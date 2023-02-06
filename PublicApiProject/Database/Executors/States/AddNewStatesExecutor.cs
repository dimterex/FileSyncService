using System.Linq;
using Common.DatabaseProject._Interfaces_;
using Common.DatabaseProject.Dto;

namespace PublicProject.Database.Actions.States
{
    public class AddNewStatesExecutor
    {
        private readonly IDataBaseFactory _dataBaseFactory;

        public AddNewStatesExecutor(IDataBaseFactory dataBaseFactory)
        {
            _dataBaseFactory = dataBaseFactory;
        }

        public void Handler(string login, string[] filePaths)
        {
            using (var dataBase = _dataBaseFactory.Create())
            {
                foreach (var filePath in filePaths)
                {
                    AddSyncState(login, filePath, dataBase);
                }

                dataBase.ApplyChanges();
            }
        }

        private void AddSyncState(string login, string filePath, IDataBaseContext dataBase)
        {
            var syncStates = dataBase.SyncStates.ToList().Where(x => x.Login == login && x.FilePath == filePath)
                .ToList();

            if (syncStates.Any())
                return;

            var state = new SyncState
            {
                Login = login,
                FilePath = filePath
            };

            dataBase.SyncStates.Add(state);
        }
    }
}