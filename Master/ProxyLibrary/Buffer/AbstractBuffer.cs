
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
        private ConcurrentQueue<ProxyObject> _lastMessages = new();

        /// <summary>
        /// Tokens to send
        /// </summary>
        private int _tokens = 0;

        /// <summary>
        /// Object to sned next
        /// </summary>
        protected ProxyObject? _objectToSend = null;

        /// <summary>
        /// Sender
        /// </summary>
        private ProxySender? _sender = null;

        /// <summary>
        /// Constructor. Set tcp client to receive data
        /// </summary>
        /// <param name="client">Tcp client</param>
        public AbstractBuffer(TcpClient client)
        {
            _client = client;
        }

        /// <inheritdoc/>
        public abstract List<ProxyObject> PrepareData();

        /// <inheritdoc/>
        public void ReadData()
        {
            NetworkStream stream = _client.GetStream();

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);

            ProxyObject message = ProxyObject.Desserialize(buffer);

            _lastMessages.Enqueue(message);
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

                if (_sender == null)
                {
                    _sender = new ProxySender(_objectToSend.NextAddress, _objectToSend.NextPort);
                }

                List<ProxyObject> proxyObjects = PrepareData();

                proxyObjects.ForEach(x => _sender.SendData(x.Serialize()));

                _objectToSend = null;
            } 
        }

        /// <inheritdoc/>
        public void Stop()
        {
            _client.Close();
            _client.Dispose();
        }
    }
}
