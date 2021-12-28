using DataBaseProject;
using FileSystemProject;
using SdkProject;
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
            var connectionStateManager = new ConnectionStateManager();
            var fileManager = new FileManager();
            var apiController = new ApiController();
            var settingsManager = new SettingsManager();
            
            var syncDb = new SyncTableDataBase();
            var userDb = new UserTableDataBase();
            
            var syncModule = new CoreModule(fileManager, syncDb, connectionStateManager, userDb);
            syncModule.Initialize(apiController);

            var attachModule = new FilesApi(new FilesService(connectionStateManager, syncDb));
            attachModule.Initialize(apiController);

            var configModule = new ConfigurationModule(userDb, syncDb);
            configModule.Initialize(apiController);
            
            var ws = new WsService(connectionStateManager, apiController, IPAddress.Parse(settingsManager.Settings.IpAddress), settingsManager.Settings.HttpPort, settingsManager.Settings.HttpsPort);
            ws.Start();

            // TODO: Добавить CancellationToken
            Console.ReadKey(true);

            ws.Stop();
        }
    }
}