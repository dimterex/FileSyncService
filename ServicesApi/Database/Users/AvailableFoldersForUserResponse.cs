using ServicesApi.Common._Attribute_;
using ServicesApi.Common._Interfaces_;

namespace ServicesApi.Database.Users
{
    [RabbitMqApiMessage(QueueConstants.DATABASE_QUEUE, "AvailableFoldersForUserResponse")]
    public class AvailableFoldersForUserResponse : IMessage
    {
        public string[] FilePaths { get; set; }
    }
}