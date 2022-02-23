using System.Linq;
using DataBaseWriterService._Interfaces_;
using DataBaseWriterService.Dto;
using ServicesApi.Common._Interfaces_;
using ServicesApi.Database.States;

namespace DataBaseWriterService.Actions.States
{
    public class AddNewStateAction : IMessageHandler<AddNewState>
    {
        private readonly IDataBaseFactory _dataBaseFactory;

        internal AddNewStateAction(IDataBaseFactory dataBaseFactory)
        {
            _dataBaseFactory = dataBaseFactory;
        }
        
        public void Handler(AddNewState message)
        {
            using (var dataBase = _dataBaseFactory.Create())
            {
                AddSyncState(message.Login, message.FilePath, dataBase);
                dataBase.ApplyChanges();
            }
        }
        
        private void AddSyncState(string login, string filePath, IDataBaseContext dataBase)
        {
            var syncStates = dataBase.SyncStates.ToList().Where(x => x.Login == login && x.FilePath == filePath).ToList();

            if (syncStates.Any())
                return;

            var state = new SyncState()
            {
                Login = login,
                FilePath = filePath
            };

            dataBase.SyncStates.Add(state);
        }
    }
}