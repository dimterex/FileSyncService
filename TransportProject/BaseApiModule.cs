using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using SdkProject;
using SdkProject.Api.Sync;
using WebSocketSharp.Server;

namespace TransportProject
{
   public abstract class BaseApiModule
    {
        #region Fields

        private readonly Dictionary<string, Action<HttpRequestEventArgs>> _restMethods;
        private readonly Dictionary<Type, Action<IMessage, IMessage, HttpRequestEventArgs>> _restRequestMethods;

        #endregion Fields

        #region Properties

        public string Name { get; }

        public Version Version { get; }

        public ApiController ApiController { get; private set; }


        #endregion Properties

        #region Constructors

        protected BaseApiModule(string name, Version version)
        {
            Name = name;
            Version = version;

            _restMethods = new Dictionary<string, Action<HttpRequestEventArgs>>();
            _restRequestMethods = new Dictionary<Type, Action<IMessage, IMessage, HttpRequestEventArgs>>();
        } 

        #endregion Constructors

        #region Methods

        public void Initialize(ApiController controller)
        {
            if (ApiController != null)
                throw new Exception("Is already initialized");

            ApiController = controller;

            const string request = "request";
            
            ApiController.RegisterRequest(request, this);
            _restMethods.Add($"{"POST"} {Version.Major}/{request}", (e) =>
            {
                
                var data = DeserializeFromStream(e.Request.InputStream).ToString();
                
                var messages = JsonConvert.DeserializeObject<MessageContainer[]>(data);
                foreach (var message in messages)
                {
                    var payload = ApiController.DeserializePacket(message);
                    
                    var type = payload.GetType();
                    
                    if (!_restRequestMethods.TryGetValue(type, out Action<IMessage, IMessage, HttpRequestEventArgs> method))
                        return;
              
                    method(DeserializeRequest<SyncFilesRequest>(e.Request.QueryString), payload, e);
                }
            });
            
            OnInitialize();
        }

        public void Handle(string resource, HttpRequestEventArgs e)
        {
            if (!_restMethods.TryGetValue($"{e.Request.HttpMethod} {resource}", out Action<HttpRequestEventArgs> method))
            {
                e.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                e.Response.StatusDescription = $"Unknown {e.Request.HttpMethod}-method '{resource}' or module '{Name}'.";
                return;
            }

            method(e);
        }
  
        private static T DeserializeRequest<T>(NameValueCollection query)
        {
            return JsonConvert.DeserializeObject<T>(
                JsonConvert.SerializeObject(
                    query.Cast<string>().ToDictionary(key => key, value => query[value])));
        }
        
        private static object DeserializeFromStream(Stream stream)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            {
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    return serializer.Deserialize<object>(jsonTextReader);
                }
            }
        }
        
        protected void RegisterGetRequest<T>(string resourceName, Action<T, HttpRequestEventArgs> method)
        {
            RegisterRequest<T>("GET", resourceName, method);
        }

        protected void RegisterPostRequest<T>(string resourceName, Action<T, HttpRequestEventArgs> method)
        {
            RegisterRequest<T>("POST", resourceName, method);
        }

        private void RegisterRequest<T>(string httpMethod, string resourceName, Action<T, HttpRequestEventArgs> method)
        {
            ApiController.RegisterRequest(resourceName, this);
            _restMethods.Add($"{httpMethod} {Version.Major}/{resourceName}", (e) => method(DeserializeRequest<T>(e.Request.QueryString), e));
        }
        
        /// <summary>
        /// Регистрирует обработчик REST POST-запроса.
        /// </summary>
        /// <param name="method">Метод для обработки запроса.</param>
        protected void RegisterPostRequestWithBody<TResponse>(Action<SyncFilesRequest, TResponse, HttpRequestEventArgs> method)
        {
            _restRequestMethods.Add(typeof(TResponse), (request, response, e) => method((SyncFilesRequest)request, (TResponse)response, e));
        } 

        protected virtual void OnInitialize()
        {
        }

        #endregion Methods
    }
}