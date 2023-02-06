using System.Collections.Generic;
using Common.DatabaseProject.Dto;

namespace PublicProject._Interfaces_
{
    public interface IHistoryService
    {
        void AddNewEvent(string login, string filepath, string action);

        IList<HistoryDto> GetEvents();
    }
}