
using System.Collections.Concurrent;
using System.Net.Sockets;
using Common;

namespace ProxyLibrary.Buffer
{
    /// <summary>
    /// Base buffer
    /// </summary>
    public abstract class AbstractBuffer : IBuffer
    {
        /// <summary>
        /// Tcp client used for connection
        /// </summary>
        private TcpClient _client; 

        /// <summary>
        /// Buffer with last messages
        /// </summary>
        private ConcurrentQueue<ProxyData> _lastMessages = new();

        /// <summary>
        /// Tokens to send
        /// </summary>
        private int _tokens = 100;

        /// <summary>
        /// Object to sned next
        /// </summary>
        protected ProxyData? _objectToSend = null;

        /// <summary>
        /// Sender
        /// </summary>
        public ProxySender? Sender { get; set; } = null;

        /// <summary>
        /// Buffer thread.
        /// </summary>
        private Thread _bufferThread;

        /// <summary>
        /// Disposed flag
        /// </summary>
        private volatile bool _disposed = false;

        /// <summary>
        /// Caller id
        /// </summary>
        private string _callerId = string.Empty;

        /// <summary>
        /// App id
        /// </summary>
        protected string _appId;

        /// <summary>
        /// Constructor. Set tcp client to receive data
        /// </summary>
        /// <param name="client">Tcp client</param>
        /// <param name="id">app id</param>
        /// <param name="sender">Sender to use</param>
        public AbstractBuffer(TcpClient client, string id, ProxySender? sender = null)
        {
            _client = client;
            _appId = id;
            Sender = sender;

            
            _bufferThread = new Thread(() =>
            {
                Console.WriteLine($"Start listening");
                if (Sender != null)
                {
                    Thread.Sleep(100);
                }

                while (!_disposed)
                {
                    ReadData();
                    Thread.Sleep(10);
                }
            });

            _bufferThread.Start();
        }

        /// <inheritdoc/>
        public abstract List<ProxyData> PrepareData();

        /// <inheritdoc/>
        public void ReadData()
        {
            if (_client.Available > 0)
            {
                int availableData = _client.Available;
                byte[] buffer = new byte[availableData];
                NetworkStream stream = _client.GetStream();
                
                stream.Read(buffer, 0, availableData);

                buffer = buffer.ToArray();

                if (buffer.Length > 0)
                {
                    if (Sender == null)
                    {
                        ProxyObject message = ProxyObject.Desserialize(buffer);
                        _callerId = message.CallerId;

                        Sender = new ProxySender(message, _appId);
                        //Sender = new EncryptionProxySender(message, _appId);
                        
                        Console.WriteLine($"Created sender to server {message.NextAddress} {message.NextPort}");
                    }
                    else
                    {
                        ProxyData message = new ProxyData { Data = buffer};
                        //Console.WriteLine($"Received data of size {buffer.Length} bytes from {_callerId}");
                        _lastMessages.Enqueue(message);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void SendData(int tokens)
        {
            _tokens += tokens;

            bool hasValue = true;
            if (_objectToSend == null)
            {
                hasValue = _lastMessages.TryDequeue(out _objectToSend);
            }

            if (!hasValue)
            {
                return;
            }

            //Console.WriteLine(_tokens + "-" + _objectToSend.Data.Length);
            if (_tokens >= _objectToSend.Data.Length)
            {
                _tokens -= _objectToSend.Data.Length;

                List<ProxyData> proxyObjects = PrepareData();

                proxyObjects.ForEach(x => Sender.SendData(x.Data));

                _objectToSend = null;
            } 
        }

        /// <inheritdoc/>
        public void Stop()
        {
            _disposed = true;

            if (Sender != null)
            {
                Sender.Close();
            }

            _client.Close();
            _client.Dispose();
        }
    }
}
