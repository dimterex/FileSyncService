using System;
using System.Text;
using Core.Publisher._Interfaces_;
using NLog;
using RabbitMQ.Client;
using ServicesApi;
using ServicesApi.Common._Interfaces_;

namespace Core.Publisher
{
    public class PublisherService : IPublisherService
    {
        private const string TAG = nameof(PublisherService);
        private readonly string _host;

        private readonly RabbitMqPacketSerializer _package;
        private readonly ILogger _logger;

        public PublisherService(string host)
        {
            _logger = LogManager.GetLogger(TAG);
            _host = host;
            _package = new RabbitMqPacketSerializer();
            _logger.Info(() => $"Publisher {host} created.");
        }

        public void SendMessage(IMessage message)
        {
            var messageContainer = _package.Serialize(message);
            var rawMessage = messageContainer.Serialize();
            _logger.Info(() => $"Send: {rawMessage}.");

            var messageBytes = Encoding.UTF8.GetBytes(rawMessage);

            var factory = new ConnectionFactory
            {
                Uri = new Uri(_host)
            };

            var connection = factory.CreateConnection();
            var connectionChannel = connection.CreateModel();

            connectionChannel.BasicPublish(
                string.Empty,
                messageContainer.Queue,
                connectionChannel.CreateBasicProperties(),
                messageBytes);
        }
    }
}