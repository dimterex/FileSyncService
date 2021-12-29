using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using NLog;
using SdkProject;
using WebSocketSharp.Server;

namespace TransportProject
{
    public class ApiController
    {
        public const string API_RESOURCE_PATH = "/api";
        
        private readonly PacketSerializer _package;
        private static ILogger _logger;

        
        private readonly Dictionary<string, BaseApiModule> _requestToModule;


        public ApiController()
        {
            _logger = LogManager.GetCurrentClassLogger();
            _package = new PacketSerializer();
            _requestToModule = new Dictionary<string, BaseApiModule>();
        }
        
        public void HandleRequest(string resource, HttpRequestEventArgs e)
        {
            try
            {
                string token = e.Request.QueryString["token"];


                if (string.IsNullOrEmpty(resource) || !_requestToModule.TryGetValue(resource, out BaseApiModule service))
                {
                    e.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }
                
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


          public void RegisterRequest(string resource, BaseApiModule module)
          {
              // Стандартная регистрация ресурса с использованием версии модуля:
              _requestToModule.Add($"/{module.Name}/{module.Version.Major}/{resource}", module);
          }
          
          private MessageContainer SerializePacket(IMessage message)
          {
              return _package.Serialize(message);
          }

          public IMessage DeserializePacket(MessageContainer messageContainer)
          {
              return _package.Deserialize(messageContainer);
          }
          
          public void SendResponse(HttpRequestEventArgs e, IMessage response)
          {
              e.Response.SendChunked = true;
            
              MessageContainer messageContainer = SerializePacket(response);
            
              using (var streamWriter = new StreamWriter(e.Response.OutputStream, Encoding.UTF8))
              {
                  var serializer = JsonSerializer.Create(new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                  var jsonWriter = new JsonTextWriter(streamWriter);

                  try
                  {
                      _logger.Debug(() => $"Send response: {messageContainer.Identifier}");
                      serializer.Serialize(jsonWriter, new [] { messageContainer });
                  }
                  finally
                  {
                      jsonWriter.Close();
                  }
              }
          }
    }
}
