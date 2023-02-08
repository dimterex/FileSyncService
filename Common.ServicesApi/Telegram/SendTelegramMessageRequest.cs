namespace ServicesApi.Telegram
{
    using Common._Attribute_;
    using Common._Interfaces_;

    using Newtonsoft.Json;

    [RabbitMqApiMessage(MESSAGE_ID)]
    public class SendTelegramMessageRequest : IMessage
    {
        public const string MESSAGE_ID = "send_telegram_message_request";

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
