using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SdkProject._Interfaces_;

namespace SdkProject.Api.Files
{
    public class UploadResponse : ISdkMessage
    {
        [JsonProperty(PropertyName = "result")]
        [JsonConverter(typeof (StringEnumConverter))]
        public FilesOperationResult Result { get; set; }

        [JsonProperty(PropertyName = "file_id")]
        public string FileId { get; set; }
    }
    
    
    public enum FilesOperationResult
    {
        [EnumMember(Value = "success")] Success,
        [EnumMember(Value = "restricted_format")] RestrictedFormat,
        [EnumMember(Value = "file_too_large")] FileTooLarge,
        [EnumMember(Value = "not_found")] NotFound,
        [EnumMember(Value = "file_storage_unavailable")] FileStorageUnavailable,
        [EnumMember(Value = "exists_already")] ExistsAlready,
        [EnumMember(Value = "space_not_enough")] SpaceNotEnough,
        [EnumMember(Value = "unexpected_error")] UnexpectedError,
    }
}