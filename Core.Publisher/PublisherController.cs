using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ServicesApi;
using ServicesApi.Common;
using ServicesApi.Common._Interfaces_;

namespace Core.Publisher
{
    public class PublisherController : IPublisherService
    { 
        private readonly ILogger _logger;
        private readonly RabbitMqPacketSerializer _package;

        private readonly IConnection _connection;
        
        private readonly IModel _connectionChannel;
        private readonly string _replyQueueName;
        private readonly EventingBasicConsumer _consumer;
        private readonly BlockingCollection<IMessage> _respQueue;
        private readonly IBasicProperties _props;

        public PublisherController(string host, string user, string password)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _package = new RabbitMqPacketSerializer();
            _respQueue = new BlockingCollection<IMessage>();
            
            var factory = new ConnectionFactory()
            {
                HostName = host,
                // UserName = user,
                // Password = password,
            };
            
            _connection = factory.CreateConnection();
            _connectionChannel = _connection.CreateModel();
            _connectionChannel.ConfirmSelect();
            _replyQueueName = _connectionChannel.QueueDeclare().QueueName;
            _consumer = new EventingBasicConsumer(_connectionChannel);

            _props = _connectionChannel.CreateBasicProperties();
            
            string correlationId = Guid.NewGuid().ToString();
            _props.CorrelationId = correlationId;
            _props.ReplyTo = _replyQueueName;

            _consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                string response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    var message = JsonConvert.DeserializeObject<RabbitMqMessageContainer>(response);
                    var payload = _package.Deserialize(message);
                    _respQueue.Add(payload);
                }
            };
            _connectionChannel.BasicAcks += (sender, ea) =>
            {

            };
            _connectionChannel.BasicNacks += (sender, ea) =>
            {

            };
        }

        public void Stop()
        {
            _connection.Close();
            _connection.Dispose();
            
            _connectionChannel.Close();
            _connectionChannel.Dispose();
        }

        public IMessage SendWithResponse(IMessage message)
        {
            var messageContainer = _package.Serialize(message);
            var messageBytes = Encoding.UTF8.GetBytes(messageContainer.Serialize());

            _connectionChannel.BasicPublish(
                exchange: string.Empty,
                routingKey: messageContainer.Queue,
                basicProperties: _props,
                body: messageBytes);

            _connectionChannel.BasicConsume(
                consumer: _consumer,
                queue: _replyQueueName,
                autoAck: true);

            return _respQueue.Take();            
        }
        
        public void Send(IMessage message)
        {
            var messageContainer = _package.Serialize(message);
            var messageBytes = Encoding.UTF8.GetBytes(messageContainer.Serialize());

            _connectionChannel.BasicPublish(
                exchange: string.Empty,
                routingKey: messageContainer.Queue,
                basicProperties: _props,
                body: messageBytes);
        }
    }
}