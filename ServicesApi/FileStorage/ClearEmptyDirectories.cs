using ServicesApi.Common._Attribute_;
using ServicesApi.Common._Interfaces_;

namespace ServicesApi.FileStorage
{
    [RabbitMqApiMessage(QueueConstants.FILE_STORAGE_QUEUE, "ClearEmptyDirectories")]
    public class ClearEmptyDirectories : IMessage
    {
        public string[] FilePaths { get; set; }
    }
}