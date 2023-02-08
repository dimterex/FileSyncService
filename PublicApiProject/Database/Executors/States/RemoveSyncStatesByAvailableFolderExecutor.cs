namespace PublicProject.Database.Actions.States
{
    using System.Linq;

    using Common.DatabaseProject._Interfaces_;
    using Common.DatabaseProject.Dto;

    public class RemoveSyncStatesByAvailableFolderExecutor
    {
        private readonly IDataBaseFactory _dataBaseFactory;

        public RemoveSyncStatesByAvailableFolderExecutor(IDataBaseFactory dataBaseFactory)
        {
            _dataBaseFactory = dataBaseFactory;
        }

        public void Handler(string login, string availableFolder)
        {
            using (IDataBaseContext dataBase = _dataBaseFactory.Create())
            {
                IQueryable<SyncState> syncState = from b in dataBase.SyncStates
                                                  where b.Login == login && b.FilePath.StartsWith(availableFolder)
                                                  select b;

                dataBase.SyncStates.RemoveRange(syncState);
                dataBase.ApplyChanges();
            }
        }
    }
}
