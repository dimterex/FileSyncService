using System.Collections.Generic;
using System.Linq;
using Common.DatabaseProject._Interfaces_;

namespace TelegramBotService.Database.Actions
{
    public class AvailableFoldersRequestExecutor
    {
        private readonly IDataBaseFactory _dataBaseFactory;

        public AvailableFoldersRequestExecutor(IDataBaseFactory dataBaseFactory)
        {
            _dataBaseFactory = dataBaseFactory;
        }

        public IList<string> Handler()
        {
            var result = new List<string>();

            using (var dataBase = _dataBaseFactory.Create())
            {
                var users = dataBase.Users.ToList();
                foreach (var user in users) result.AddRange(user.AvailableFolders);
            }

            return result;
        }
    }
}