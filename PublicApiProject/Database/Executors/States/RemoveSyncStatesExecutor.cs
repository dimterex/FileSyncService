namespace PublicProject.Database.Actions.States
{
    using System.Linq;

    using Common.DatabaseProject._Interfaces_;
    using Common.DatabaseProject.Dto;

    public class RemoveSyncStatesExecutor
    {
        private readonly IDataBaseFactory _dataBaseFactory;

        public RemoveSyncStatesExecutor(IDataBaseFactory dataBaseFactory)
        {
            _dataBaseFactory = dataBaseFactory;
        }

        public void Handler(string login, string[] filePaths)
        {
            using (IDataBaseContext dataBase = _dataBaseFactory.Create())
            {
                IQueryable<SyncState> syncState = from b in dataBase.SyncStates where b.Login == login && filePaths.Contains(b.FilePath) select b;

                dataBase.SyncStates.RemoveRange(syncState);
                dataBase.ApplyChanges();
            }
        }
    }
}
