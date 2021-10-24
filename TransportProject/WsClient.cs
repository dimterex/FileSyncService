using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using NLog;
using SdkProject;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace TransportProject
{
     internal class WsClient : WebSocketBehavior, IClient
    {
        #region Fields

        private readonly WsService _service;

        private readonly object _lockObject;
        private readonly Queue<MessageContainer> _mainMessageQueue;
        private readonly Queue<MessageContainer> _additionalMessageQueue;

        private bool _queueLockMode;
        private ILogger _logger;

        private readonly ConcurrentDictionary<ushort, bool> _listenPorts;


        #endregion Fields

        #region Constructors

       
        public WsClient(WsService service)
        {
            _service = service;

            _lockObject = new object();
            _mainMessageQueue = new Queue<MessageContainer>();
            _additionalMessageQueue = new Queue<MessageContainer>();
            _listenPorts = new ConcurrentDictionary<ushort, bool>();
            _queueLockMode = false;
        }

        #endregion Constructors

        #region Methods

        public void SendResponse(HttpRequestEventArgs e, IMessage response)
        {
            e.Response.SendChunked = true;
            
            MessageContainer messageContainer = _service.Controller.SerializePacket(response);
            
            using (var streamWriter = new StreamWriter(e.Response.OutputStream, Encoding.UTF8))
            {
                
                var serializer = JsonSerializer.Create(new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                var jsonWriter = new JsonTextWriter(streamWriter);

                try
                {
                    serializer.Serialize(jsonWriter, new [] { messageContainer });
                }
                finally
                {
                    jsonWriter.Close();
                }
            }
        }

        public void InitializeRawData(IList<ushort> listenPorts)
        {
            _listenPorts.Clear();
            foreach (var port in listenPorts)
                _listenPorts[port] = true;
        }

        public bool IsConfiguredPort(ushort port)
        {
            return _listenPorts.ContainsKey(port);
        }

        
        public void TimeTick()
        {
            SendQueue();
        }

        public void LockQueue()
        {
            lock (_lockObject)
            {
                _queueLockMode = true;
            }
        }

        public void UnlockQueue()
        {
            lock (_lockObject)
            {
                _queueLockMode = false;
                while (_additionalMessageQueue.Count > 0)
                {
                    var messageContainer = _additionalMessageQueue.Dequeue();
                    _mainMessageQueue.Enqueue(messageContainer);
                }
            }

            SendQueue();
        }

        private void SendMessage(IMessage message, bool broadcast)
        {
            try
            {
                MessageContainer messageContainer = _service.Controller.SerializePacket(message);

                lock (_lockObject)
                {
                    if (!_queueLockMode)
                    {
                        _mainMessageQueue.Enqueue(messageContainer);
                    }
                    else
                    {
                        _additionalMessageQueue.Enqueue(messageContainer);
                    }
                }

                SendQueue();
            }
            catch (Exception ex)
            {
                if (!broadcast)
                    throw;
                _logger.Warn( () =>  $"{ex}");
            }
        }


        protected override void OnOpen()
        {
            _logger = LogManager.GetCurrentClassLogger();
            _logger.Debug(() => $"opened connection: {ID}");

            _service.Authorize(this);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            _logger.Debug(() => $"closed connection: {ID}");

            _service.Remove(this);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.IsText)
            {
                _logger?.Trace(() => $"message received: {e.Data}");

                try
                {
                    var messages = JsonConvert.DeserializeObject<MessageContainer[]>(e.Data);
                    foreach (var message in messages)
                    {
                        _service.Controller.HandleMessage(this, message);
                    }

                }
                catch (Exception ex)
                {
                    _logger?.Warn(ex);
                }
            }
        }

        private void SendQueue()
        {
            MessageContainer[] messages;

            lock (_lockObject)
            {
                if (_mainMessageQueue.Count == 0 || _queueLockMode)
                    return;

                messages = _mainMessageQueue.ToArray();
                _mainMessageQueue.Clear();
            }

            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            string serializedMessages = JsonConvert.SerializeObject(messages, settings);
            Send(serializedMessages);
            _logger?.Trace(() => $"message sent: {serializedMessages}");
        }

        public void Close()
        {
            Context.WebSocket.Close();
        }

        #endregion Methods
    }
}