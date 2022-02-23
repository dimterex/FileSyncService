using ServicesApi.Common._Attribute_;
using ServicesApi.Common._Interfaces_;

namespace ServicesApi.Database.Users
{
    [RabbitMqApiMessage(QueueConstants.DATABASE_QUEUE, "RemoveUserInfo")]
    public class RemoveUserInfo : IMessage
    {
        public string Login { get; set; }
        
        public string Password { get; set; }
        
        public string AvailableFolderPath { get; set; }
    }
}