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
using PublicProject._Interfaces_;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace PublicProject
{
    public class WsService
    {
        #region Constants

        private const int BIND_SOCKET_INTERVAL = 1000;


        #endregion Constants

        #region Fields

        private readonly IPAddress _listenAddress;
        private readonly int _httpPort;
        private readonly int _httpsPort;

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
            ApiController controller,
            int httpPort,
            int httpsPort)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _logger.Info("Initializing servers and static data.");

            Controller = controller;
            _listenAddress =  IPAddress.Any;
            _httpPort = httpPort;
            _httpsPort = httpsPort;

            _wsServer = null;
            _wssServer = null;
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
                // Неизвестный запрос:
                e.Response.StatusCode = (int)HttpStatusCode.NotFound;
                _logger.Warn(() => $"Can't route '{e.Request.RawUrl}' request to appropriate handler.");
            }
        }
       

        #endregion Methods
    }
}