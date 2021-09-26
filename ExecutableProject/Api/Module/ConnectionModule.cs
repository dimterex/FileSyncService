using System;
using System.Linq;
using DataBaseProject;
using NLog;
using SdkProject.Api.Connection;
using TransportProject;

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
            RegisterMessage<ConnectionRequest>(OnConnectionRequest);
        }

        private void OnConnectionRequest(IClient client, ConnectionRequest connectionRequest)
        {
            _connectionStateManager.Add(connectionRequest.Login, client.ID);
           
            
            var folders =  _userTableDataBase.GetAvailableFolders(connectionRequest.Login);
            
            client.SendMessage(new ConnectionResponse()
            {
                Shared_folders = folders.ToArray(),
                Token = client.ID
            });
        }
    }
}