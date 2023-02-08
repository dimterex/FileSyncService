namespace ServicesApi.Configuration
{
    using System.Runtime.Serialization;

    public enum AvailableFolderAction
    {
        [EnumMember(Value = "add")]
        Add,

        [EnumMember(Value = "remove")]
        Remove
    }
}
