using System.Collections.Generic;
using System.Text;
using DataBaseProject;
using FileSystemProject;
using SdkProject;
using TelegramBotProject;
using TransportProject;

namespace ExecutableProject
{
      using Service.Api;
    using Service.Api.Module;

    using System;
    using System.Net;
    using System.Net.Sockets;

    class Program
    {
        /// <summary>
        /// TODO: Добавить DI.
        /// </summary>
        static void Main(string[] args)
        {
            IConnectionStateManager connectionStateManager = new ConnectionStateManager();
            IFileManager fileManager = new FileManager();
            var apiController = new ApiController();
            var settingsManager = new SettingsManager();
            
            ISyncTableDataBase syncDb = new SyncTableDataBase();
            IUserTableDataBase userDb = new UserTableDataBase();
            var telegramService = new TelegramService(settingsManager.Settings.Token, settingsManager.Settings.Telegram_id);

            telegramService.Configure("/clean_folders", "clean empty folders", () =>
            {
                var ls = userDb.GetAvailableFolders();
                var removedList = fileManager.RemoveEmptyDirectories(ls);
                var sb = new StringBuilder();
                sb.AppendLine("Removed dictionaries:");
                sb.AppendJoin(Environment.NewLine, removedList);
                SendToTelegram(sb.ToString());
            });
            
            var syncModule = new CoreModule(fileManager, syncDb, connectionStateManager, userDb);
            syncModule.Initialize(apiController);
            syncModule.SendMessage += (o, s) => SendToTelegram(s);
            
            var attachModule = new FilesApi(new FilesService(connectionStateManager, syncDb), connectionStateManager);
            attachModule.Initialize(apiController);
            attachModule.SendMessage += (o, s) => SendToTelegram(s);

            var configModule = new ConfigurationModule(userDb, syncDb);
            configModule.Initialize(apiController);
            configModule.SendMessage += (o, s) => SendToTelegram(s);
            
            var ws = new WsService(connectionStateManager, apiController, IPAddress.Parse(settingsManager.Settings.IpAddress), settingsManager.Settings.HttpPort, settingsManager.Settings.HttpsPort);
            ws.Start();

            void SendToTelegram(string message)
            {
                telegramService.SendTextMessageAsync(message);
            }
            
            // TODO: Добавить CancellationToken
            Console.ReadKey(true);

            ws.Stop();
        }
    }
}