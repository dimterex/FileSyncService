namespace PublicProject._Interfaces_
{
    using System.Collections.Generic;

    using Common.DatabaseProject.Dto;

    public interface IHistoryService
    {
        void AddNewEvent(string login, string filepath, string action);

        IList<HistoryDto> GetEvents();
    }
}
