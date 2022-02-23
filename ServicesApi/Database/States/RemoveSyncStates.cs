using ServicesApi.Common._Attribute_;
using ServicesApi.Common._Interfaces_;

namespace ServicesApi.Database.States
{
    
    [RabbitMqApiMessage(QueueConstants.DATABASE_QUEUE, "RemoveSyncStates")]
    public class RemoveSyncStates : IMessage
    {
        public string Login { get; set; }
        
        public string[] FilePaths { get; set; }
    }
}