using ServicesApi.Common._Attribute_;
using ServicesApi.Common._Interfaces_;

namespace ServicesApi.Telegram
{
    [RabbitMqApiMessage(QueueConstants.TELEGRAM_QUEUE, "TelegramMessage")]
    public class TelegramMessage : IMessage
    {
        public string Message { get; set; }
    }
}