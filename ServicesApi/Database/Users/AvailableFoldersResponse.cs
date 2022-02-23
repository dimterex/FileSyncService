using ServicesApi.Common._Attribute_;
using ServicesApi.Common._Interfaces_;

namespace ServicesApi.Database.Users
{
    [RabbitMqApiMessage(QueueConstants.DATABASE_QUEUE, "AvailableFoldersResponse")]
    public class AvailableFoldersResponse : IMessage
    {
        public string[] FilePaths { get; set; }
    }
}