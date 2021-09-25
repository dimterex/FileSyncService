using System;
using System.Linq;
using NLog;
using Service.Api.Message.Connection;
using Service.Api.Module.Common;
using Service.Transport;

namespace Service.Api.Module
{
    public class ConnectionModule : BaseApiModule
    {
        private readonly ConnectionStateManager _connectionStateManager;
        private readonly SettingsManager _settingsManager;
        private readonly ILogger _logger;
        
        public ConnectionModule(ConnectionStateManager connectionStateManager, SettingsManager settingsManager)
            : base("connection", new Version(0, 1))
        {
            _connectionStateManager = connectionStateManager;
            _settingsManager = settingsManager;
            _logger = LogManager.GetCurrentClassLogger();
        }

        protected override void OnInitialize()
        {
            RegisterMessage<ConnectionRequest>(OnConnectionRequest);
        }

        private void OnConnectionRequest(IClient client, ConnectionRequest connectionRequest)
        {
            _connectionStateManager.Add(connectionRequest.Login, client.ID);
            var folders = _settingsManager.GetFoldersForClient(connectionRequest.Login);
            
            client.SendMessage(new ConnectionResponse()
            {
                Shared_folders = folders.ToArray(),
                Token = client.ID
            });
        }
    }
}