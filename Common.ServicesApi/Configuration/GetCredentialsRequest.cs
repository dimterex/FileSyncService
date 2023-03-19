namespace ServicesApi.Configuration
{
    using Common._Attribute_;
    using Common._Interfaces_;

    [RabbitMqApiMessage(MESSAGE_ID)]
    public class GetCredentialsRequest : IMessage
    {
        public const string MESSAGE_ID = "get_credentials_request";
    }
}
