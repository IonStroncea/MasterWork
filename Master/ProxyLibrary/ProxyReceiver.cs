using ProxyLibrary.Buffer;
using ProxyLibrary.Handler;
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
        /// Handler
        /// </summary>
        AbstractHandler _handler;

        /// <summary>
        /// BufferType
        /// </summary>
        private BufferEnum _bufferType;

        /// <summary>
        /// Address to listen
        /// </summary>
        private string _address;

        /// <summary>
        /// Port to listen
        /// </summary>
        private int _port;
        
        /// <summary>
        /// Packet size
        /// </summary>
        private int _packetSize;

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
        /// <param name="address">Address</param>
        /// <param name="port">Port</param>
        /// <param name="bufferType">Buffer type</param>
        /// <param name="packetSize">Size of each message</param>
        ///  <param name="handler">Handles when to send messages</param>
        public ProxyReceiver(string address, int port, BufferEnum bufferType, int packetSize, AbstractHandler handler)
        {
            _address = address;
            _port = port;
            _bufferType = bufferType;
            _packetSize = packetSize;
            _handler = handler;
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

            _handler.Stop();
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
            _server.Start();

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

                if (_bufferType == BufferEnum.SameSize)
                {
                    _handler.AddBuffer(new SameSizeBuffer(client, _packetSize, _port.ToString()));
                }
                else
                {
                    _handler.AddBuffer(new DefaultBuffer(client, _port.ToString()));
                }
            }
        }
    }
}
