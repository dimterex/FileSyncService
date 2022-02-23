using ServicesApi.Common._Attribute_;
using ServicesApi.Common._Interfaces_;

namespace ServicesApi.Database.States
{
    [RabbitMqApiMessage(QueueConstants.DATABASE_QUEUE, "AddNewStates")]
    public class AddNewStates : IMessage
    {
        public string Login { get; set; }
        
        public string[] FilePaths { get; set; }
    }
}