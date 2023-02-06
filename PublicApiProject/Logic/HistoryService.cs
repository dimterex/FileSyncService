using System;
using System.Collections.Generic;
using System.Linq;
using Common.DatabaseProject._Interfaces_;
using Common.DatabaseProject.Dto;
using PublicProject._Interfaces_;

namespace PublicProject.Logic
{
    public class HistoryService : IHistoryService
    {
        private readonly IDataBaseFactory _dataBaseFactory;

        public HistoryService(IDataBaseFactory dataBaseFactory)
        {
            _dataBaseFactory = dataBaseFactory;
        }

        public void AddNewEvent(string login, string filepath, string action)
        {
            using (var dataBase = _dataBaseFactory.Create())
            {
                dataBase.History.Add(new HistoryDto()
                {
                    Login = login,
                    File = filepath,
                    Action = action,
                    TimeStamp = DateTime.UtcNow.ToString(),
                });
                dataBase.ApplyChanges();
            }
        }

        public IList<HistoryDto> GetEvents()
        {
            using (var dataBase = _dataBaseFactory.Create())
            {
                return dataBase.History.ToList();
            }
        }
    }
}