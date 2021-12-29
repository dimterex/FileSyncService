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
                var users = dataBase.Users.ToList().FirstOrDefault(x => x.Login == login);
            
                if (users != null)
                    result.AddRange(users.AvailableFolders);
            }
            
            return result;
        }
        
        public IList<string> GetAvailableFolders()
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
            
            return result;
        }

        public void Add(string login, string password, string availableFolderPath)
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
                    dataBase.Users.Add(new User()
                    {
                        Login = login,
                        Password = password,
                        AvailableFolders = new List<string>() { availableFolderPath }
                    });
                }
                
                dataBase.ApplyChanges();
            }
        }

        public void Remove(string login, string password, string availableFolderPath)
        {
            using (var dataBase = _dataBaseFactory.Create())
            {
                var users = dataBase.Users.ToList().FirstOrDefault(x => x.Login == login);

                if (users != null)
                {
                    users.Password = password;
                    users.AvailableFolders.Remove(availableFolderPath);
                  
                }

                dataBase.ApplyChanges();
            }
        }
    }
}