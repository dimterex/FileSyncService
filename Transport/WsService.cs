namespace Service.Transport
{
    using NLog;

    using Service.Api;

    using System.Collections.Generic;
    using System.Net;

    using WebSocketSharp.Server;

    public class WsService
    {
        #region Fields

        private readonly ILogger _logger;

        private readonly object _lockObject;
        private readonly IPEndPoint _listenAddress;

        private readonly List<WsClient> _connections;

        private HttpServer _wsServer;

        #endregion Fields

        #region Properties

        public ApiController ApiController { get; }

        #endregion Properties

        #region Constructors

        public WsService(ApiController controller, IPEndPoint endPoint)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _lockObject = new object();
            ApiController = controller;
            _listenAddress = endPoint;

            _wsServer = null;
            _connections = new List<WsClient>();
        }

        #endregion Constructors

        #region Methods

        public void Start()
        {
            lock (_lockObject)
            {
                if (_wsServer != null)
                    return;

                _wsServer = new HttpServer(_listenAddress.Address, _listenAddress.Port);
                _wsServer.AddWebSocketService("/", () => new WsClient(this));

                _wsServer.Start();

                _logger.Info(@"
                        ===================================================
                        Started listening requests at: {0}:{1}
                        ===================================================",
                        _listenAddress.Address, _listenAddress.Port);
            }
        }

        public void Stop()
        {
            lock (_lockObject)
            {
                _wsServer?.Stop();
                _wsServer = null;
            }
        }

        public void Authorize(WsClient wsClient)
        {
            lock (_lockObject)
            {
                _connections.Add(wsClient);
            }
        }

        public void Remove(WsClient wsClient)
        {
            lock (_lockObject)
            {
                _connections.Remove(wsClient);
            }
        }

        #endregion Methods
    }
}
