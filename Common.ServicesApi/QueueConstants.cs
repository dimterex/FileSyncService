namespace ServicesApi
{
    public class QueueConstants
    {
#if DEBUG
        public const string FILE_STORAGE_QUEUE = "filestorage_queue_debug";
        public const string TELEGRAM_QUEUE = "telegram_queue_debug";
        public const string LOGGER_QUEUE = "logger_queue";
#else
        public const string FILE_STORAGE_QUEUE = "filestorage_queue";
        public const string TELEGRAM_QUEUE = "telegram_queue";
        public const string LOGGER_QUEUE = "logger_queue";
#endif
    }
}