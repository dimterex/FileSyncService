using System.Linq;
using DataBaseWriterService._Interfaces_;
using ServicesApi.Common._Interfaces_;
using ServicesApi.Database.Users;

namespace DataBaseWriterService.Actions.Users
{
    public class RemoveUserInfoAction : IMessageHandler<RemoveUserInfo>
    {
        private readonly IDataBaseFactory _dataBaseFactory;

        internal RemoveUserInfoAction(IDataBaseFactory dataBaseFactory)
        {
            _dataBaseFactory = dataBaseFactory;
        }

        public void Handler(RemoveUserInfo message)
        {
            using (var dataBase = _dataBaseFactory.Create())
            {
                var users = dataBase.Users.ToList().FirstOrDefault(x => x.Login == message.Login);

                if (users != null)
                {
                    users.Password = message.Password;
                    users.AvailableFolders.Remove(message.AvailableFolderPath);
                  
                }

                dataBase.ApplyChanges();
            }
        }
    }
}