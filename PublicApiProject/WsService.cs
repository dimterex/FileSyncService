using System;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using Core.Logger;
using Core.Logger._Enums_;
using Core.Logger._Interfaces_;
using PublicProject.Modules;
using WebSocketSharp.Server;
using AuthenticationSchemes = WebSocketSharp.Net.AuthenticationSchemes;

namespace PublicProject
{
    public class WsService
    {
        private readonly ILoggerService _loggerService;

        #region Constructors

        public WsService(ApiController controller, ILoggerService loggerService)
        {
            _loggerService = loggerService;
            _loggerService.SendLog(LogLevel.Info, TAG, () => "Initializing servers and static data.");

            Controller = controller;
            _listenAddress = IPAddress.Any;

            _wsServer = null;
            _wssServer = null;
        }

        #endregion Constructors

        #region Properties

        public ApiController Controller { get; }

        #endregion Properties

        #region Constants

        private const int BIND_SOCKET_INTERVAL = 1000;
        private const string TAG = nameof(WsService);

        #endregion Constants

        #region Fields

        private readonly IPAddress _listenAddress;
        private int _httpPort;
        private int _httpsPort;

        private HttpServer _wsServer;
        private HttpListener listener;

        private HttpServer _wssServer;

        private Thread _socketThread;
        private bool _isActive;

        #endregion Fields

        #region Methods

        public void Start(int httpPort, int httpsPort)
        {
            if (_socketThread != null)
                return;

            _httpPort = httpPort;
            _httpsPort = httpsPort;
            _isActive = true;
            _socketThread = new Thread(BindSocketsWorker);
            _socketThread.Start();
        }

        private void BindSocketsWorker()
        {
            _loggerService.SendLog(LogLevel.Info, TAG, () =>
            {
                var sb = new StringBuilder();
                sb.Append("=================================================");
                sb.Append($"Started listening requests at: {_listenAddress}:{_httpPort}");
                sb.Append("=================================================");
                return sb.ToString();
            });

            while (_isActive)
            {
                StartHttpServer();
                // StartHttpsServer();


                if (_wsServer != null && _wssServer != null)
                    break;

                Thread.Sleep(BIND_SOCKET_INTERVAL);
            }

            _loggerService.SendLog(LogLevel.Info, TAG, () => "Servers were started.");
        }

        private void StartHttpServer()
        {
            if (_wsServer != null)
                return;

            try
            {
                _wsServer = new HttpServer(_listenAddress, _httpPort, false);
                InitHttpServer(_wsServer);
                _loggerService.SendLog(LogLevel.Info, TAG,
                    () => $"HttpServer server started: {_listenAddress}:{_httpPort}");
            }
            catch (Exception ex)
            {
                _wsServer?.Stop();
                _wsServer = null;
                _loggerService.SendLog(LogLevel.Error, TAG, () => ex.ToString());
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
                    AuthenticationSchemes = AuthenticationSchemes.Anonymous
                };

                _wssServer.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls12;
                InitHttpServer(_wssServer);
            }
            catch (Exception ex)
            {
                _wssServer?.Stop();
                _wssServer = null;
                _loggerService.SendLog(LogLevel.Error, TAG, () => ex.ToString());
            }
        }

        private void InitHttpServer(HttpServer server)
        {
            server.WaitTime = TimeSpan.FromSeconds(100);
            server.OnGet += (server, e) => { HandleRequest(new HttpRequestEventModel(e.Request, e.Response)); };
            server.OnPost += (server, e) => { HandleRequest(new HttpRequestEventModel(e.Request, e.Response)); };
            ;
            server.Start();
        }

        private void HandleRequest(HttpRequestEventModel model)
        {
            _loggerService.SendLog(LogLevel.Info, TAG, () => $"Handle request '{model.Request.RawUrl}'.");

            if (model.Request.Url.AbsolutePath.StartsWith(ApiController.API_RESOURCE_PATH))
            {
                model.Response.Headers.Add("Access-Control-Allow-Origin: *");
                Controller.HandleRequest(
                    model.Request.Url.AbsolutePath.Substring(ApiController.API_RESOURCE_PATH.Length), model);
            }
            else
            {
                // Неизвестный запрос:
                model.Response.StatusCode = (int)HttpStatusCode.NotFound;
                _loggerService.SendLog(LogLevel.Warning, TAG,
                    () => $"Can't route '{model.Request.RawUrl}' request to appropriate handler.");
            }
        }

        #endregion Methods
    }
}