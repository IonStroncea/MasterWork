using System.Net.Sockets;
using System.Text;

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
        /// Caller id
        /// </summary>
        private string _callerId = string.Empty;

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
                if (_callerId == string.Empty)
                {
                    byte[] bufferString = Array.Empty<byte>();
                    int readString = 0;
                    while (stream.DataAvailable)
                    {
                        bufferString = bufferString.Concat(new byte[1024]).ToArray();
                        readString += stream.Read(bufferString, 0, bufferString.Length);
                    }

                    bufferString = bufferString.Take(readString).ToArray();
                    _callerId = Encoding.ASCII.GetString(bufferString);
                    return;
                }

                byte[] buffer = Array.Empty<byte>();
                int read = 0;
                while (stream.DataAvailable)
                {
                    buffer = buffer.Concat(new byte[1024]).ToArray();
                    read+=stream.Read(buffer, 0, buffer.Length);
                }

                Console.WriteLine($"Received data from {_callerId} of size {read} bytes");
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
