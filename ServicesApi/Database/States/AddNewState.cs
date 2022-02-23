using ServicesApi.Common._Attribute_;
using ServicesApi.Common._Interfaces_;

namespace ServicesApi.Database.States
{
    [RabbitMqApiMessage(QueueConstants.DATABASE_QUEUE, "AddNewState")]
    
    public class AddNewState : IMessage
    {
        public string Login { get; set; }
        
        public string FilePath { get; set; }
    }
}