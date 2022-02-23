using System.Linq;
using DataBaseWriterService._Interfaces_;
using ServicesApi.Common._Interfaces_;
using ServicesApi.Database.States;

namespace DataBaseWriterService.Actions.States
{
    public class RemoveSyncStatesByAvailableFolderAction  : IMessageHandler<RemoveSyncStatesByAvailableFolder>
    {
        private readonly IDataBaseFactory _dataBaseFactory;

        internal RemoveSyncStatesByAvailableFolderAction(IDataBaseFactory dataBaseFactory)
        {
            _dataBaseFactory = dataBaseFactory;
        }

        public void Handler(RemoveSyncStatesByAvailableFolder message)
        {
            using (var dataBase = _dataBaseFactory.Create())
            {
                    
                var syncState = from b in dataBase.SyncStates
                    where b.Login == message.Login && b.FilePath.StartsWith(message.AvailableFolder)
                    select b;

                dataBase.SyncStates.RemoveRange(syncState);
                dataBase.ApplyChanges();
            }
        }
    }
}