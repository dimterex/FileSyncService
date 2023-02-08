namespace ServicesApi.Configuration
{
    using Common._Attribute_;
    using Common._Interfaces_;

    using Newtonsoft.Json;

    [RabbitMqApiMessage(MESSAGE_ID)]
    public class UpdateSyncStateRequest : IMessage
    {
        public const string MESSAGE_ID = "update_sync_state_request";

        [JsonProperty(PropertyName = "login")]
        public string Login { get; set; }

        [JsonProperty(PropertyName = "sync_files")]
        public string[] SyncFiles { get; set; }
    }
}
