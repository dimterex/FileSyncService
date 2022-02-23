using System.Linq;
using DataBaseWriterService._Interfaces_;
using DataBaseWriterService.Dto;
using ServicesApi.Common._Interfaces_;
using ServicesApi.Database.States;

namespace DataBaseWriterService.Actions.States
{
    public class AddNewStatesAction : IMessageHandler<AddNewStates>
    {
        private readonly IDataBaseFactory _dataBaseFactory;

        internal AddNewStatesAction(IDataBaseFactory dataBaseFactory)
        {
            _dataBaseFactory = dataBaseFactory;
        }
        
        public void Handler(AddNewStates message)
        {
            using (var dataBase = _dataBaseFactory.Create())
            {
                foreach (var filePath in message.FilePaths)
                {
                    AddSyncState(message.Login, filePath, dataBase);
                }
                    
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