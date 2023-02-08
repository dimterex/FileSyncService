namespace SdkProject.Api.Connection
{
    using System.Collections.Generic;

    using _Attribute_;

    using _Interfaces_;

    using Newtonsoft.Json;

    [SdkApiMessage("connection_response")]
    public class ConnectionResponse : ISdkMessage
    {
        #region Constructors

        public ConnectionResponse()
        {
            Shared_folders = new List<SharedFolder>();
        }

        #endregion

        #region Properties

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        [JsonProperty(PropertyName = "shared_folders")]
        public List<SharedFolder> Shared_folders { get; set; }

        #endregion
    }
}
