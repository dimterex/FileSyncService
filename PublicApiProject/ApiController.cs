using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using NLog;
using PublicProject._Interfaces_;
using PublicProject.Modules;
using SdkProject;
using SdkProject._Interfaces_;

namespace PublicProject
{
    public class ApiController : IApiController
    {
        public const string API_RESOURCE_PATH = "/api";
        private const string TAG = nameof(ApiController);

        private readonly Dictionary<string, BaseApiModule> _requestToModule;
        private readonly SdkPacketSerializer _sdkPacketSerializer;
        private readonly ILogger _logger;


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
                var token = e.Request.QueryString["token"];

                if (string.IsNullOrEmpty(resource) || !_requestToModule.TryGetValue(resource, out var service))
                {
                    _logger.Warn(() => $"Can't route '{e.Request.RawUrl}' request to appropriate handler.");
                    e.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }

                _logger.Info(() => $"Can route '{e.Request.RawUrl}' request to appropriate handler.");

                service.Handle(resource.Substring(resource.IndexOf('/', 1) + 1), e);
            }
            catch (Exception ex)
            {
                _logger.Error(() => $"Handle Request Error {ex.Message}");

                try
                {
                    e.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
                catch (InvalidOperationException)
                {
                }
            }
        }

        public void SetErrorResponse(HttpRequestEventModel e)
        {
            e.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        public void RegisterRequest(string resource, BaseApiModule module)
        {
            _requestToModule.Add($"/{module.Name}/{module.Version.Major}/{resource}", module);
        }

        public void SendResponse(HttpRequestEventModel e, ISdkMessage response)
        {
            e.Response.SendChunked = true;

            var rabbitMqMessageContainer = _sdkPacketSerializer.Serialize(response);

            using (var streamWriter = new StreamWriter(e.Response.OutputStream, Encoding.UTF8))
            {
                var serializer = JsonSerializer.Create(new JsonSerializerSettings
                    { NullValueHandling = NullValueHandling.Ignore });
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
}