namespace SdkProject.Api.Connection
{
    using _Attribute_;

    using _Interfaces_;

    using Newtonsoft.Json;

    [SdkApiMessage("connection_request")]
    public class ConnectionRequest : ISdkMessage
    {
        #region Properties

        [JsonProperty(PropertyName = "login")]
        public string Login { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        #endregion
    }
}
