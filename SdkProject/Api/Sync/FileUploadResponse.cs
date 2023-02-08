namespace SdkProject.Api.Sync
{
    using _Attribute_;

    using _Interfaces_;

    using Newtonsoft.Json;

    [SdkApiMessage("file_upload_response")]
    public class FileUploadResponse : ISdkMessage
    {
        #region Properties

        [JsonProperty(PropertyName = "file_name")]
        public string[] FileName { get; set; }

        #endregion
    }
}
