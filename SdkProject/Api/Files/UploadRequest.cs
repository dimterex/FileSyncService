namespace SdkProject.Api.Files;

using _Interfaces_;

using Newtonsoft.Json;

public class UploadRequest : ISdkMessage
{
    [JsonProperty(PropertyName = "file_name")]
    public string FileName { get; set; }

    [JsonProperty(PropertyName = "token")]
    public string Token { get; set; }
}