namespace Core.Publisher
{
    using _Interfaces_;

    using NLog;

    using ServicesApi;
    using ServicesApi.Common._Interfaces_;

    public class RpcPublisherService : IPublisherService
    {
        private const string TAG = nameof(RpcPublisherService);
        private readonly string _host;
        private readonly ILogger _logger;
        private readonly RabbitMqPacketSerializer _packetSerializer;

        public RpcPublisherService(string host)
        {
            _host = host;
            _logger = LogManager.GetLogger(TAG);
            _packetSerializer = new RabbitMqPacketSerializer();

            _logger.Info(() => $"Publisher {host} created.");
        }

        public IMessage CallAsync(string queue, IMessage message)
        {
            using (var rcp = new RpcClient(_host, _packetSerializer))
            {
                IMessage response = rcp.CallAsync(queue, message).Result;
                return response;
            }
        }
    }
}
