namespace Service.Api.Message.Temp
{
    using Newtonsoft.Json;

    using Service.Api.Interfaces;
    using Service.Attribute.Api;

    [ApiMessage("SaveFileResponce")]
    public class SaveFileResponce : IMessage
    {
        [JsonProperty(PropertyName = "file_name")]
        public string FileName { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
