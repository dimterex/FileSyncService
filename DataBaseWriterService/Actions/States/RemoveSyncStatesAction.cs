using System.Linq;
using DataBaseWriterService._Interfaces_;
using ServicesApi.Common._Interfaces_;
using ServicesApi.Database.States;

namespace DataBaseWriterService.Actions.States
{
    public class RemoveSyncStatesAction : IMessageHandler<RemoveSyncStates>
    {
        private readonly IDataBaseFactory _dataBaseFactory;

        internal RemoveSyncStatesAction(IDataBaseFactory dataBaseFactory)
        {
            _dataBaseFactory = dataBaseFactory;
        }

        public void Handler(RemoveSyncStates message)
        {
            using (var dataBase = _dataBaseFactory.Create())
            {
                var syncState = from b in dataBase.SyncStates
                    where b.Login == message.Login && message.FilePaths.Contains(b.FilePath)
                    select b;

                dataBase.SyncStates.RemoveRange(syncState);
                dataBase.ApplyChanges();
            }
        }
    }
}