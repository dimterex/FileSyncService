using PublicProject._Interfaces_;
using PublicProject._Interfaces_.Factories;
using PublicProject.Database.Actions.Users;
using PublicProject.Logic;
using PublicProject.Modules;

namespace PublicProject.Factories
{
    public class ConnectionRequestTaskFactory : IConnectionRequestTaskFactory
    {
        private readonly ApiController _apiController;
        private readonly AvailableFoldersForUserRequestExecutor _availableFoldersForUserRequestExecutor;
        private readonly IConnectionStateManager _connectionStateManager;

        public ConnectionRequestTaskFactory(IConnectionStateManager connectionStateManager,
            ApiController apiController,
            AvailableFoldersForUserRequestExecutor availableFoldersForUserRequestExecutor)
        {
            _connectionStateManager = connectionStateManager;
            _apiController = apiController;
            _availableFoldersForUserRequestExecutor = availableFoldersForUserRequestExecutor;
        }

        public ConnectionRequestTask Create(string login, HttpRequestEventModel e)
        {
            return new ConnectionRequestTask(login, e, _connectionStateManager, _availableFoldersForUserRequestExecutor,
                _apiController);
        }
    }
}