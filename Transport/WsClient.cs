namespace Service.Transport
{
    using Newtonsoft.Json;

    using NLog;

    using Service.Api.Message;

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using WebSocketSharp;
    using WebSocketSharp.Server;

    public class WsClient : WebSocketBehavior, IClient
    {
        #region Fields

        private readonly WsService _service;

        private readonly object _lockObject;
        private readonly Queue<MessageContainer> _mainMessageQueue;
        private readonly Queue<MessageContainer> _additionalMessageQueue;

        private readonly ILogger _logger;

        public readonly ConcurrentDictionary<ushort, bool> _listenPorts;

        #endregion Fields

        public bool IsConnected { get; set; }

        #region Constructors

        public WsClient(WsService service)
        {
            _service = service;
            _logger = LogManager.GetCurrentClassLogger();
            _lockObject = new object();
            _mainMessageQueue = new Queue<MessageContainer>();
            _additionalMessageQueue = new Queue<MessageContainer>();
            _listenPorts = new ConcurrentDictionary<ushort, bool>();
        }

        #endregion Constructors

        #region Methods

        public void Initialize()
        {
            _service.Authorize(this);
        }


        internal void Send(FileInfo fileInfo)
        {
            Send(fileInfo);
        }

        public void SendMessage(MessageContainer messageContainer)
        {
            try
            {

                lock (_lockObject)
                {
                    var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                    string serializedMessages = JsonConvert.SerializeObject(messageContainer, settings);

                    Send(serializedMessages);

                    //if (!_queueLockMode)
                    //    _mainMessageQueue.Enqueue(messageContainer);
                    //else
                    //    _additionalMessageQueue.Enqueue(messageContainer);
                }

                //SendQueue();
            }
            catch (Exception ex)
            {
                _logger.Warn(ex);
                IsConnected = false;
            }
        }

        protected override void OnOpen()
        {
            _logger.Debug(() => $"opened connection: {ID}");
            IsConnected = true;
            _service.Authorize(this);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            _logger.Debug(() => $"closed connection: {ID}");
            _service.Remove(this);
            IsConnected = false;
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (!e.IsText)
                return;
            
            try
            {
                if (e.Data == "test")
                {
                    _service.ApiController.Execute("[{\"Type\":\"SyncFilesRequest\",\"Value\":{\"files\":[]}}]", this);
                    return;
                }
                _service.ApiController.Execute(e.Data, this);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex);
            }
        }

        private void SendQueue()
        {
            MessageContainer[] messages;

            lock (_lockObject)
            {
                if (_mainMessageQueue.Count == 0)
                    return;

                messages = _mainMessageQueue.ToArray();
                _mainMessageQueue.Clear();

            }

            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            string serializedMessages = JsonConvert.SerializeObject(messages, settings);

            Send(serializedMessages);
            //SendData(serializedMessages);
        }

        private void SendData(string rawString)
        {
            byte[] data = Encoding.UTF8.GetBytes(rawString);
            byte[] dataLength = BitConverter.GetBytes(data.Length);

            var datas = new byte[4];
            Buffer.BlockCopy(dataLength, 0, datas, 0, 4);

            Send(datas);

            int bytesSent = 0;
            int bytesLeft = data.Length;

            while (bytesLeft > 0)
            {
                int curDataSize = Math.Min(1024, bytesLeft);
                datas = new byte[curDataSize];
                Buffer.BlockCopy(data, bytesSent, datas, 0, curDataSize);
                Send(datas);

                bytesSent += curDataSize;
                bytesLeft -= curDataSize;
            }
        }

        public void Close()
        {
            Context.WebSocket.Close();
        }

        #endregion Methods
    }
}
