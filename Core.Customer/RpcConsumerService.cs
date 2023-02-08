namespace Core.Customer
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Newtonsoft.Json;

    using NLog;

    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    using ServicesApi;
    using ServicesApi.Common;
    using ServicesApi.Common._Interfaces_;

    public class CustomerController
    {
        private const string TAG = nameof(CustomerController);
        private readonly IConnection _connection;
        private readonly IModel _connectionChannel;
        private readonly ILogger _logger;

        private readonly Dictionary<Type, Func<IMessage, IMessage>> _methods;
        private readonly RabbitMqPacketSerializer _package;

        public CustomerController(string host, string queue)
        {
            _logger = LogManager.GetLogger(TAG);
            _logger.Info(() => "Initializing...");
            _package = new RabbitMqPacketSerializer();
            _methods = new Dictionary<Type, Func<IMessage, IMessage>>();

            var factory = new ConnectionFactory
            {
                Uri = new Uri(host)
            };

            _connection = factory.CreateConnection();
            _connectionChannel = _connection.CreateModel();

            _connectionChannel.QueueDeclare(queue, false, false, false, null);

            var consumer = new EventingBasicConsumer(_connectionChannel);

            consumer.Received += (sender, e) =>
            {
                ReadOnlyMemory<byte> body = e.Body;
                string message = Encoding.UTF8.GetString(body.ToArray());

                Received(
                    message,
                    response =>
                    {
                        byte[] responseBytes = Encoding.UTF8.GetBytes(response.Serialize());
                        IBasicProperties props = e.BasicProperties;
                        IBasicProperties replyProps = _connectionChannel.CreateBasicProperties();
                        replyProps.CorrelationId = props.CorrelationId;
                        _connectionChannel.BasicPublish(string.Empty, props.ReplyTo, replyProps, responseBytes);
                    });
            };

            _connectionChannel.BasicConsume(queue, true, consumer);

            _logger.Info(() => $"Subscribed to the queue '{queue}'");
        }

        private void Received(string data, Action<RabbitMqMessageContainer> sendResponse)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                _logger.Error(() => "Emtpy data.");
                return;
            }

            _logger.Trace(() => $"Received: {data}.");
            var message = JsonConvert.DeserializeObject<RabbitMqMessageContainer>(data);
            IMessage payload = _package.Deserialize(message);

            Type type = payload.GetType();

            if (!_methods.TryGetValue(type, out Func<IMessage, IMessage> method))
                return;
            IMessage response = method(payload);
            sendResponse(_package.Serialize(response));
        }

        public void Configure<T>(IMessageHandler<T> messageHandler)
            where T : IMessage
        {
            if (_methods.TryGetValue(typeof(T), out Func<IMessage, IMessage> method))
            {
                _logger.Warn(() => $"{typeof(T)} was configured.");
                return;
            }

            _methods.Add(typeof(T), message => messageHandler.Handler((T)message));
        }
    }
}
