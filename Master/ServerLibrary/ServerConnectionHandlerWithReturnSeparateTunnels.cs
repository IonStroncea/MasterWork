
using Common;
using System.Globalization;
using System.Net.Sockets;
using System.Text;

namespace ServerLibrary
{
    /// <summary>
    /// Connectionhandler for separate tunnels
    /// </summary>
    public class ServerConnectionHandlerWithReturnSeparateTunnels : ServerConnectionHandlerWithReturn
    {
        /// <summary>
        /// Return tcp client
        /// </summary>
        private TcpClient? _returnClient;

        /// <inheritdoc/>
        public ServerConnectionHandlerWithReturnSeparateTunnels(TcpClient client) : base(client)
        {
        }

        /// <inheritdoc/>
        public override void ReadData()
        {
            int avalableData = _client.Available;
            if (avalableData > 0)
            {
                NetworkStream stream = _client.GetStream();
                if (_callerId == string.Empty)
                {
                    byte[] bufferString = new byte[avalableData];

                    stream.Read(bufferString, 0, bufferString.Length);
                    ProxyObject message = ProxyObject.Desserialize(bufferString);
                    _callerId = message.CallerId;

                    _returnClient = new TcpClient(message.NextEndPoints[0].NextAddress, message.NextPort);
                    message.NextEndPoints = message.NextEndPoints.Skip(1).ToList();
                    message.NrOfNextProxies = message.NextEndPoints.Count;
                    NetworkStream sendStrem = _returnClient.GetStream();
                    sendStrem.Write(message.Serialize());

                    if (_callerId.Contains("_0"))
                    {
                        _writer = new CSVWriter($"Server{_callerId}.csv");
                        string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        //_writer.WriteData(timestamp, "Created connection");
                        Console.WriteLine($"{_callerId} arrived to server");
                    }
                    return;
                }

                byte[] buffer = new byte[avalableData];
                stream.Read(buffer, 0, avalableData);

                if (_writer != null)
                {
                    string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    _writer.WriteData(timestamp, avalableData.ToString());
                    if (_callerId.Contains("_0", StringComparison.CurrentCultureIgnoreCase))
                    {
                        //Console.WriteLine(timestamp);
                        //Console.WriteLine($"Received {avalableData} bytes of data from {_callerId}");
                    }
                    //Console.WriteLine($"Received {avalableData} bytes of data from {_callerId}");
                }

                SendBack(buffer, _returnClient.GetStream());
            }
        }
    }
}
