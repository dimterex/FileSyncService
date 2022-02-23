using ServicesApi.Common._Attribute_;
using ServicesApi.Common._Interfaces_;

namespace ServicesApi.Database.Users
{
    [RabbitMqApiMessage(QueueConstants.DATABASE_QUEUE, "AvailableFoldersRequest")]
    public class AvailableFoldersRequest : IMessage
    {
        
    }
}