namespace ServicesApi.Configuration
{
    using Common._Attribute_;
    using Common._Interfaces_;

    using Newtonsoft.Json;

    [RabbitMqApiMessage(MESSAGE_ID)]
    public class CreateUserRequest : IMessage
    {
        public const string MESSAGE_ID = "create_user_request";

        [JsonProperty(PropertyName = "login")]
        public string Login { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "available_folders")]
        public AvailableFolder[] AvailableFolders { get; set; }
    }
}
