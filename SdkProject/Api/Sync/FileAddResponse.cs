namespace SdkProject.Api.Sync
{
    using _Attribute_;

    using _Interfaces_;

    using Newtonsoft.Json;

    [SdkApiMessage("file_add_response")]
    public class FileAddResponse : ISdkMessage
    {
        #region Properties

        [JsonProperty(PropertyName = "size")]
        public long Size { get; set; }

        [JsonProperty(PropertyName = "file_name")]
        public string[] FileName { get; set; }

        #endregion

        #region Constructors

        public FileAddResponse()
        {
            Size = 0;
        }

        #endregion
    }
}
