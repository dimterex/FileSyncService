using System.Runtime.Serialization;

namespace SdkProject.Api.Confguration
{
    public enum AvailableFolderAction
    {
        [EnumMember(Value = "add")]
        Add,
        
        [EnumMember(Value = "remove")]
        Remove,
    }
}