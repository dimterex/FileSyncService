using System.Collections.Generic;
using System.Linq;
using Common.DatabaseProject._Interfaces_;

namespace PublicProject.Database.Actions.States
{
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
    }
}