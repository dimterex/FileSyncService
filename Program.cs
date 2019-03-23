using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Service.Api;

namespace Service
{
    class Program
    {
        public static TcpClient client;
        private static TcpListener listener;
        private static string ipString;
        private static FileManager _fileManager;

        private static JsonManager _jsonManager;

        static void Main(string[] args)
        {
            _fileManager = new FileManager();
            
            _jsonManager = new JsonManager(_fileManager);

            IPAddress[] localIp = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress address in localIp)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipString = address.ToString();
                    break;
                }
            }
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ipString), 1234);
            listener = new TcpListener(ep);
            listener.Start();
            Console.WriteLine(@"  
            ===================================================  
                   Started listening requests at: {0}:{1}  
            ===================================================",
            ep.Address, ep.Port);
            client = listener.AcceptTcpClient();
            Console.WriteLine("Connected to client!" + " \n");
            while (client.Connected)
            {
                try
                {
                    const int bytesize = 1024 * 1024;
                    byte[] buffer = getData(client);

                    string data = Encoding.UTF8.GetString(buffer);

                    _jsonManager.Execute(data, client);
                }
                catch (Exception exc)
                {
                    client.Dispose();
                    client.Close();
                }
            }
        }

        public static byte[] getData(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] fileSizeBytes = new byte[4];
            int bytes = stream.Read(fileSizeBytes, 0, fileSizeBytes.Length);
            int dataLength = BitConverter.ToInt32(fileSizeBytes, 0);

            int bytesLeft = dataLength;
            byte[] data = new byte[dataLength];

            int buffersize = 1024;
            int bytesRead = 0;

            while (bytesLeft > 0)
            {
                int curDataSize = System.Math.Min(buffersize, bytesLeft);
                if (client.Available < curDataSize)
                    curDataSize = client.Available;//This save me

                bytes = stream.Read(data, bytesRead, curDataSize);
                bytesRead += curDataSize;
                bytesLeft -= curDataSize;
            }
            return data;
        }
    }
}

