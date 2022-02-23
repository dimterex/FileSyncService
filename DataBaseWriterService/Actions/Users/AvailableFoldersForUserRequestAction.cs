using System;
using System.Collections.Generic;
using System.Linq;
using DataBaseWriterService._Interfaces_;
using ServicesApi.Common._Interfaces_;
using ServicesApi.Database.Users;

namespace DataBaseWriterService.Actions.Users
{
    public class AvailableFoldersForUserRequestAction : IMessageHandlerWithResponse<AvailableFoldersForUserRequest>
    {
        private readonly IDataBaseFactory _dataBaseFactory;

        internal AvailableFoldersForUserRequestAction(IDataBaseFactory dataBaseFactory)
        {
            _dataBaseFactory = dataBaseFactory;
        }

        public void Handler(AvailableFoldersForUserRequest message, Action<IMessage> responseAction)
        {
            var result = new List<string>();

            using (var dataBase = _dataBaseFactory.Create())
            {
                var users = dataBase.Users.ToList().FirstOrDefault(x => x.Login == message.Login);
            
                if (users != null)
                    result.AddRange(users.AvailableFolders);
            }

            responseAction(new AvailableFoldersForUserResponse()
            {
                FilePaths = result.ToArray()
            });
        }
    }
}