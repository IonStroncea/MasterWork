using Common;
using System.Globalization;
using System.IO;
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
        /// CSV writer
        /// </summary>
        private CSVWriter? _writer;

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
            int avalableData = _client.Available;
            if (avalableData>0)
            {
                NetworkStream stream = _client.GetStream();
                if (_callerId == string.Empty)
                {
                    byte[] bufferString = new byte[avalableData];
                    
                    stream.Read(bufferString, 0, bufferString.Length);
                    _callerId = Encoding.ASCII.GetString(bufferString);

                    if (_callerId.Contains("_0"))
                    {
                        _writer = new CSVWriter($"Server{_callerId}.csv");
                        string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        //_writer.WriteData(timestamp, "Created connection");
                    }
                    return;
                }

                byte[] buffer = new byte[avalableData];
                stream.Read(buffer, 0, avalableData);

                if (_writer != null)
                {
                    string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    _writer.WriteData(timestamp, avalableData.ToString());
                    if (_callerId.Contains("1_0", StringComparison.CurrentCultureIgnoreCase))
                    {
                        //Console.WriteLine(timestamp);
                    }
                    //Console.WriteLine($"Received {avalableData} bytes of data from {_callerId}");
                }

                SendBack(buffer, stream);
            }
        }

        /// <summary>
        /// Send back data
        /// </summary>
        /// <param name="buffer">Data to send</param>
        /// <param name="stream">Stream to send data with</param>
        protected virtual void SendBack(byte[] buffer, NetworkStream stream)
        {
            return;
        }

        /// <summary>
        /// Stops handler
        /// </summary>
        public void Stop()
        {
            _disposed = true;
            _client.Close();
            _client.Dispose();
            if(_writer != null ) 
            {
                _writer.Close();
            }
        }
    }
}
