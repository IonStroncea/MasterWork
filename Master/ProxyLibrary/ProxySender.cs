
using Common;
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
        /// Object with info for next proxies
        /// </summary>
        private ProxyObject _proxyObject;

        /// <summary>
        /// Constructor. Sets server address and proxy address
        /// </summary>
        /// <param name="proxyObject">Proxy object</param>
        public ProxySender(ProxyObject proxyObject)
        {
            _serverAddress = proxyObject.NextAddress;
            _serverPort = proxyObject.NextPort;
            _callerId = proxyObject.CallerId;
            _proxyObject = proxyObject;
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
                if (_proxyObject.NrOfNextProxies == 0)
                {
                    networkStream.Write(Encoding.ASCII.GetBytes(_callerId));
                    networkStream.Flush();
                }
                else 
                {
                    ProxyObject proxyObject = new ProxyObject 
                    { 
                        NextAddress = _proxyObject.NextEndPoints[0].NextAddress,
                        NextPort = _proxyObject.NextEndPoints[0].NextPort,
                        CallerId = _proxyObject.CallerId,
                        NrOfNextProxies = _proxyObject.NrOfNextProxies-1,
                        NextEndPoints = _proxyObject.NextEndPoints.Skip(1).ToList()
                    };

                    networkStream.Write(proxyObject.Serialize());
                    networkStream.Flush();
                }
                Thread.Sleep(1000);
            }

            //Console.WriteLine($"Send {data.Length} bytes to server {_serverAddress}:{_serverPort}");

            NetworkStream stream = tcpClient.GetStream();
            stream.Write(data, 0, data.Length);
            stream.Flush();
            //Console.WriteLine($"Succesfully sent {data.Length} bytes to server {_serverAddress}:{_serverPort}");
        }
    }
}
