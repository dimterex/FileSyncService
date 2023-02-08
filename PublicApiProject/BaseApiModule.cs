namespace PublicProject
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Net;

    using _Interfaces_;

    using Modules;

    using Newtonsoft.Json;

    using SdkProject;
    using SdkProject._Interfaces_;
    using SdkProject.Api;
    using SdkProject.Api.Sync;

    public abstract class BaseApiModule
    {
        #region Events

        private readonly SdkPacketSerializer _sdkPacketSerializer;

        #endregion Events

        #region Constructors

        protected BaseApiModule(string name, Version version, IApiController apiController)
        {
            _apiController = apiController;
            Name = name;
            Version = version;

            _restMethods = new Dictionary<string, Action<HttpRequestEventModel>>();
            _restRequestMethods = new Dictionary<Type, Action<ISdkMessage, ISdkMessage, HttpRequestEventModel>>();
            _sdkPacketSerializer = new SdkPacketSerializer();
            Initialize();
        }

        #endregion Constructors

        #region Fields

        private readonly Dictionary<string, Action<HttpRequestEventModel>> _restMethods;
        private readonly Dictionary<Type, Action<ISdkMessage, ISdkMessage, HttpRequestEventModel>> _restRequestMethods;
        protected readonly IApiController _apiController;

        #endregion Fields

        #region Properties

        public string Name { get; }

        public Version Version { get; }

        #endregion Properties

        #region Methods

        private void Initialize()
        {
            const string request = "request";

            _apiController.RegisterRequest(request, this);
            _restMethods.Add(
                $"{"POST"} {Version.Major}/{request}",
                e =>
                {
                    var data = DeserializeFromStream(e.Request.InputStream).ToString();

                    SdkMessageContainer[] messages = JsonConvert.DeserializeObject<SdkMessageContainer[]>(data);
                    foreach (SdkMessageContainer message in messages)
                    {
                        ISdkMessage payload = _sdkPacketSerializer.Deserialize(message);

                        Type type = payload.GetType();

                        if (!_restRequestMethods.TryGetValue(type, out Action<ISdkMessage, ISdkMessage, HttpRequestEventModel> method))
                            return;

                        method(DeserializeRequest<SyncFilesRequest>(e.Request.QueryString), payload, e);
                    }
                });

            OnInitialize();
        }

        public void Handle(string resource, HttpRequestEventModel e)
        {
            if (!_restMethods.TryGetValue($"{e.Request.HttpMethod} {resource}", out Action<HttpRequestEventModel> method))
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
                JsonConvert.SerializeObject(query.Cast<string>().ToDictionary(key => key, value => query[value])));
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

        protected void RegisterGetRequest<T>(string resourceName, Action<T, HttpRequestEventModel> method)
        {
            RegisterRequest("GET", resourceName, method);
        }

        protected void RegisterPostRequest<T>(string resourceName, Action<T, HttpRequestEventModel> method)
        {
            RegisterRequest("POST", resourceName, method);
        }

        private void RegisterRequest<T>(string httpMethod, string resourceName, Action<T, HttpRequestEventModel> method)
        {
            _apiController.RegisterRequest(resourceName, this);
            _restMethods.Add($"{httpMethod} {Version.Major}/{resourceName}", e => method(DeserializeRequest<T>(e.Request.QueryString), e));
        }

        /// <summary>
        /// Регистрирует обработчик REST POST-запроса.
        /// </summary>
        /// <param name = "method">Метод для обработки запроса.</param>
        protected void RegisterPostRequestWithBody<TResponse>(Action<SyncFilesRequest, TResponse, HttpRequestEventModel> method)
        {
            _restRequestMethods.Add(typeof(TResponse), (request, response, e) => method((SyncFilesRequest)request, (TResponse)response, e));
        }

        protected virtual void OnInitialize()
        {
        }

        #endregion Methods
    }
}
