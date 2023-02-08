namespace ServicesApi.FileStorage
{
    using Common._Attribute_;
    using Common._Interfaces_;

    [RabbitMqApiMessage(MESSAGE_ID)]
    public class ClearEmptyDirectoriesRequest : IMessage
    {
        public const string MESSAGE_ID = "clear_empty_directories_request";
        
        public string[] FilePaths { get; set; }
    }
}
