namespace PublicProject.Actions
{
    using Database.Actions.States;

    using ServicesApi.Common;
    using ServicesApi.Common._Interfaces_;
    using ServicesApi.Configuration;

    public class UpdateSyncStateAction : IMessageHandler<UpdateSyncStateRequest>
    {
        private readonly AddNewStatesExecutor _addNewStatesExecutor;

        public UpdateSyncStateAction(AddNewStatesExecutor addNewStatesExecutor)
        {
            _addNewStatesExecutor = addNewStatesExecutor;
        }
        public IMessage Handler(UpdateSyncStateRequest message)
        {
            _addNewStatesExecutor.Handler(message.Login, message.SyncFiles);
            return new StatusResponse
            {
                Message = "Success",
                Status = Status.Ok
            };
        }
    }
}
