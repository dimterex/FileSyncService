using System;
using System.Collections.Generic;
using System.Net;
using NLog;
using SdkProject;
using WebSocketSharp.Server;

namespace TransportProject
{
    public class ApiController
    {
        /// <summary>
        /// Префикс API для идентификации REST-запросов.
        /// </summary>
        public const string API_RESOURCE_PATH = "/api";
        
        private const int BUFFER_SIZE = 1024;

        private PacketSerializer _package;
        private static ILogger _logger;

        
        private readonly Dictionary<Type, BaseApiModule> _messageToModule; // Тип MAPI-запроса в соответствии с модулем.
        private readonly Dictionary<string, BaseApiModule> _requestToModule; // URL REST-запроса (без префикса API) в соответствии с модулем.
        private IClient _wsClient;


        public ApiController()
        {
            _logger = LogManager.GetCurrentClassLogger();
            _package = new PacketSerializer();
            _messageToModule = new Dictionary<Type, BaseApiModule>();
            _requestToModule = new Dictionary<string, BaseApiModule>();
        }
        
        public void HandleMessage(IClient client, MessageContainer container)
        {
            IMessage message = _package.Deserialize(container);
            if (message != null)
            {
                HandleMessage(client, message);
                return;
            }

            _logger?.Warn(() => $"Discard message from {client} with unknown id {container.Identifier}");
        }
        
        public void HandleMessage(IClient client, IMessage message)
        {
            try
            {
                if (_messageToModule.TryGetValue(message.GetType(), out BaseApiModule service))
                {
                    service.Handle(client, message);
                }
                else
                {
                    _logger?.Warn(() => $"No API module was found to support the message. Message type - '{message.GetType()}'.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(() => $"{ex}");
            }
        }
        
        /// <summary>
        /// Конфигурирует сопоставление API-модуля и обрабатываемого им типа сообщения.
        /// </summary>
        /// <typeparam name="T">Тип сообщения.</typeparam>
        /// <param name="module">API-модуль, который будет обрабатывать сообщение.</param>
        public void RegisterMessage<T>(BaseApiModule module)
        {
            _messageToModule.Add(typeof(T), module);
        }
        
        /// <summary>
        /// Обработка полученного REST-запроса.
        /// </summary>
        /// <param name="resource">REST-запрос без префикса API. Например "/module_name/resource_name".</param>
        /// <param name="e">Параметры запроса.</param>
        public void HandleRequest(string resource, HttpRequestEventArgs e)
        {
            try
            {
                string token = e.Request.QueryString["token"];

                // AuthorizationInfo verification = VerifyAuthentication(token); // Аутентифкация клиента по токену из строки запроса.


                // if (!verification.IsAuthorized)
                // {
                //     // Проверять соответствие клиента с токеном по дополнительному полю логина не имеет смысла.
                //     // Если злоумышленник перехватил токен, скорее всего он перехватит и незашифрованный логин,
                //     // который сможет подставить в свою строку запроса и обойти данную проверку.
                //     // IP-адрес также не может быть использован в качестве гарантии отсутствия подмены клиента,
                //     // т.к. часть из них допускает использование динамических адресов.
                //     e.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                //     return;
                // }

                // Требуемый обработчик не был найден, ресурс отсутствует:
                if (string.IsNullOrEmpty(resource) || !_requestToModule.TryGetValue(resource, out BaseApiModule service))
                {
                    e.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }
                
                // Выполнение запроса клиента:
                service.Handle(_wsClient, // Получение и передача клиента для последующей проверки профиля.
                    resource.Substring(resource.IndexOf('/', 1) + 1), // Определение имени ресурса.
                    e); // Параметры REST-запроса.
            }
            catch (Exception ex)
            {
                _logger.Error(() => $"Handle Request Error {ex.Message}");

                try
                {
                    e.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // Произошла ошибка во время обработки запроса.
                }
                catch (InvalidOperationException)
                {
                    // Исключение при обработке запроса может быть вызвано разрывом подключения при передаче данных.
                    // При подобных ошибках может возникнуть ситуация, когда не представляется возможным
                    // изменить код статуса ответного сообщения, т.к. его заголовок уже был отправлен.
                }
            }
        }

        
          /// <summary>
          /// Конфигурирует REST-запрос в соответствии с экземпляром обрабатывающего его модуля.
          /// </summary>
          /// <param name="resource">REST-запрос модуля.</param>
          /// <param name="module">Экземпляр модуля.</param>
          public void RegisterRequest(string resource, BaseApiModule module)
          {
              // Стандартная регистрация ресурса с использованием версии модуля:
              _requestToModule.Add($"/{module.Name}/{module.Version.Major}/{resource}", module);
          }
          
          public MessageContainer SerializePacket(IMessage message)
          {
              return _package.Serialize(message);
          }

          public IMessage DeserializePacket(MessageContainer messageContainer)
          {
              return _package.Deserialize(messageContainer);
          }

          public void AddClient(IClient wsClient)
          {
              _wsClient = wsClient;
          }
    }
}
