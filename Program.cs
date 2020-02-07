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
            var fileManager = new FileManager();
            var jsonManager = new ApiController();

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

            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ipString), port);

            var suncModule = new SyncModule(jsonManager, fileManager);
            var infoModule = new InfoModule(jsonManager, fileManager);

            var tcp = new TcpService(jsonManager, ep);
            tcp.Start();
            ep = new IPEndPoint(IPAddress.Parse(ipString), port + 1);
            var ws = new WsService(jsonManager, ep);
            ws.Start();

            // TODO: Добавить CancellationToken
            Console.ReadKey(true);

            tcp.Stop();
            ws.Stop();
            return;
        }
    }
}

