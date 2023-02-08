namespace Core.Publisher
{
    using System;
    using System.Collections.Concurrent;
    using System.Text;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    using NLog;

    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    using ServicesApi;
    using ServicesApi.Common;
    using ServicesApi.Common._Interfaces_;

    public class RpcClient : IDisposable
    {
        private readonly ConcurrentDictionary<string, TaskCompletionSource<IMessage>> _callbackMapper = new();
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly ILogger _logger;
        private readonly RabbitMqPacketSerializer _packetSerializer;
        private readonly string _replyQueueName;

        public RpcClient(string host, RabbitMqPacketSerializer rabbitMqPacketSerializer)
        {
            _packetSerializer = rabbitMqPacketSerializer;
            var factory = new ConnectionFactory
            {
                Uri = new Uri(host)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _replyQueueName = _channel.QueueDeclare().QueueName;

            _logger = LogManager.GetLogger($"RpcClient.{_replyQueueName}");
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                if (!_callbackMapper.TryRemove(ea.BasicProperties.CorrelationId, out TaskCompletionSource<IMessage> tcs))
                    return;
                byte[] body = ea.Body.ToArray();
                string response = Encoding.UTF8.GetString(body);
                _logger.Info(() => $"Received: {response}.");

                var message = JsonConvert.DeserializeObject<RabbitMqMessageContainer>(response);
                IMessage payload = _packetSerializer.Deserialize(message);
                tcs.TrySetResult(payload);
            };

            _channel.BasicConsume(consumer: consumer, queue: _replyQueueName, autoAck: true);
            _logger.Debug(() => "Initialized");
        }

        public void Dispose()
        {
            _connection.Close();
        }

        public Task<IMessage> CallAsync(string queue, IMessage message)
        {
            IBasicProperties props = _channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = _replyQueueName;

            RabbitMqMessageContainer messageContainer = _packetSerializer.Serialize(message);
            string rawMessage = messageContainer.Serialize();
            _logger.Info(() => $"Send: {rawMessage}.");

            byte[] messageBytes = Encoding.UTF8.GetBytes(rawMessage);

            var tcs = new TaskCompletionSource<IMessage>();
            _callbackMapper.TryAdd(correlationId, tcs);

            _channel.BasicPublish(string.Empty, queue, props, messageBytes);

            return tcs.Task;
        }
    }
}
