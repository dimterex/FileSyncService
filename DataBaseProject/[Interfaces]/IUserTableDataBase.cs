using System.Collections.Generic;

namespace DataBaseProject
{
    public interface IUserTableDataBase
    {
        IList<string> GetAvailableFolders(string login);
        void AddOrUpdate(string login, string password, string[] availableFolders);
    }
}