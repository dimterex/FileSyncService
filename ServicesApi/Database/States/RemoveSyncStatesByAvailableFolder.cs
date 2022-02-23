using ServicesApi.Common._Attribute_;
using ServicesApi.Common._Interfaces_;

namespace ServicesApi.Database.States
{
    [RabbitMqApiMessage(QueueConstants.DATABASE_QUEUE, "RemoveSyncStatesByAvailableFolder")]
    public class RemoveSyncStatesByAvailableFolder : IMessage
    {
        public string Login { get; set; }
        
        public string AvailableFolder { get; set; }
    }
}