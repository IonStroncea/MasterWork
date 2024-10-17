using Common;
using System.Net.Sockets;

namespace SenderLibrary
{
    /// <summary>
    /// Base sender object
    /// </summary>
    public class BaseSender
    {
        /// <summary>
        /// End server address
        /// </summary>
        private string _serverAddress = string.Empty;

        /// <summary>
        /// End server port
        /// </summary>
        private int _serverPort;

        /// <summary>
        /// Proxy address
        /// </summary>
        private string _proxyAddress = string.Empty;

        /// <summary>
        /// Proxy port
        /// </summary>
        private int _proxyPort;

        /// <summary>
        /// Tcp client
        /// </summary>
        private TcpClient _client;

        /// <summary>
        /// Constructor. Sets server address and proxy address
        /// </summary>
        /// <param name="serverAddress">Server address. End connection</param>
        /// <param name="serverPort">Server port. End connection</param>
        /// <param name="proxyAddress">Proxy address. Intermediate connection</param>
        /// <param name="proxyPort">Proxy address. Intermediate connectio</param>
        public BaseSender(string serverAddress, int serverPort, string proxyAddress, int proxyPort)
        {
            _serverAddress = serverAddress;
            _serverPort = serverPort;
            _proxyAddress = proxyAddress;
            _proxyPort = proxyPort;

            _client = new TcpClient(_proxyAddress, _proxyPort);
        }

        /// <summary>
        /// Send data of random size to proxy
        /// </summary>
        public void SendRandomSizeData()
        {
            byte[] data = new byte[new Random().Next()];
            SendData(data);
        }

        /// <summary>
        /// Send data of specific size to proxy
        /// </summary>
        /// <param name="dataSize">DataSize</param>
        public void SendSpecificSizeData(int dataSize)
        {
            byte[] data = new byte[dataSize];
            SendData(data);
        }

        public void Stop()
        {
            _client.Close();
        }

        /// <summary>
        /// Send data to proxy
        /// </summary>
        /// <param name="data">Data</param>
        public void SendData(byte[] data)
        {
            ProxyObject sendObject = new ();

            sendObject.Data = data;
            sendObject.NextAddress = _serverAddress;
            sendObject.NextPort = _serverPort;

            byte[] messageBytes = sendObject.Serialize();

            NetworkStream stream = _client.GetStream();
            stream.Write(messageBytes, 0, messageBytes.Length);
        }
    }
}
