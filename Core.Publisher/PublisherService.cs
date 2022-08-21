using System;
using System.Text;
using Core.Logger;
using Core.Logger._Enums_;
using Core.Logger._Interfaces_;
using Core.Publisher._Interfaces_;
using RabbitMQ.Client;
using ServicesApi;
using ServicesApi.Common._Interfaces_;

namespace Core.Publisher
{
    public class PublisherService : IPublisherService
    {
        private const string TAG = nameof(PublisherService);
        private readonly string _host;
        private readonly ILoggerService _loggerService;

        private readonly RabbitMqPacketSerializer _package;

        public PublisherService(string host, ILoggerService loggerService)
        {
            _host = host;
            _loggerService = loggerService;
            _package = new RabbitMqPacketSerializer();
            _loggerService.SendLog(LogLevel.Info, TAG, () => $"Publisher {host} created.");
        }

        public void SendMessage(IMessage message)
        {
            var messageContainer = _package.Serialize(message);
            var rawMessage = messageContainer.Serialize();
            if (messageContainer.Queue != QueueConstants.LOGGER_QUEUE)
                _loggerService.SendLog(LogLevel.Info, TAG, () => $"Send: {rawMessage}.");

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