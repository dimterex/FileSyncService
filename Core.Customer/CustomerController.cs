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

namespace Core.Customer
{
    public class CustomerController
    {
        private readonly ILogger _logger;
        private readonly RabbitMqPacketSerializer _package;

        private readonly Dictionary<Type, Action<IMessage>> _methods;
        private readonly Dictionary<Type, Action<IMessage, Action<IMessage>>> _methodsWithResponse;
        private readonly IConnection _connection;
        private readonly IModel _connectionChannel;

        public CustomerController(string host, string user, string password, string queue)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _package = new RabbitMqPacketSerializer();
            _methods = new Dictionary<Type, Action<IMessage>>();
            _methodsWithResponse = new Dictionary<Type, Action<IMessage, Action<IMessage>>>();
            
            var factory = new ConnectionFactory()
            {
                HostName = host,
                // UserName = user,
                // Password = password,
            };

            _connection = factory.CreateConnection();
            _connectionChannel = _connection.CreateModel();
            
            {
                _connectionChannel.QueueDeclare(queue: queue,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new EventingBasicConsumer(_connectionChannel);

                consumer.Received += (sender, e) =>
                {
                    
                    var body = e.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());
                  
                    
                    Receaved(message, (response) =>
                    {
                        var responseBytes = Encoding.UTF8.GetBytes(response);
                        var props = e.BasicProperties;
                        var replyProps = _connectionChannel.CreateBasicProperties();
                        replyProps.CorrelationId = props.CorrelationId;
                        _connectionChannel.BasicPublish(exchange: string.Empty, routingKey: props.ReplyTo,
                            basicProperties: replyProps, body: responseBytes);
                        // channel.BasicAck(deliveryTag: e.DeliveryTag,
                        //     multiple: false);
                    });
                };

                _connectionChannel.BasicConsume(queue: queue,
                    autoAck: true,
                    consumer: consumer);

                _logger.Info(() => $"Subscribed to the queue '{QueueConstants.DATABASE_QUEUE}'");
          
            }
        }

        public void Stop()
        {
            _connection.Dispose();
            _connectionChannel.Dispose();
        }

        private void Receaved(string data, Action<string> sendResponse)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                _logger.Error(() => $"Emtpy data.");
                return;
            }
            
            var message = JsonConvert.DeserializeObject<RabbitMqMessageContainer>(data);
            var payload = _package.Deserialize(message);
                    
            var type = payload.GetType();

            if (_methods.TryGetValue(type, out Action<IMessage> method))
            {
                method(payload);
            }
                
            if (_methodsWithResponse.TryGetValue(type, out Action<IMessage, Action<IMessage>> responseAction))
            {
                responseAction(payload, (response) =>
                {
                    var messageContainer = _package.Serialize(response);
                    sendResponse?.Invoke(messageContainer.Serialize());
                });
            }
        }

        public void Configure<T>(IMessageHandler<T> messageHandler) where T: IMessage
        {
            if (_methods.TryGetValue(typeof(T), out Action<IMessage> method))
            {
                _logger.Warn(() => $"{typeof(T)} was configured.");
                return;
            }
            
            _methods.Add(typeof(T), (message) => messageHandler.Handler((T)message));
        }

        public void ConfigureWithResponse<T>(IMessageHandlerWithResponse<T> messageHandler) where T: IMessage
        {
            if (_methodsWithResponse.TryGetValue(typeof(T), out Action<IMessage, Action<IMessage>> method))
            {
                _logger.Warn(() => $"{typeof(T)} was configured.");
                return;
            }
            
            _methodsWithResponse.Add(typeof(T), (message, action) =>
            {
                messageHandler.Handler((T)message, action);
            });
        }
    }
}