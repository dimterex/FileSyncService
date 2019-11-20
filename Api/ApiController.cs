namespace Service.Api
{
    using Newtonsoft.Json;

    using NLog;

    using Service.Api.Interfaces;
    using Service.Api.Message;
    using Service.Api.Serializer;
    using Service.Transport;

    using System;
    using System.Collections.Generic;

    public class ApiController
    {
        private const int BUFFER_SIZE = 1024;

        private PacketSerializer _package;
        private static ILogger _logger;

        private readonly Dictionary<Type, Action<object, IClient>> _messageHandlers;

        public ApiController()
        {
            _logger = LogManager.GetCurrentClassLogger();
            _package = new PacketSerializer();
            _messageHandlers = new Dictionary<Type, Action<object, IClient>>();
        }
        
        public void Send(IClient wsClient, IMessage message)
        {
            var result = _package.Serialize(message);
            wsClient.SendMessage(result);
        }

        public void Execute(string data, IClient wsClient)
        {
            var messages = JsonConvert.DeserializeObject<MessageContainer[]>(data);
            foreach (var message in messages)
            {
                var obj = _package.Deserialize(message);
                if (obj != null)
                    ReceiveMessageLowLevel(wsClient, obj);
                else
                    _logger.Debug(() => $"Cannot deserialize {message.Identifier}");
            }
        }

        public void Configure<T>(Action<T, IClient> handler) where T : IMessage
        {
            _messageHandlers[typeof(T)] = (x, y) => handler((T)x, y);
        }

        private void ReceiveMessageLowLevel(IClient client, IMessage message)
        {
            if (_messageHandlers.TryGetValue(message.GetType(), out var handler))
                handler(message, client);
        }
    }
}
