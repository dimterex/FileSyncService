﻿using System;
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

        private readonly Dictionary<Type, Action<IClient, IMessage>> _methods;
        private readonly Dictionary<string, Action<IClient, HttpRequestEventArgs>> _restMethods;
        private readonly Dictionary<Type, Action<IClient, IMessage, IMessage, HttpRequestEventArgs>> _restRequestMethods;

        #endregion Fields

        #region Properties

        public string Name { get; }

        public Version Version { get; }

        public ApiController ApiController { get; private set; }

        public string Path => $"{ApiController.API_RESOURCE_PATH}/{Name}/{Version.Major}";

        #endregion Properties

        #region Constructors

        protected BaseApiModule(string name, Version version)
        {
            Name = name;
            Version = version;

            _methods = new Dictionary<Type, Action<IClient, IMessage>>();
            _restMethods = new Dictionary<string, Action<IClient, HttpRequestEventArgs>>();
            _restRequestMethods = new Dictionary<Type, Action<IClient, IMessage, IMessage, HttpRequestEventArgs>>();
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
            _restMethods.Add($"{"POST"} {Version.Major}/{request}", (client, e) =>
            {
                
                var data = DeserializeFromStream(e.Request.InputStream).ToString();
                
                var messages = JsonConvert.DeserializeObject<MessageContainer[]>(data);
                foreach (var message in messages)
                {
                    var payload = ApiController.DeserializePacket(message);
                    
                    var type = payload.GetType();
                    
                    if (!_restRequestMethods.TryGetValue(type, out Action<IClient, IMessage, IMessage, HttpRequestEventArgs> method))
                        return;
              
                    method(client, DeserializeRequest<SyncFilesRequest>(e.Request.QueryString), payload, e);
                }
            });
            
            OnInitialize();
        }

        public void Handle(IClient client, IMessage payload)
        {
            var type = payload.GetType();

            if (!_methods.TryGetValue(type, out Action<IClient, IMessage> method))
                return;

            method(client, payload);
        }

        /// <summary>
        /// Выполнение необходимого обработчика на REST-запрос.
        /// </summary>
        public void Handle(IClient client, string resource, HttpRequestEventArgs e)
        {
            if (!_restMethods.TryGetValue($"{e.Request.HttpMethod} {resource}", out Action<IClient, HttpRequestEventArgs> method)) // Метод обработки не зарегистрирован.
            {
                e.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                e.Response.StatusDescription = $"Unknown {e.Request.HttpMethod}-method '{resource}' or module '{Name}'.";
                return;
            }

            method(client, e);
        }
  

        /// <summary>
        /// Регистрирует обработчик сообщения.
        /// </summary>
        /// <typeparam name="T">Тип сообщения.</typeparam>
        /// <param name="method">Делегат-обработчик сообщения.</param>
        protected void RegisterMessage<T>(Action<IClient, T> method)
        {
            ApiController.RegisterMessage<T>(this);
            _methods.Add(typeof(T), (client, message) => method(client, (T)message));
        }

        /// <summary>
        /// Десериализация параметров REST-запроса в объект для обработчика.
        /// </summary>
        /// <typeparam name="T">Тип объекта.</typeparam>
        /// <param name="query">Строка с параметрами.</param>
        /// <returns>Объект подготовленными полями.</returns>
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
        

        /// <summary>
        /// Регистрирует обработчик REST GET-запроса.
        /// </summary>
        /// <param name="resourceName">Имя запроса.</param>
        /// <param name="method">Метод для обработки запроса.</param>
        protected void RegisterGetRequest<T>(string resourceName, Action<IClient, T, HttpRequestEventArgs> method)
        {
            RegisterRequest<T>("GET", resourceName, method);
        }

        /// <summary>
        /// Регистрирует обработчик REST POST-запроса.
        /// </summary>
        /// <param name="resourceName">Имя запроса.</param>
        /// <param name="method">Метод для обработки запроса.</param>
        protected void RegisterPostRequest<T>(string resourceName, Action<IClient, T, HttpRequestEventArgs> method)
        {
            RegisterRequest<T>("POST", resourceName, method);
        }

        /// <summary>
        /// Регистрирует обработчик REST-запроса.
        /// </summary>
        /// <param name="httpMethod">Http метод.</param>
        /// <param name="resourceName">Имя запроса.</param>
        /// <param name="method">Метод для обработки запроса.</param>
        protected void RegisterRequest<T>(string httpMethod, string resourceName, Action<IClient, T, HttpRequestEventArgs> method)
        {
            ApiController.RegisterRequest(resourceName, this);
            _restMethods.Add($"{httpMethod} {Version.Major}/{resourceName}", (client, e) => method(client, DeserializeRequest<T>(e.Request.QueryString), e));
        }
        
        /// <summary>
        /// Регистрирует обработчик REST POST-запроса.
        /// </summary>
        /// <param name="method">Метод для обработки запроса.</param>
        protected void RegisterPostRequestWithBody<TResponse>(Action<IClient, SyncFilesRequest, TResponse, HttpRequestEventArgs> method)
        {
            _restRequestMethods.Add(typeof(TResponse), (client, request, response, e) => method(client, (SyncFilesRequest)request, (TResponse)response, e));
        } 
        

        protected virtual void OnInitialize()
        {
        }

        #endregion Methods
    }
}