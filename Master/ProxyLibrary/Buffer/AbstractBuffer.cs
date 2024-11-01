
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
        private int _tokens = 50000;

        /// <summary>
        /// Object to sned next
        /// </summary>
        protected ProxyData? _objectToSend = null;

        /// <summary>
        /// Sender
        /// </summary>
        private ProxySender? _sender = null;

        /// <summary>
        /// Buffer thread.
        /// </summary>
        private Thread _bufferThread;

        /// <summary>
        /// Disposed flag
        /// </summary>
        private volatile bool _disposed = false;

        /// <summary>
        /// Network stream to read
        /// </summary>
        private NetworkStream _stream;

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
        public AbstractBuffer(TcpClient client, string id)
        {
            _client = client;
            _stream = _client.GetStream();
            _appId = id;


            _bufferThread = new Thread(() =>
            {
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
            if (_stream.DataAvailable)
            {

                byte[] buffer = Array.Empty<byte>();
                int read = 0;
                while (_stream.DataAvailable)
                {
                    buffer = buffer.Concat(new byte[1024]).ToArray();
                    read+= _stream.Read(buffer, 0, buffer.Length);
                }

                buffer = buffer.Take(read).ToArray();

                if (buffer.Length > 0)
                {
                    if (_sender == null)
                    {
                        ProxyObject message = ProxyObject.Desserialize(buffer);
                        _callerId = message.CallerId;

                        //_sender = new ProxySender(message, _appId);
                        _sender = new EncryptionProxySender(message, _appId);
                        
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

            if (_tokens >= _objectToSend.Data.Length)
            {
                _tokens -= _objectToSend.Data.Length;

                List<ProxyData> proxyObjects = PrepareData();

                proxyObjects.ForEach(x => _sender.SendData(x.Data));

                _objectToSend = null;
            } 
        }

        /// <inheritdoc/>
        public void Stop()
        {
            _disposed = true;

            if (_sender != null)
            {
                _sender.Close();
            }

            _client.Close();
            _client.Dispose();
        }
    }
}
