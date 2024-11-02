
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
        public TcpClient TcpClient { get; set; }

        /// <summary>
        /// Caller id
        /// </summary>
        private string _callerId;

        /// <summary>
        /// Object with info for next proxies
        /// </summary>
        private ProxyObject _proxyObject;

        /// <summary>
        /// App id
        /// </summary>
        protected string _appId;

        /// <summary>
        /// Flag that indicates if this is first call
        /// </summary>
        private bool _firstCall = true;

        /// <summary>
        /// Constructor. Sets server address and proxy address
        /// </summary>
        /// <param name="proxyObject">Proxy object</param>
        /// <param name="id">app id</param>
        /// <param name="tcpClient">Tcp client to use. Null if no needed</param>
        public ProxySender(ProxyObject proxyObject, string id, TcpClient? tcpClient = null)
        {
            _appId = id;
            proxyObject = DecryptProxyObject(proxyObject);
            _serverAddress = proxyObject.NextAddress;
            _serverPort = proxyObject.NextPort;
            _callerId = proxyObject.CallerId;
            _proxyObject = proxyObject;

            if (tcpClient != null)
            {
                TcpClient = tcpClient;
                _firstCall = false;
            }
            else
            {
                TcpClient = new TcpClient(_serverAddress, _serverPort);
            }
        }

        /// <summary>
        /// Decrypts proxy object
        /// </summary>
        /// <param name="message">Message to decrypt</param>
        /// <returns>Decrypted message</returns>
        protected virtual ProxyObject DecryptProxyObject(ProxyObject message)
        {
            return message;
        }

        /// <summary>
        /// Close connection
        /// </summary>
        public void Close()
        {
            if (TcpClient == null)
            {
                return;
            }
            if (TcpClient.Connected)
            {
                TcpClient.Close();
            }
        }

        /// <summary>
        /// Send data to proxy
        /// </summary>
        /// <param name="data">Data</param>
        public void SendData(byte[] data)
        {
            if (_firstCall)
            {
                NetworkStream networkStream = TcpClient.GetStream();
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
                _firstCall = false;
                Thread.Sleep(1000);
            }

            //Console.WriteLine($"Send {data.Length} bytes to server {_serverAddress}:{_serverPort}");

            NetworkStream stream = TcpClient.GetStream();
            stream.Write(data, 0, data.Length);
            stream.Flush();
            //Console.WriteLine($"Succesfully sent {data.Length} bytes to server {_serverAddress}:{_serverPort}");
        }
    }
}
