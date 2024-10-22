
using System.Net.Sockets;
using System.Text;

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
        /// Caller id
        /// </summary>
        private string _callerId;

        /// <summary>
        /// Constructor. Sets server address and proxy address
        /// </summary>
        /// <param name="serverAddress">Server address. End connection</param>
        /// <param name="serverPort">Server port. End connection</param>
        public ProxySender(string serverAddress, int serverPort, string callerId)
        {
            _serverAddress = serverAddress;
            _serverPort = serverPort;
            _callerId = callerId;
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
                NetworkStream networkStream = tcpClient.GetStream();
                networkStream.Write(Encoding.ASCII.GetBytes(_callerId));
                networkStream.Flush();
                Thread.Sleep(1000);
            }

            //Console.WriteLine($"Send {data.Length} bytes to server {_serverAddress}:{_serverPort}");

            NetworkStream stream = tcpClient.GetStream();
            stream.Write(data, 0, data.Length);
            stream.Flush();
            Console.WriteLine($"Succesfully sent {data.Length} bytes to server {_serverAddress}:{_serverPort}");
        }
    }
}
