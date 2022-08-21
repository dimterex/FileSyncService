using PublicProject.Logic;
using PublicProject.Modules;

namespace PublicProject._Interfaces_.Factories
{
    public interface IConnectionRequestTaskFactory
    {
        ConnectionRequestTask Create(string login, HttpRequestEventModel e);
    }
}