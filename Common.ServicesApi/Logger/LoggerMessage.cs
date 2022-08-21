using ServicesApi.Common._Attribute_;
using ServicesApi.Common._Interfaces_;

namespace ServicesApi.Logger
{
    [RabbitMqApiMessage(QueueConstants.LOGGER_QUEUE, "LoggerMessage")]
    public class LoggerMessage : IMessage
    {
        public string Level { get; set; }
        public string Tag { get; set; }
        public string Application { get; set; }
        public string Datetime { get; set; }
        public string Message { get; set; }
    }
}