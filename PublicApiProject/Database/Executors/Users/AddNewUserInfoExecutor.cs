using System.Collections.Generic;
using System.Linq;
using Common.DatabaseProject._Interfaces_;
using Common.DatabaseProject.Dto;

namespace PublicProject.Database.Actions.Users
{
    public class AddNewUserInfoExecutor
    {
        private readonly IDataBaseFactory _dataBaseFactory;

        public AddNewUserInfoExecutor(IDataBaseFactory dataBaseFactory)
        {
            _dataBaseFactory = dataBaseFactory;
        }

        public void Handler(string login, string password, string availableFolderPath)
        {
            using (var dataBase = _dataBaseFactory.Create())
            {
                var users = dataBase.Users.ToList().FirstOrDefault(x => x.Login == login);

                if (users != null)
                {
                    users.Password = password;
                    users.AvailableFolders.Add(availableFolderPath);
                }
                else
                {
                    dataBase.Users.Add(new User
                    {
                        Login = login,
                        Password = password,
                        AvailableFolders = new List<string> { availableFolderPath }
                    });
                }

                dataBase.ApplyChanges();
            }
        }
    }
}