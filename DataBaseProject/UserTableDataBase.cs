using System.Collections.Generic;
using System.Linq;
using DataBaseProject.Dto;

namespace DataBaseProject
{
    public class UserTableDataBase : IUserTableDataBase
    {
        private readonly IDataBaseFactory _dataBaseFactory;

        public UserTableDataBase()
        {
            _dataBaseFactory = new DataBaseFactory();
        }

        public IList<string> GetAvailableFolders(string login)
        {
            var result = new List<string>();

            using (var dataBase = _dataBaseFactory.Create())
            {
                // получаем объекты из бд и выводим на консоль
                var users = dataBase.Users.ToList().FirstOrDefault(x => x.Login == login);
            
                if (users != null)
                    result.AddRange(users.AvailableFolders);
            }
            
            return result;
        }

        public void AddOrUpdate(string login, string password, string[] availableFolders)
        {
            using (var dataBase = _dataBaseFactory.Create())
            {
                // получаем объекты из бд и выводим на консоль
                var users = dataBase.Users.ToList().FirstOrDefault(x => x.Login == login);

                if (users != null)
                {
                    users.Password = password;
                    users.AvailableFolders.Clear();
                    users.AvailableFolders.AddRange(availableFolders);
                }

                else
                {
                    dataBase.Users.Add(new User()
                    {
                        Login = login,
                        Password = password,
                        AvailableFolders = new List<string>(availableFolders)
                    });
                }
                
                dataBase.ApplyChanges();
            }
        }
    }
}