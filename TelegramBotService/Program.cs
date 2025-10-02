namespace TelegramBotService
{
    using System;

    using Commands;

    using Core.Daemon;


    using NLog;

    internal class Program
    {
        private const string RABBIT_HOST = "RABBIT_HOST";
        private const string TELEGRAM_TOKEN = "TOKEN";
        private const string FOLDER_TO_SAVE = "FOLDER_TO_SAVE";

        public const string TAG = nameof(TelegramBotService);

        private static void Main(string[] args)
        {
            string host = Environment.GetEnvironmentVariable(RABBIT_HOST);
            string token = Environment.GetEnvironmentVariable(TELEGRAM_TOKEN);
            string folderToSave = Environment.GetEnvironmentVariable(FOLDER_TO_SAVE);

            var daemon = new Daemon();
            daemon.Run(
                () =>
                {
                    Logger logger = LogManager.GetLogger(TAG);
                    logger.Info(() => "Starting...");

                    var processService = new ProcessService();
                    var telegramService = new TelegramService(token);

                    // var publisherController = new RpcPublisherService(host);
                    telegramService.Configure("/pip", "Download pip package", new GetPipPackageCommand(processService, folderToSave));
                    telegramService.Configure("/web", "Download website", new GetWebSiteCommand(folderToSave));
                    telegramService.Configure("/file", "Download file", new GetFileCommand(folderToSave));

                    logger.Info(() => "Started");
                });
        }
    }
}
