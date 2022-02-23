using ServicesApi.Common._Attribute_;
using ServicesApi.Common._Interfaces_;

namespace ServicesApi.Database.States
{
    [RabbitMqApiMessage(QueueConstants.DATABASE_QUEUE, "SyncStatesRequest")]
    public class SyncStatesRequest : IMessage
    {
        public string Login { get; set; }
    }
}