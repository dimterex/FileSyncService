using System;
using PublicProject.Database.Actions.States;
using PublicProject.Database.Actions.Users;
using SdkProject.Api.Confguration;
using SdkProject.Api.Sync;

namespace PublicProject.Modules
{
    public class ConfigurationModule : BaseApiModule
    {
        private readonly AddNewStatesExecutor _addNewStatesExecutor;
        private readonly AddNewUserInfoExecutor _addNewUserInfoExecutor;
        private readonly RemoveSyncStatesByAvailableFolderExecutor _removeSyncStatesByAvailableFolderExecutor;
        private readonly RemoveUserInfoExecutor _removeUserInfoExecutor;

        public ConfigurationModule(ApiController apiController,
            AddNewUserInfoExecutor addNewUserInfoExecutor,
            RemoveUserInfoExecutor removeUserInfoExecutor,
            RemoveSyncStatesByAvailableFolderExecutor removeSyncStatesByAvailableFolderExecutor,
            AddNewStatesExecutor addNewStatesExecutor)
            : base("config", new Version(0, 1), apiController)
        {
            _addNewUserInfoExecutor = addNewUserInfoExecutor;
            _removeUserInfoExecutor = removeUserInfoExecutor;
            _removeSyncStatesByAvailableFolderExecutor = removeSyncStatesByAvailableFolderExecutor;
            _addNewStatesExecutor = addNewStatesExecutor;
        }

        protected override void OnInitialize()
        {
            RegisterPostRequestWithBody<CreateUserRequest>(OnCreateUserRequest);
            RegisterPostRequestWithBody<CreateSyncStateRequest>(OnCreateSyncStateRequest);
        }

        private void OnCreateSyncStateRequest(SyncFilesRequest syncFilesRequest, CreateSyncStateRequest request,
            HttpRequestEventModel arg3)
        {
            _addNewStatesExecutor.Handler(request.Login, request.SyncFiles);
        }

        private void OnCreateUserRequest(SyncFilesRequest syncFilesRequest, CreateUserRequest request,
            HttpRequestEventModel arg3)
        {
            foreach (var availableFolder in request.AvailableFolders)
                switch (availableFolder.AvailableFolderAction)
                {
                    case AvailableFolderAction.Add:
                        _addNewUserInfoExecutor.Handler(request.Login, request.Password, availableFolder.Path);
                        break;
                    case AvailableFolderAction.Remove:
                        _removeUserInfoExecutor.Handler(request.Login, request.Password, availableFolder.Path);
                        _removeSyncStatesByAvailableFolderExecutor.Handler(request.Login, availableFolder.Path);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
        }
    }
}