namespace PublicProject.Database.Actions.Users
{
    using System.Linq;

    using Common.DatabaseProject._Interfaces_;
    using Common.DatabaseProject.Dto;

    public class RemoveUserInfoExecutor
    {
        private readonly IDataBaseFactory _dataBaseFactory;

        public RemoveUserInfoExecutor(IDataBaseFactory dataBaseFactory)
        {
            _dataBaseFactory = dataBaseFactory;
        }

        public void Handler(string login, string password, string availableFolderPath)
        {
            using (IDataBaseContext dataBase = _dataBaseFactory.Create())
            {
                User users = dataBase.Users.ToList().FirstOrDefault(x => x.Login == login);

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
