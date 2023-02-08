namespace PublicProject.Actions
{
    using Database.Actions.States;
    using Database.Actions.Users;

    using ServicesApi.Common;
    using ServicesApi.Common._Interfaces_;
    using ServicesApi.Configuration;

    public class CreateUserAction : IMessageHandler<CreateUserRequest>
    {
        private readonly AddNewUserInfoExecutor _addNewUserInfoExecutor;
        private readonly RemoveSyncStatesByAvailableFolderExecutor _removeSyncStatesByAvailableFolderExecutor;
        private readonly RemoveUserInfoExecutor _removeUserInfoExecutor;

        public CreateUserAction(
            AddNewUserInfoExecutor addNewUserInfoExecutor,
            RemoveUserInfoExecutor removeUserInfoExecutor,
            RemoveSyncStatesByAvailableFolderExecutor removeSyncStatesByAvailableFolderExecutor)
        {
            _addNewUserInfoExecutor = addNewUserInfoExecutor;
            _removeUserInfoExecutor = removeUserInfoExecutor;
            _removeSyncStatesByAvailableFolderExecutor = removeSyncStatesByAvailableFolderExecutor;
        }

        public IMessage Handler(CreateUserRequest message)
        {
            foreach (AvailableFolder availableFolder in message.AvailableFolders)
            {
                switch (availableFolder.AvailableFolderAction)
                {
                    case AvailableFolderAction.Add:
                        _addNewUserInfoExecutor.Handler(message.Login, message.Password, availableFolder.Path);
                        break;
                    case AvailableFolderAction.Remove:
                        _removeUserInfoExecutor.Handler(message.Login, message.Password, availableFolder.Path);
                        _removeSyncStatesByAvailableFolderExecutor.Handler(message.Login, availableFolder.Path);
                        break;
                    default:
                        return new StatusResponse
                        {
                            Message = $"Not found {availableFolder.AvailableFolderAction}",
                            Status = Status.Error
                        };
                }
            }

            return new StatusResponse
            {
                Message = "Success",
                Status = Status.Ok
            };
        }
    }
}
