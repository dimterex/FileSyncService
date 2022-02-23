using System;
using System.Collections.Generic;
using System.Linq;
using DataBaseWriterService._Interfaces_;
using ServicesApi.Common._Interfaces_;
using ServicesApi.Database.States;

namespace DataBaseWriterService.Actions.States
{
    public class SyncStatesRequestAction : IMessageHandlerWithResponse<SyncStatesRequest>
    {
        private readonly IDataBaseFactory _dataBaseFactory;

        internal SyncStatesRequestAction(IDataBaseFactory dataBaseFactory)
        {
            _dataBaseFactory = dataBaseFactory;
        }

        public void Handler(SyncStatesRequest message, Action<IMessage> responseAction)
        {
            var result = new List<string>();
            using (var dataBase = _dataBaseFactory.Create())
            {
                var syncStates = dataBase.SyncStates.ToList().Where(x => x.Login == message.Login);
                foreach (var syncState in syncStates)
                {
                    result.Add(syncState.FilePath);
                }
            }

            responseAction?.Invoke(new SyncStatesResponse()
            {
                FilePaths = result.ToArray()
            });
        }
    }
}