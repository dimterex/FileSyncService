namespace ServicesApi.History
{
    using Common._Attribute_;
    using Common._Interfaces_;

    [RabbitMqApiMessage(MESSAGE_ID)]
    public class GetHistoryRequest : IMessage
    {
        public const string MESSAGE_ID = "get_history_request";
    }
}
