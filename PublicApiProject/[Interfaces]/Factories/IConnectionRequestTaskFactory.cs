namespace PublicProject._Interfaces_.Factories
{
    using Logic;

    using Modules;

    public interface IConnectionRequestTaskFactory
    {
        ConnectionRequestTask Create(string login, HttpRequestEventModel e);
    }
}
