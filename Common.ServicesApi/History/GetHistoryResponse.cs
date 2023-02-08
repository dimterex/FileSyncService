namespace ServicesApi.History
{
    using System.Collections.Generic;

    using Common._Attribute_;
    using Common._Interfaces_;

    using Newtonsoft.Json;

    [RabbitMqApiMessage(MESSAGE_ID)]
    public class GetHistoryResponse : IMessage
    {
        public const string MESSAGE_ID = "get_history_response";

        public GetHistoryResponse()
        {
            Items = new List<HistoryDto>();
        }

        [JsonProperty(PropertyName = "items")]
        public List<HistoryDto> Items { get; set; }
    }
}
