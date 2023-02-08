namespace SdkProject.Api.Sync
{
    using _Interfaces_;

    using Newtonsoft.Json;

    public class SyncFilesRequest : ISdkMessage
    {
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
    }
}
