using System.Collections.Generic;
using Newtonsoft.Json;

namespace SdkProject.Api.Connection
{
    public class SharedFolder
    {
        public SharedFolder()
        {
            Files = new List<string>();
        }

        [JsonProperty(PropertyName = "path")] public List<string> Files { get; set; }
    }
}