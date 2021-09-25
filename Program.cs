using Service.DataBase;

namespace Service
{
    using Service.Api;
    using Service.Api.Module;
    using Service.Transport;

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
            
            var db = new DataBaseFactory();
            var settingsManager = new SettingsManager(db);
            

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

            // TODO: Вынести в настройки.
            var port = 1234;

            var syncModule = new SyncModule(fileManager, settingsManager, connectionStateManager);
            syncModule.Initialize(apiController);

            var attachModule = new AttachmentApi(new AttachmentService(settingsManager, connectionStateManager));
            attachModule.Initialize(apiController);

            var connectionModule = new ConnectionModule(connectionStateManager, settingsManager);
            connectionModule.Initialize(apiController);
            
            // var tcp = new TcpService(jsonManager, ep);
            // tcp.Start();
            // ep = new IPEndPoint(IPAddress.Parse(ipString), port + 1);
            var ws = new WsService(connectionStateManager, apiController, IPAddress.Parse(ipString), port, port++ );
            ws.Start();

            // TODO: Добавить CancellationToken
            Console.ReadKey(true);

            // tcp.Stop();
            ws.Stop();
            return;
        }
    }
}

