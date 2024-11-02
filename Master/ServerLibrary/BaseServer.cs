using System.Net;
using System.Net.Sockets;

namespace ServerLibrary
{
    /// <summary>
    /// Basic server
    /// </summary>
    public class BaseServer
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
        /// Buffers of next step
        /// </summary>
        List<ServerConnectionHandler> _handlers = new();

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
        /// Flag if to send back values
        /// </summary>
        private bool _returnValues;

        /// <summary>
        /// Constructor. Sets address and port to listen to
        /// </summary>
        /// <param name="address">Address to run</param>
        /// <param name="port">Port</param>
        /// <param name="returnValues">True if send back balues</param>
        public BaseServer(string address, int port, bool returnValues)
        {
            _address = address;
            _port = port;
            _returnValues = returnValues;
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

            List<Task> closeBuffersTasks = new();

            _handlers.ForEach(x => closeBuffersTasks.Add(new Task(() => { x.Stop(); })));

            closeBuffersTasks.ForEach(x => x.Start());

            Task.WaitAll(closeBuffersTasks.ToArray());
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

            _serverThread = new Thread(() => { ServerLoop(); });
            _serverThread.Start();
            Console.WriteLine($"Started server {_address}:{_port}");
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

                _handlers.Add(_returnValues ? new ServerConnectionHandlerWithReturn(client) : new ServerConnectionHandler(client));
                Console.WriteLine($"Client connected");
            }
        }
    }
}
