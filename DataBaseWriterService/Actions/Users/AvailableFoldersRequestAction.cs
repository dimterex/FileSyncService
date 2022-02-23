using System;
using System.Collections.Generic;
using System.Linq;
using DataBaseWriterService._Interfaces_;
using ServicesApi.Common._Interfaces_;
using ServicesApi.Database.Users;

namespace DataBaseWriterService.Actions.Users
{
    public class AvailableFoldersRequestAction : IMessageHandlerWithResponse<AvailableFoldersRequest>
    {
        private readonly IDataBaseFactory _dataBaseFactory;

        internal AvailableFoldersRequestAction(IDataBaseFactory dataBaseFactory)
        {
            _dataBaseFactory = dataBaseFactory;
        }

        public void Handler(AvailableFoldersRequest message, Action<IMessage> responseAction)
        {
            var result = new List<string>();

            using (var dataBase = _dataBaseFactory.Create())
            {
                var users = dataBase.Users.ToList();
                foreach (var user in users)
                {
                    result.AddRange(user.AvailableFolders);
                }
            }
            
            responseAction?.Invoke(new AvailableFoldersResponse()
            {
                FilePaths = result.ToArray()
            });
        }
    }
}