using Newtonsoft.Json;
using SdkProject._Attribute_;
using SdkProject._Interfaces_;

namespace SdkProject.Api.Confguration
{
    [SdkApiMessage("CreateUserRequest")]
    public class CreateUserRequest : ISdkMessage
    {
        [JsonProperty(PropertyName = "login")]
        public string Login { get; set; }
        
        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }
        
        [JsonProperty(PropertyName = "available_folders")]
        public AvailableFolder[] AvailableFolders { get; set; }
    }
}