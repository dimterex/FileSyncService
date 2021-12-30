using System;
using System.Collections.Generic;
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

namespace TransportProject
{
    public class WsService
    {
        #region Constants

        private const int BIND_SOCKET_INTERVAL = 1000;

        private const int CHECK_INTERVAL = 1000;

        private const string STATIC_FILE = "static.zip";

        private const string DEFAULT_FILE = "index.html";


        #endregion Constants

        #region Fields

        private readonly object _lockObject;
        private readonly IConnectionStateManager _connectionStateManager;
        private readonly IPAddress _listenAddress;
        private readonly int _httpPort;
        private readonly int _httpsPort;

        private static Dictionary<string, StaticFileEntry> _staticFiles;

        private HttpServer _wsServer;
        private HttpServer _wssServer;
        private readonly ILogger _logger;

        private Thread _socketThread;
        private bool _isActive;

        #endregion Fields

        #region Properties

        public ApiController Controller { get; }

        #endregion Properties

        #region Constructors

        public WsService(
            IConnectionStateManager connectionStateManager,
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

                _logger.Debug("Threads were stopped.");

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

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

        private void StartHttpsServer()
        {
            if (_wssServer != null)
                return;

            try
            {
                _wssServer = new HttpServer(_listenAddress, _httpsPort, true)
                {
                    AuthenticationSchemes = WebSocketSharp.Net.AuthenticationSchemes.Anonymous
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
            server.OnGet += HandleRequest;
            server.OnPost += HandleRequest;
            server.Start();
        }

        private void InitializeStatic()
        {
            var staticPath = Path.Combine("", STATIC_FILE);
            if (!File.Exists(staticPath))
                return;

            FileStream staticFile = null;
            ZipArchive archive = null;
            try
            {
                staticFile = new FileStream(staticPath, FileMode.Open);
                archive = new ZipArchive(staticFile, ZipArchiveMode.Read);

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
                _logger.Error(() => $"{e}");
            }
            finally
            {
                archive?.Dispose();
                staticFile?.Dispose();
            }
        }

        private void HandleRequest(object sender, HttpRequestEventArgs e)
        {
            _logger.Info(() => $"Handle request '{e.Request.RawUrl}'.");
            
            if (e.Request.Url.AbsolutePath.StartsWith(ApiController.API_RESOURCE_PATH))
            {
                e.Response.Headers.Add("Access-Control-Allow-Origin: *");
                Controller.HandleRequest(e.Request.Url.AbsolutePath.Substring(ApiController.API_RESOURCE_PATH.Length), e);
            }
            else
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