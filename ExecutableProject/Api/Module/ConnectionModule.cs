using System;
using System.Linq;
using DataBaseProject;
using NLog;
using SdkProject.Api.Connection;
using SdkProject.Api.Sync;
using TransportProject;
using WebSocketSharp.Server;

namespace Service.Api.Module
{
    public class ConnectionModule : BaseApiModule
    {
        private readonly IConnectionStateManager _connectionStateManager;
        private readonly IUserTableDataBase _userTableDataBase;
        private readonly ILogger _logger;
        
        public ConnectionModule(IConnectionStateManager connectionStateManager,
            IUserTableDataBase userTableDataBase)
            : base("connection", new Version(0, 1))
        {
            _connectionStateManager = connectionStateManager;
            _userTableDataBase = userTableDataBase;
            _logger = LogManager.GetCurrentClassLogger();
        }

        protected override void OnInitialize()
        {
            RegisterPostRequestWithBody<ConnectionRequest>(OnConnectionRequest);
        }

        private void OnConnectionRequest(IClient client, SyncFilesRequest arg2, ConnectionRequest connectionRequest, HttpRequestEventArgs e)
        {
            _connectionStateManager.Add(connectionRequest.Login, client.ID);
            var folders =  _userTableDataBase.GetAvailableFolders(connectionRequest.Login);

            client.SendResponse(e, new ConnectionResponse()
            {
                Shared_folders = folders.ToArray(),
                Token = client.ID
            });
        }
    }
}