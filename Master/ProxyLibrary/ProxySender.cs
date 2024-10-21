
using System.Net.Sockets;

namespace ProxyLibrary
{
    /// <summary>
    /// Sends data further
    /// </summary>
    public class ProxySender
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
        /// TCP client
        /// </summary>
        private TcpClient? tcpClient;

        /// <summary>
        /// Constructor. Sets server address and proxy address
        /// </summary>
        /// <param name="serverAddress">Server address. End connection</param>
        /// <param name="serverPort">Server port. End connection</param>
        public ProxySender(string serverAddress, int serverPort)
        {
            _serverAddress = serverAddress;
            _serverPort = serverPort;
        }

        /// <summary>
        /// Close connection
        /// </summary>
        public void Close()
        {
            if (tcpClient == null)
            {
                return;
            }
            if (tcpClient.Connected)
            {
                tcpClient.Close();
            }
        }

        /// <summary>
        /// Send data to proxy
        /// </summary>
        /// <param name="data">Data</param>
        public void SendData(byte[] data)
        {
            if (tcpClient == null || !tcpClient.Connected)
            {
                tcpClient = new TcpClient(_serverAddress, _serverPort);
            }

            Console.WriteLine($"Send {data.Length} bytes to server {_serverAddress}:{_serverPort}");

            NetworkStream stream = tcpClient.GetStream();
            stream.Write(data, 0, data.Length);

            Console.WriteLine($"Succesfully sent {data.Length} bytes to server {_serverAddress}:{_serverPort}");
        }
    }
}
