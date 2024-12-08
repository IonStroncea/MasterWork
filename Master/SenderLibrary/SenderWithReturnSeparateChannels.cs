
using Common;
using System.Globalization;
using System.Net;
using System.Net.Sockets;

namespace SenderLibrary
{
    /// <summary>
    /// Sender with return with separate commubnications tunnels
    /// </summary>
    public class SenderWithReturnSeparateChannels : SenderWithReturn
    {
        /// <summary>
        /// Listener
        /// </summary>
        private TcpListener _tcpListener;

        /// <summary>
        /// Client to listen to
        /// </summary>
        private TcpClient? _tcpClient = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverAddress">Server address</param>
        /// <param name="serverPort">Server port</param>
        /// <param name="name">Name</param>
        /// <param name="proxyInfos">Proxyies to go through</param>
        /// <param name="listenAddress">Listen address</param>
        /// <param name="listenPort">Listen port</param>
        public SenderWithReturnSeparateChannels(string serverAddress, int serverPort, string name, List<ProxyInfo> proxyInfos, string listenAddress, int listenPort) : base(serverAddress, serverPort, name, proxyInfos)
        {
            IPAddress address = IPAddress.Parse(listenAddress);
            _tcpListener = new TcpListener(address, listenPort);
            _tcpListener.Start();
        }

        /// <inheritdoc/>
        protected override void WaitResponse(int messageLength)
        {
            if (_tcpClient == null)
            {
                _tcpClient = _tcpListener.AcceptTcpClient();
            }

            bool waitRead = true;
            int totalRead = 0;
            while (waitRead)
            {
                int availableData = _tcpClient.Available;
                if (availableData > 0)
                {
                    totalRead += availableData;
                    _stream = _tcpClient.GetStream();
                    byte[] buffer = new byte[availableData];

                    _stream.Read(buffer, 0, availableData);

                    if (totalRead >= messageLength)
                    {
                        if (_writer != null)
                        {
                            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            _writer.WriteData(timestamp, buffer.Length.ToString(), "returned");
                            //Console.WriteLine($"{_name} Received back data {buffer.Length}");
                        }
                        waitRead = false;
                        //
                    }
                }
                Thread.Sleep(2);
            }
        }
    }
}
