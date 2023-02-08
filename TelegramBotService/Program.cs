namespace TelegramBotService
{
    using System;

    using Actions;

    using Commands;

    using Common.DatabaseProject;

    using Core.Customer;
    using Core.Daemon;
    using Core.Publisher;

    using Database.Actions;

    using NLog;

    using ServicesApi;

    internal class Program
    {
        private const string RABBIT_HOST = "RABBIT_HOST";
        private const string TELEGRAM_TOKEN = "TOKEN";
        private const string TELEGRAM_CHANNEL_ID = "CHANNEL_ID";
        private const string DB_PATH = "DB_PATH";

        public const string TAG = nameof(TelegramBotService);

        private static void Main(string[] args)
        {
            string host = Environment.GetEnvironmentVariable(RABBIT_HOST);
            string token = Environment.GetEnvironmentVariable(TELEGRAM_TOKEN);
            string rawChannelId = Environment.GetEnvironmentVariable(TELEGRAM_CHANNEL_ID);

            if (!int.TryParse(rawChannelId, out int channelId))
                throw new Exception("Not suppoerted channel ID.");

            var daemon = new Daemon();
            daemon.Run(
                () =>
                {
                    Logger logger = LogManager.GetLogger(TAG);
                    string dbPath = Environment.GetEnvironmentVariable(DB_PATH);
                    var database = new DataBaseFactory(dbPath);
                    logger.Info(() => "Starting...");

                    var customerController = new CustomerController(host, QueueConstants.TELEGRAM_QUEUE);

                    // States
                    var telegramService = new TelegramService(token, channelId);
                    customerController.Configure(new TelegramMessageAction(telegramService));

                    var publisherController = new RpcPublisherService(host);
                    var availableFoldersRequestExecutor = new AvailableFoldersRequestExecutor(database);

                    telegramService.Configure(
                        "/clean_folders",
                        "clean empty folders",
                        new ClearFolderTelegramCommand(publisherController, availableFoldersRequestExecutor));
                    telegramService.Configure("/history", "get history log", new GetHistoryLogCommand(publisherController));

                    logger.Info(() => "Started");
                });
        }
    }
}
