namespace PublicProject._Interfaces_.Factories
{
    using Core.WebServiceBase.Models;

    using Logic;

    public interface IConnectionRequestTaskFactory
    {
        ConnectionRequestTask Create(string login, HttpRequestEventModel e);
    }
}
