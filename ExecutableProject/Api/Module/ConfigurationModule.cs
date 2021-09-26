using System;
using DataBaseProject;
using SdkProject.Api.Confguration;
using TransportProject;

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
            RegisterMessage<CreateUserRequest>(OnCreateUserRequest);
            RegisterMessage<CreateSyncStateRequest>(OnCreateSyncStateRequest);
        }

        private void OnCreateSyncStateRequest(IClient client, CreateSyncStateRequest request)
        {
            foreach (var file in request.SyncFiles)
            {
                _syncTableDataBase.AddState(request.Login, file);
            }
        }

        private void OnCreateUserRequest(IClient client, CreateUserRequest request)
        {
            _userTableDataBase.AddOrUpdate(request.Login, request.Password, request.AvailableFolders);
        }
    }
}