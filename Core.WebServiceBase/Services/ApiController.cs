namespace Core.WebServiceBase.Services;

using System.Net;
using System.Text;

using _Interfaces_;

using Models;

using Newtonsoft.Json;

using NLog;

using SdkProject;
using SdkProject._Interfaces_;
using SdkProject.Api;

public class ApiController : IApiController
    {
        public const string API_RESOURCE_PATH = "/api";
        private const string TAG = nameof(ApiController);
        private readonly ILogger _logger;

        private readonly Dictionary<string, BaseApiModule> _requestToModule;
        private readonly SdkPacketSerializer _sdkPacketSerializer;

        public ApiController()
        {
            _logger = LogManager.GetLogger(TAG);
            _requestToModule = new Dictionary<string, BaseApiModule>();
            _sdkPacketSerializer = new SdkPacketSerializer();
        }

        public void HandleRequest(string resource, HttpRequestEventModel e)
        {
            try
            {
                if (string.IsNullOrEmpty(resource) || !_requestToModule.TryGetValue(resource, out BaseApiModule service))
                {
                    _logger.Warn(() => $"Can't route '{e.Request.RawUrl}' request to appropriate handler.");
                    e.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }

                _logger.Info(() => $"Success route '{e.Request.RawUrl}' request to appropriate handler.");

                service.Handle(resource, e);
            }
            catch (Exception ex)
            {
                _logger.Error(() => $"Handle Request Error {ex.Message}");
                e.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }

        public void SetErrorResponse(HttpRequestEventModel e)
        {
            e.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        public void RegisterRequest(string resource, BaseApiModule module)
        {
            _requestToModule.Add($"/{module.Name}/{resource}", module);
        }

        public void SendResponse(HttpRequestEventModel e, ISdkMessage response)
        {
            e.Response.SendChunked = true;

            SdkMessageContainer rabbitMqMessageContainer = _sdkPacketSerializer.Serialize(response);

            using (var streamWriter = new StreamWriter(e.Response.OutputStream, Encoding.UTF8))
            {
                var serializer = JsonSerializer.Create(
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                var jsonWriter = new JsonTextWriter(streamWriter);

                try
                {
                    _logger.Debug(() => $"Send response: {rabbitMqMessageContainer.Identifier}");
                    serializer.Serialize(jsonWriter, new[] { rabbitMqMessageContainer });
                }
                finally
                {
                    jsonWriter.Close();
                }
            }
        }
    }
