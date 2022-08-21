using System.Collections.Generic;
using Newtonsoft.Json;
using SdkProject._Attribute_;
using SdkProject._Interfaces_;

namespace SdkProject.Api.Sync
{
    [SdkApiMessage("SyncStateFilesResponse")]
    public class SyncStateFilesResponse : ISdkMessage
    {
        public SyncStateFilesResponse()
        {
            AddedFiles = new List<FileAddResponse>();
            RemovedFiles = new List<FileRemoveResponse>();
            UploadedFiles = new List<FileUploadRequest>();
            UpdatedFiles = new List<FileUpdatedResponse>();
            ServerRemovedFiles = new List<FileServerRemovedResponse>();
            DatabaseAddedFiles = new List<FileDataBaseAddResponse>();
        }

        [JsonProperty(PropertyName = "added_files")]
        public List<FileAddResponse> AddedFiles { get; set; }

        [JsonProperty(PropertyName = "removed_files")]
        public List<FileRemoveResponse> RemovedFiles { get; set; }

        [JsonProperty(PropertyName = "uploaded_files")]
        public List<FileUploadRequest> UploadedFiles { get; set; }

        [JsonProperty(PropertyName = "updated_files")]
        public List<FileUpdatedResponse> UpdatedFiles { get; set; }

        [JsonProperty(PropertyName = "server_removed_files")]
        public List<FileServerRemovedResponse> ServerRemovedFiles { get; set; }

        [JsonProperty(PropertyName = "database_added_files")]
        public List<FileDataBaseAddResponse> DatabaseAddedFiles { get; set; }
    }
}