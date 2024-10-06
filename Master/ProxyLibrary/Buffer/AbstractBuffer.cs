
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
        private List<ProxyObject> _lastMessages = new();

        /// <summary>
        /// Constructor. Set tcp client to receive data
        /// </summary>
        /// <param name="client">Tcp client</param>
        public AbstractBuffer(TcpClient client)
        {
            _client = client;
        }

        /// <inheritdoc/>
        public abstract void PrepareData();

        /// <inheritdoc/>
        public void ReadData()
        {
            NetworkStream stream = _client.GetStream();

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);

            ProxyObject message = ProxyObject.Desserialize(buffer);

            _lastMessages.Add(message);
        }

        /// <inheritdoc/>
        public abstract void SendData();
    }
}
