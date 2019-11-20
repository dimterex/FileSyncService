namespace Service.Transport
{
    using Newtonsoft.Json;

    using NLog;

    using Service.Api.Message;

    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Text;

    public class TcpCustomClient : IClient
    {
        private const int BUFFER_SIZE = 1024;

        private readonly TcpService _service;

        private readonly Queue<MessageContainer> _mainMessageQueue;

        private readonly ILogger _logger;

        private readonly TcpClient _client;
        private readonly NetworkStream _networkStream;
        private readonly JsonSerializerSettings _settings;

        public bool IsConnected { get; set; }

        public TcpCustomClient(TcpClient client, TcpService tcpService)
        {
            _client = client;
            _networkStream = _client.GetStream();
            _service = tcpService;
            _logger = LogManager.GetCurrentClassLogger();
            _mainMessageQueue = new Queue<MessageContainer>();
            _settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        }

        public void Start()
        {
            _logger.Trace(() => $"Connected: {DateTime.Now}");
            IsConnected = true;
            StartReadData();
        }

        public void SendMessage(MessageContainer messageContainer)
        {
            try
            {
                string serializedMessages = JsonConvert.SerializeObject(messageContainer, _settings);
                SendData(serializedMessages);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex);
                IsConnected = false;
            }
        }

        private void OnMessage(string rawData)
        {
            try
            {
                _service.ApiController.Execute(rawData, this);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex);
            }
        }

        public void Close()
        {
            _client.Close();
            IsConnected = false;
        }

        private void StartReadData()
        {
            byte[] fileSizeBytes = new byte[4];
            int bytes = _networkStream.Read(fileSizeBytes, 0, fileSizeBytes.Length);
            int dataLength = BitConverter.ToInt32(fileSizeBytes, 0);

            int bytesLeft = dataLength;
            byte[] data = new byte[dataLength];

            int buffersize = 1024;
            int bytesRead = 0;

            while (bytesLeft > 0)
            {
                int curDataSize = System.Math.Min(buffersize, bytesLeft);
                if (_client.Available < curDataSize)
                    curDataSize = _client.Available; //This save me

                bytes = _networkStream.Read(data, bytesRead, curDataSize);
                bytesRead += curDataSize;
                bytesLeft -= curDataSize;
            }
            var rawString = Encoding.UTF8.GetString(data);
            OnMessage(rawString);
        }

        private void SendData(string rawString)
        {
            byte[] data = Encoding.UTF8.GetBytes(rawString);
            byte[] dataLength = BitConverter.GetBytes(data.Length);
            _networkStream.Write(dataLength, 0, 4);

            int bytesSent = 0;
            int bytesLeft = data.Length;

            while (bytesLeft > 0)
            {
                int curDataSize = Math.Min(BUFFER_SIZE, bytesLeft);
                _networkStream.Write(data, bytesSent, curDataSize);

                bytesSent += curDataSize;
                bytesLeft -= curDataSize;
            }
        }
    }
}
