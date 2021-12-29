using System.Collections.Generic;

namespace DataBaseProject
{
    public interface IUserTableDataBase
    {
        IList<string> GetAvailableFolders(string login);
        void Add(string login, string password, string availableFolderPath);
        void Remove(string login, string password, string availableFolderPath);
    }
}