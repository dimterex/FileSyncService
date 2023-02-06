using System;
using Common.DatabaseProject;
using Core.Customer;
using Core.Daemon;
using Core.Publisher;
using NLog;
using ServicesApi;
using TelegramBotService.Actions;
using TelegramBotService.Commands;
using TelegramBotService.Database.Actions;

namespace TelegramBotService
{
    internal class Program
    {
        private const string RABBIT_HOST = "RABBIT_HOST";
        private const string TELEGRAM_TOKEN = "TOKEN";
        private const string TELEGRAM_CHANNEL_ID = "CHANNEL_ID";
        private const string DB_PATH = "DB_PATH";

        public const string TAG = nameof(TelegramBotService);

        private static void Main(string[] args)
        {
            var host = Environment.GetEnvironmentVariable(RABBIT_HOST);
            var token = Environment.GetEnvironmentVariable(TELEGRAM_TOKEN);
            var rawChannelId = Environment.GetEnvironmentVariable(TELEGRAM_CHANNEL_ID);

            if (!int.TryParse(rawChannelId, out var channelId))
                throw new Exception("Not suppoerted channel ID.");

            var daemon = new Daemon();
            daemon.Run(() =>
            {
                var logger = LogManager.GetLogger(TAG);
                var dbPath = Environment.GetEnvironmentVariable(DB_PATH);
                var database = new DataBaseFactory(dbPath);
                logger.Info( () => "Starting...");

                var customerController = new CustomerController(host, QueueConstants.TELEGRAM_QUEUE);

                // States
                var telegramService = new TelegramService(token, channelId);
                customerController.Configure(new TelegramMessageAction(telegramService));

                var publisherController = new PublisherService(host);
                var availableFoldersRequestExecutor = new AvailableFoldersRequestExecutor(database);

                telegramService.Configure("/clean_folders", "clean empty folders",
                    new ClearFolderTelegramCommand(publisherController, availableFoldersRequestExecutor));
                logger.Info( () => "Started");
            });
        }
    }
}