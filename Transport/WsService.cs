using Service.Api;

namespace Service.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Net;
    using System.Security.Authentication;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;

    using NLog;

    using WebSocketSharp;
    using WebSocketSharp.Server;

    using AuthenticationSchemes = WebSocketSharp.Net.AuthenticationSchemes;
    using HttpStatusCode = System.Net.HttpStatusCode;
    using Logger = NLog.Logger;

    public partial class WsService
    {
        #region Constants

        private const int BIND_SOCKET_INTERVAL = 1000;

        private const int CHECK_INTERVAL = 1000;

        private const string STATIC_FILE = "static.zip";

        private const string DEFAULT_FILE = "index.html";


        #endregion Constants

        #region Fields

        private readonly object _lockObject;
        private readonly ConnectionStateManager _connectionStateManager;
        private readonly IPAddress _listenAddress;
        private readonly int _httpPort;
        private readonly int _httpsPort;

        private readonly List<IClient> _connections;
        private readonly List<UnathorizedConnection> _unathorizedConnections;

        private static Dictionary<string, StaticFileEntry> _staticFiles;

        private HttpServer _wsServer;
        private HttpServer _wssServer;
        private readonly Timer _stateCheckTimer;
        private readonly Logger _logger;

        private Thread _socketThread;
        private bool _isActive;

        #endregion Fields

        #region Properties

        public ApiController Controller { get; }

        #endregion Properties

        #region Constructors

        public WsService(
            ConnectionStateManager connectionStateManager,
            ApiController controller,
            IPAddress address,
            int httpPort,
            int httpsPort)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _logger.Info("Initializing servers and static data.");

            _lockObject = new object();
            Controller = controller;
            _connectionStateManager = connectionStateManager;
            _listenAddress = address;
            _httpPort = httpPort;
            _httpsPort = httpsPort;

            _wsServer = null;
            _wssServer = null;
            _connections = new List<IClient>();
            _unathorizedConnections = new List<UnathorizedConnection>();
            _stateCheckTimer = new Timer(CheckState, null, -1, -1);

            _staticFiles = new Dictionary<string, StaticFileEntry>();

            InitializeStatic();
        }

        #endregion Constructors

        #region Methods

        public void Start()
        {
            if (_socketThread != null)
                return;

            _isActive = true;
            _socketThread = new Thread(BindSocketsWorker);
            _socketThread.Start();

            _stateCheckTimer.Change(CHECK_INTERVAL, CHECK_INTERVAL);
        }

        private void BindSocketsWorker()
        {
            _logger.Info(@"
                        ===================================================
                        Started listening requests at: {0}:{1}
                        ===================================================",
                _listenAddress, _httpPort);
            
            while (_isActive)
            {
                StartHttpServer();
                // StartHttpsServer();

                if (_wsServer != null && _wssServer != null)
                    break;

                Thread.Sleep(BIND_SOCKET_INTERVAL);
            }
            _logger.Info("Servers were started.");
        }

        public void Stop()
        {
            try
            {
                _logger.Debug("Threads are stopping.");

                _isActive = false;
                _socketThread?.Join();
                _socketThread = null;

                _wsServer?.Stop();
                _wsServer = null;

                _wssServer?.Stop();
                _wssServer = null;

                _stateCheckTimer.Change(-1, -1);

                _logger.Debug("Threads were stopped.");

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void Add(IClient wsClient)
        {
            lock (_lockObject)
            {
                _unathorizedConnections.Add(new UnathorizedConnection(wsClient));
            }
        }

        public void Authorize(IClient wsClient)
        {
            lock (_lockObject)
            {
                _unathorizedConnections.RemoveAll(a => a.Is(wsClient));
                _connections.Add(wsClient);
            }
        }

        public void Remove(IClient wsClient)
        {
            lock (_lockObject)
            {
                _connections.Remove(wsClient);
                _connectionStateManager.Remove(wsClient.ID);
            }
        }

        /// <summary>
        /// Инициализация сервера для незащищённых соединений.
        /// </summary>
        private void StartHttpServer()
        {
            if (_wsServer != null)
                return;

            try
            {
                _wsServer = new HttpServer(_listenAddress, _httpPort, false);
                InitHttpServer(_wsServer);
                _logger.Info(() => $"HttpServer server started: {_listenAddress}:{_httpPort}");
            }
            catch(Exception ex)
            {
                _wsServer?.Stop();
                _wsServer = null;
                _logger.Error(ex);
            }
        }

        /// <summary>
        /// Инициализация сервера для защищённых соединений.
        /// </summary>
        private void StartHttpsServer()
        {
            if (_wssServer != null)
                return;

            try
            {
                _wssServer = new HttpServer(_listenAddress, _httpsPort, true)
                {
                    AuthenticationSchemes = AuthenticationSchemes.Anonymous
                };

                _wssServer.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls12;
                InitHttpServer(_wssServer);
            }
            catch(Exception ex)
            {
                _wssServer?.Stop();
                _wssServer = null;
                _logger.Error(ex);
            }
        }

        private void InitHttpServer(HttpServer server)
        {
            // Подготовка сервера для работы с WS-протоколом:
            server.AddWebSocketService("/", () =>
            {
                var wsClient = new WsClient(this);
                Controller.AddClient(wsClient);
                return wsClient;
            });
            // Подготовка сервера для работы с REST-запросами:
            server.OnGet += HandleRequest;
            server.OnPost += HandleRequest;

            server.Start();
        }

  

        private void CheckState(object state)
        {
            if (!Monitor.TryEnter(_lockObject))
                return;

            try
            {
                var index = 0;
                while (index < _unathorizedConnections.Count)
                {
                    var unathorizedConnection = _unathorizedConnections[index];

                    if (unathorizedConnection.Check())
                    {
                        _unathorizedConnections.RemoveAt(index);
                        continue;
                    }

                    index += 1;
                }
            }
            finally
            {
                Monitor.Exit(_lockObject);
            }
        }

        private void InitializeStatic()
        {
            // TODO add configuration checks
            var staticPath = Path.Combine("", STATIC_FILE);
            if (!File.Exists(staticPath))
                return;

            FileStream staticFile = null;
            ZipArchive archive = null;
            try
            {
                staticFile = new FileStream(staticPath, FileMode.Open);
                archive = new ZipArchive(staticFile, ZipArchiveMode.Read);

                // TODO Add other types
                var headers = new Dictionary<string, StaticFileHeaders>
                {
                    [".css"] = new StaticFileHeaders(Encoding.UTF8, "text/css"),
                    [".html"] = new StaticFileHeaders(Encoding.UTF8, "text/html"),
                    [".ico"] = new StaticFileHeaders(null, "image/x-icon"),
                    [".png"] = new StaticFileHeaders(null, "image/png"),
                    [".js"] = new StaticFileHeaders(Encoding.UTF8, "application/javascript"),
                    [".json"] = new StaticFileHeaders(Encoding.UTF8, "application/json"),
                    [".map"] = new StaticFileHeaders(Encoding.UTF8, "application/json"),
                    [".woff"] = new StaticFileHeaders(null, "font/woff"),
                    [".woff2"] = new StaticFileHeaders(null, "font/woff2"),
                    [".wav"] = new StaticFileHeaders(null, "audio/x-wav"),
                };

                foreach (var entry in archive.Entries)
                {
                    Encoding encoding = null;
                    string contentType = null;

                    var extension = Path.GetExtension(entry.Name);
                    if (extension == null || !headers.TryGetValue(extension, out var header))
                        continue;

                    var path = '/' + entry.FullName;

                    byte[] data;
                    using (var stream = entry.Open())
                    {
                        data = new byte[entry.Length];
                        stream.Read(data, 0, (int)entry.Length);
                    }

                    var fileCacheInfo = new StaticFileEntry(data, header);
                    _staticFiles[path] = fileCacheInfo;
                    if (entry.Name == DEFAULT_FILE)
                        _staticFiles[path.Substring(0, path.Length - DEFAULT_FILE.Length)] = fileCacheInfo;
                }
            }
            catch (Exception e)
            {
                // TODO log exception
            }
            finally
            {
                archive?.Dispose();
                staticFile?.Dispose();
            }
        }


        /// <summary>
        /// Основной обработчик REST-запросов.
        /// </summary>
        private void HandleRequest(object sender, HttpRequestEventArgs e)
        {
            // Определение запроса:
            if (e.Request.Url.AbsolutePath.StartsWith(ApiController.API_RESOURCE_PATH)) // REST-запрос для API с проверкой токена.
            {
                e.Response.Headers.Add("Access-Control-Allow-Origin: *"); // Разрешить доступ к JSON с серверной машины.
                Controller.HandleRequest(e.Request.Url.AbsolutePath.Substring(ApiController.API_RESOURCE_PATH.Length), e);
            }
            else // Запрос не является специальным или запросом API, возможно ожидаются ресурсы.
            {
                if (e.Request.HttpMethod == "GET")
                {
                    var fileExtensionRegex = new Regex(@"(/.+\..+)$");
                    var hasExtension = fileExtensionRegex.IsMatch(e.Request.Url.LocalPath);

                    if (_staticFiles.TryGetValue(e.Request.Url.LocalPath, out var entry) || !hasExtension && _staticFiles.TryGetValue("/", out entry))
                    {
                        e.Response.ContentType = entry.Headers.ContentType;
                        e.Response.ContentEncoding = entry.Headers.Encoding;
                        e.Response.WriteContent(entry.Data);
                        return;
                    }
                }

                // Неизвестный запрос:
                e.Response.StatusCode = (int)HttpStatusCode.NotFound;
                _logger.Warn(() => $"Can't route '{e.Request.RawUrl}' request to appropriate handler.");
            }
        }
        
       

        #endregion Methods

        #region Classes

        private class StaticFileEntry
        {
            public byte[] Data { get; }

            public StaticFileHeaders Headers { get; }

            public StaticFileEntry(byte[] data, StaticFileHeaders headers)
            {
                Data = data;
                Headers = headers;
            }
        }

        private class StaticFileHeaders
        {
            public Encoding Encoding { get; }

            public string ContentType { get; }

            public StaticFileHeaders(Encoding encoding, string contentType)
            {
                Encoding = encoding;
                ContentType = contentType;
            }
        }

        #endregion Classes
    }
}
