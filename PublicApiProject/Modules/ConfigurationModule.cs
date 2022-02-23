using System;
using System.Collections.Generic;
using System.Linq;
using Core.Publisher;
using SdkProject.Api.Confguration;
using SdkProject.Api.Sync;
using ServicesApi.Database.States;
using ServicesApi.Database.Users;
using WebSocketSharp.Server;

namespace PublicProject.Modules
{
    public class ConfigurationModule : BaseApiModule
    {
        private readonly PublisherController _publisherController;
        
        public ConfigurationModule(PublisherController publisherController) : base("config", new Version(0, 1))
        {
            _publisherController = publisherController;
        }
        
        protected override void OnInitialize()
        {
            RegisterPostRequestWithBody<CreateUserRequest>(OnCreateUserRequest);
            RegisterPostRequestWithBody<CreateSyncStateRequest>(OnCreateSyncStateRequest);
        }

        private void OnCreateSyncStateRequest(SyncFilesRequest syncFilesRequest, CreateSyncStateRequest request, HttpRequestEventArgs arg3)
        {
            _publisherController.Send(new AddNewStates()
            {
                Login = request.Login,
                FilePaths = request.SyncFiles,
            });
        }
        

        private void OnCreateUserRequest(SyncFilesRequest syncFilesRequest, CreateUserRequest request, HttpRequestEventArgs arg3)
        {
            foreach (var availableFolder in request.AvailableFolders)
            {
                switch (availableFolder.AvailableFolderAction)
                {
                    case AvailableFolderAction.Add:
                        _publisherController.Send(new AddNewUserInfo()
                        {
                            Login = request.Login,
                            Password = request.Password,
                            AvailableFolderPath = availableFolder.Path,
                        });
                        
                        break;
                    case AvailableFolderAction.Remove:
                        _publisherController.Send(new RemoveUserInfo()
                        {
                            Login = request.Login,
                            Password = request.Password,
                            AvailableFolderPath = availableFolder.Path,
                        });
                        
                        _publisherController.Send(new RemoveSyncStatesByAvailableFolder()
                        {
                            Login = request.Login,
                            AvailableFolder = availableFolder.Path
                        });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}