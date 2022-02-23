using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using NLog;
using SdkProject;
using SdkProject._Interfaces_;
using SdkProject.Api;
using ServicesApi;
using ServicesApi.Common;
using ServicesApi.Common._Interfaces_;
using WebSocketSharp.Server;

namespace PublicProject
{
    public class ApiController
    {
        public const string API_RESOURCE_PATH = "/api";
        
        private readonly RabbitMqPacketSerializer _package;
        private static ILogger _logger;

        
        private readonly Dictionary<string, BaseApiModule> _requestToModule;
        private readonly SdkPacketSerializer _sdkPacketSerializer;


        public ApiController()
        {
            _requestToModule = new Dictionary<string, BaseApiModule>();
            _sdkPacketSerializer = new SdkPacketSerializer();
            _logger = LogManager.GetCurrentClassLogger();
        }
        
        public void HandleRequest(string resource, HttpRequestEventArgs e)
        {
            try
            {
                string token = e.Request.QueryString["token"];


                if (string.IsNullOrEmpty(resource) || !_requestToModule.TryGetValue(resource, out BaseApiModule service))
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

        public void SetErrorResponse(HttpRequestEventArgs e)
        {
            e.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }


          public void RegisterRequest(string resource, BaseApiModule module)
          {
              // Стандартная регистрация ресурса с использованием версии модуля:
              _requestToModule.Add($"/{module.Name}/{module.Version.Major}/{resource}", module);
          }
          
         
          
          public void SendResponse(HttpRequestEventArgs e, ISdkMessage response)
          {
              e.Response.SendChunked = true;
            
              SdkMessageContainer rabbitMqMessageContainer = _sdkPacketSerializer.Serialize(response);
            
              using (var streamWriter = new StreamWriter(e.Response.OutputStream, Encoding.UTF8))
              {
                  var serializer = JsonSerializer.Create(new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                  var jsonWriter = new JsonTextWriter(streamWriter);

                  try
                  {
                      _logger.Debug(() => $"Send response: {rabbitMqMessageContainer.Identifier}");
                      serializer.Serialize(jsonWriter, new [] { rabbitMqMessageContainer });
                  }
                  finally
                  {
                      jsonWriter.Close();
                  }
              }
          }
    }
}
