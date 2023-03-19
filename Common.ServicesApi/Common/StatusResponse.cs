namespace ServicesApi.Common
{
    using _Attribute_;

    using _Interfaces_;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [RabbitMqApiMessage(STATUS_RESPONSE_TYPE)]
    public class StatusResponse : IMessage
    {
        public const string STATUS_RESPONSE_TYPE = "status_response_type";

        [JsonProperty(PropertyName = "status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Status Status { get; set; }

        [JsonProperty(PropertyName = "message")]
        public object Message { get; set; }
    }
}
