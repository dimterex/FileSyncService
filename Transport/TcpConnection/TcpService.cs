namespace Service.Transport
{
    using NLog;

    using Service.Api;

    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    public class TcpService
    {
        private readonly ILogger _logger;

        private readonly object _lockObject;
        private readonly IPEndPoint _listenAddress;

        private static TcpListener _listener;

        #region Properties

        public ApiController ApiController { get; }

        #endregion Properties

        public TcpService(ApiController controller, IPEndPoint endPoint)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _lockObject = new object();
            ApiController = controller;
            _listenAddress = endPoint;

            _listener = null;
        }

        public void Start()
        {
            lock (_lockObject)
            {
                if (_listener != null)
                    return;

                _listener = new TcpListener(_listenAddress);
                _listener.Start();

                _logger.Info(@"
                        ===================================================
                        Started listening requests at: {0}:{1}
                        ===================================================",
                        _listenAddress.Address, _listenAddress.Port);

                var tt = new Task(() => {
                    TcpClient client;

                    // TODO: Добавить CancellationToken
                    while (true)
                    {
                        client = _listener.AcceptTcpClient();
                        ThreadPool.QueueUserWorkItem((x) => {
                            var newClient = new TcpCustomClient(client, this);
                            newClient.Start();
                        });
                    }
                });
                tt.Start();
            }
        }

        public void Stop()
        {
            lock (_lockObject)
            {
                _listener?.Stop();
                _listener = null;
            }
        }
    }
}
