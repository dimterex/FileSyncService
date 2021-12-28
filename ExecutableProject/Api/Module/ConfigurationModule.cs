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
            foreach (var file in request.SyncFiles)
            {
                _syncTableDataBase.AddState(request.Login, file);
            }
        }

        private void OnCreateUserRequest(SyncFilesRequest syncFilesRequest, CreateUserRequest request, HttpRequestEventArgs arg3)
        {
            _userTableDataBase.AddOrUpdate(request.Login, request.Password, request.AvailableFolders);
        }
    }
}