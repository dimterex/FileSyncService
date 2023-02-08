namespace PublicProject.Database.Actions.States
{
    using System.Collections.Generic;
    using System.Linq;

    using Common.DatabaseProject._Interfaces_;
    using Common.DatabaseProject.Dto;

    public class SyncStatesRequestExecutor
    {
        private readonly IDataBaseFactory _dataBaseFactory;

        public SyncStatesRequestExecutor(IDataBaseFactory dataBaseFactory)
        {
            _dataBaseFactory = dataBaseFactory;
        }

        public IList<string> Handler(string login)
        {
            var result = new List<string>();
            using (IDataBaseContext dataBase = _dataBaseFactory.Create())
            {
                IEnumerable<SyncState> syncStates = dataBase.SyncStates.ToList().Where(x => x.Login == login);
                foreach (SyncState syncState in syncStates)
                {
                    result.Add(syncState.FilePath);
                }
            }

            return result;
        }
    }
}
