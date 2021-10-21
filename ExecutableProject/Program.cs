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
            
            IPAddress[] localIp = Dns.GetHostAddresses(Dns.GetHostName());

            string ipString = string.Empty;

            foreach (IPAddress address in localIp)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipString = address.ToString();
                    break;
                }
            }

            var syncDb = new SyncTableDataBase();
            var userDb = new UserTableDataBase();
            
            
            // TODO: Вынести в настройки.
            var port = 1234;

            var syncModule = new SyncModule(fileManager, syncDb, connectionStateManager, userDb);
            syncModule.Initialize(apiController);

            var attachModule = new AttachmentApi(new AttachmentService(connectionStateManager, syncDb));
            attachModule.Initialize(apiController);

            var connectionModule = new ConnectionModule(connectionStateManager, userDb);
            connectionModule.Initialize(apiController);

            var configModule = new ConfigurationModule(userDb, syncDb);
            configModule.Initialize(apiController);
            
            var ws = new WsService(connectionStateManager, apiController, IPAddress.Parse(ipString), port, port++ );
            ws.Start();

            // TODO: Добавить CancellationToken
            Console.ReadKey(true);

            ws.Stop();
        }
    }
}