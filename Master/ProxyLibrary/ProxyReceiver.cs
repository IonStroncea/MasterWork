using System.Net;
using System.Net.Sockets;

namespace ProxyLibrary
{
    /// <summary>
    /// Receives data
    /// </summary>
    public class ProxyReceiver
    {
        /// <summary>
        /// Address to listen
        /// </summary>
        private string _address;

        /// <summary>
        /// Port to listen
        /// </summary>
        private int _port;

        /// <summary>
        /// Listener
        /// </summary>
        private TcpListener? _server = null;

        /// <summary>
        /// Server thread.
        /// </summary>
        private Thread? _serverThread = null;

        /// <summary>
        /// Disposed flag
        /// </summary>
        private volatile bool _disposed = false;

        /// <summary>
        /// Constructor. Sets address and port to listen to
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        public ProxyReceiver(string address, int port)
        {
            _address = address;
            _port = port;
        }

        /// <summary>
        /// Stops server
        /// </summary>
        public void Stop()
        {
            _disposed = true;

            if (_serverThread != null && _serverThread.ThreadState == ThreadState.Running)
            {
                _serverThread.Join();
            }
        }
        
        /// <summary>
        /// Starts server
        /// </summary>
        public void Start()
        {
            if (_server != null)
            {
                return;
            }

            IPAddress address = IPAddress.Parse(_address);
            _server = new TcpListener(address, _port);

            _serverThread = new Thread(() => { ServerLoop();});
            _serverThread.Start();
        }

        /// <summary>
        /// Server loop/logic
        /// </summary>
        private void ServerLoop()
        {
            if (_serverThread == null || _server == null)
            {
                return;
            }

            _server.Server.ReceiveTimeout = 1000;

            while (!_disposed)
            {
                TcpClient client = _server.AcceptTcpClient();

                throw new NotImplementedException();
            }
        }
    }
}
