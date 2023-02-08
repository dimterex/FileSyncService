namespace TelegramBotService.Database.Actions
{
    using System.Collections.Generic;
    using System.Linq;

    using Common.DatabaseProject._Interfaces_;
    using Common.DatabaseProject.Dto;

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

            using (IDataBaseContext dataBase = _dataBaseFactory.Create())
            {
                List<User> users = dataBase.Users.ToList();
                foreach (User user in users)
                {
                    result.AddRange(user.AvailableFolders);
                }
            }

            return result;
        }
    }
}
