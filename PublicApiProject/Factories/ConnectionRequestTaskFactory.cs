namespace PublicProject.Factories
{
    using _Interfaces_;
    using _Interfaces_.Factories;

    using Database.Actions.Users;

    using Logic;

    using Modules;

    public class ConnectionRequestTaskFactory : IConnectionRequestTaskFactory
    {
        private readonly IApiController _apiController;
        private readonly AvailableFoldersForUserRequestExecutor _availableFoldersForUserRequestExecutor;
        private readonly IConnectionStateManager _connectionStateManager;

        public ConnectionRequestTaskFactory(
            IConnectionStateManager connectionStateManager,
            IApiController apiController,
            AvailableFoldersForUserRequestExecutor availableFoldersForUserRequestExecutor)
        {
            _connectionStateManager = connectionStateManager;
            _apiController = apiController;
            _availableFoldersForUserRequestExecutor = availableFoldersForUserRequestExecutor;
        }

        public ConnectionRequestTask Create(string login, HttpRequestEventModel e)
        {
            return new ConnectionRequestTask(login, e, _connectionStateManager, _availableFoldersForUserRequestExecutor, _apiController);
        }
    }
}
