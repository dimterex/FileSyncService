using System;
using DataBaseProject;
using SdkProject.Api.Confguration;
using SdkProject.Api.Sync;
using TransportProject;
using WebSocketSharp.Server;

namespace Service.Api.Module
{
    public class ConfigurationModule : BaseApiModule
    {
        private readonly IUserTableDataBase _userTableDataBase;
        private readonly ISyncTableDataBase _syncTableDataBase;

        public ConfigurationModule(IUserTableDataBase userTableDataBase,
            ISyncTableDataBase syncTableDataBase) : base("config", new Version(0, 1))
        {
            _userTableDataBase = userTableDataBase;
            _syncTableDataBase = syncTableDataBase;
        }
        
        protected override void OnInitialize()
        {
            RegisterPostRequestWithBody<CreateUserRequest>(OnCreateUserRequest);
            RegisterPostRequestWithBody<CreateSyncStateRequest>(OnCreateSyncStateRequest);
        }

        private void OnCreateSyncStateRequest(SyncFilesRequest syncFilesRequest, CreateSyncStateRequest request, HttpRequestEventArgs arg3)
        {
            _syncTableDataBase.AddStates(request.Login, request.SyncFiles);
        }

        private void OnCreateUserRequest(SyncFilesRequest syncFilesRequest, CreateUserRequest request, HttpRequestEventArgs arg3)
        {
            foreach (var availableFolder in request.AvailableFolders)
            {
                switch (availableFolder.AvailableFolderAction)
                {
                    case AvailableFolderAction.Add:
                        _userTableDataBase.Add(request.Login, request.Password, availableFolder.Path);
                        break;
                    case AvailableFolderAction.Remove:
                        _userTableDataBase.Remove(request.Login, request.Password, availableFolder.Path);
                        _syncTableDataBase.RemoveSyncStatesByAvailableFolder(request.Login, availableFolder.Path);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}