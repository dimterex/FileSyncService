using ServicesApi.Common._Attribute_;
using ServicesApi.Common._Interfaces_;

namespace ServicesApi.Database.States
{
    [RabbitMqApiMessage(QueueConstants.DATABASE_QUEUE, "SyncStatesResponse")]
    public class SyncStatesResponse : IMessage
    {
        public string[] FilePaths { get; set; }
    }
}