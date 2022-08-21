using System.Collections.Generic;
using System.Linq;
using Common.DatabaseProject._Interfaces_;

namespace PublicProject.Database.Actions.Users
{
    public class AvailableFoldersForUserRequestExecutor
    {
        private readonly IDataBaseFactory _dataBaseFactory;

        public AvailableFoldersForUserRequestExecutor(IDataBaseFactory dataBaseFactory)
        {
            _dataBaseFactory = dataBaseFactory;
        }

        public IList<string> Handler(string login)
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
    }
}