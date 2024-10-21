using System.Net.Sockets;

namespace ServerLibrary
{
    /// <summary>
    /// Handles on connection
    /// </summary>
    public class ServerConnectionHandler
    {
        /// <summary>
        /// Tcp client used for connection
        /// </summary>
        private TcpClient _client;

        /// <summary>
        /// Handler thread.
        /// </summary>
        private Thread _handlerThread;

        /// <summary>
        /// Disposed flag
        /// </summary>
        private volatile bool _disposed = false;

        /// <summary>
        /// Constructor. Set tcp client to receive data
        /// </summary>
        /// <param name="client">Tcp client</param>
        public ServerConnectionHandler(TcpClient client)
        {
            _client = client;

            _handlerThread = new Thread(() => 
            {
                while (!_disposed)
                {
                    ReadData();
                    Thread.Sleep(1);
                }
            });

            _handlerThread.Start();

        }

        /// <summary>
        /// Reads data from connection
        /// </summary>
        public void ReadData()
        {
            NetworkStream stream = _client.GetStream();
            if (stream.DataAvailable)
            {
                byte[] buffer = Array.Empty<byte>();

                while (stream.DataAvailable)
                {
                    buffer = buffer.Concat(new byte[1024]).ToArray();
                    stream.Read(buffer, 0, buffer.Length);
                }

                Console.WriteLine($"Received data from {_client.Client.LocalEndPoint} of size {buffer.Length} bytes");
            }
        }

        /// <summary>
        /// Stops handler
        /// </summary>
        public void Stop()
        {
            _disposed = true;
            _client.Close();
            _client.Dispose();
        }
    }
}
