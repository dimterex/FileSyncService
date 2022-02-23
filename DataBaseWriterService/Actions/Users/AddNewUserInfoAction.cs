using System.Collections.Generic;
using System.Linq;
using DataBaseWriterService._Interfaces_;
using DataBaseWriterService.Dto;
using ServicesApi.Common._Interfaces_;
using ServicesApi.Database.Users;

namespace DataBaseWriterService.Actions.Users
{
    public class AddNewUserInfoAction : IMessageHandler<AddNewUserInfo>
    {
        private readonly IDataBaseFactory _dataBaseFactory;

        internal AddNewUserInfoAction(IDataBaseFactory dataBaseFactory)
        {
            _dataBaseFactory = dataBaseFactory;
        }

        public void Handler(AddNewUserInfo message)
        {
            using (var dataBase = _dataBaseFactory.Create())
            {
                var users = dataBase.Users.ToList().FirstOrDefault(x => x.Login == message.Login);

                if (users != null)
                {
                    users.Password = message.Password;
                    users.AvailableFolders.Add(message.AvailableFolderPath);
                }
                else
                {
                    dataBase.Users.Add(new User()
                    {
                        Login = message.Login,
                        Password = message.Password,
                        AvailableFolders = new List<string>() { message.AvailableFolderPath }
                    });
                }
                
                dataBase.ApplyChanges();
            }
        }
    }
}