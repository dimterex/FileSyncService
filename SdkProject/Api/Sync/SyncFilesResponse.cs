using System.Collections.Generic;
using Newtonsoft.Json;

namespace SdkProject.Api.Sync
{
    [ApiMessage("SyncFilesResponse")]
    public class SyncFilesResponse : IMessage
    {
        [JsonProperty(PropertyName = "added_files")]
        public List<FileAddResponse> AddedFiles { get; set; }
        
        [JsonProperty(PropertyName = "removed_files")]
        public List<FileRemoveResponse> RemovedFiles { get; set; }
        
        [JsonProperty(PropertyName = "uploaded_files")]
        public List<FileUploadRequest> UploadedFiles { get; set; }
        
        [JsonProperty(PropertyName = "updated_files")]
        public List<FileUpdatedResponse> UpdatedFiles { get; set; }

        public SyncFilesResponse()
        {
            AddedFiles = new List<FileAddResponse>();
            RemovedFiles = new List<FileRemoveResponse>();
            UploadedFiles = new List<FileUploadRequest>();
            UpdatedFiles = new List<FileUpdatedResponse>();
        }
    }
}