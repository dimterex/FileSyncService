namespace SdkProject.Api.Sync
{
    using _Attribute_;

    using _Interfaces_;

    using Newtonsoft.Json;

    [SdkApiMessage("file_updated_response")]
    public class FileUpdatedResponse : ISdkMessage
    {
        #region Constructors

        public FileUpdatedResponse()
        {
            Size = 0;
        }

        #endregion

        #region Properties

        [JsonProperty(PropertyName = "file_name")]
        public string[] FileName { get; set; }

        [JsonProperty(PropertyName = "size")]
        public long Size { get; set; }

        #endregion
    }
}
