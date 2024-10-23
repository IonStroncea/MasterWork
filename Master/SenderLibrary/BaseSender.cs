using Common;
using System.Globalization;
using System.Net.Sockets;

namespace SenderLibrary
{
    /// <summary>
    /// Base sender object
    /// </summary>
    public class BaseSender
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
        /// Proxy address
        /// </summary>
        private string _proxyAddress = string.Empty;

        /// <summary>
        /// Proxy port
        /// </summary>
        private int _proxyPort;

        /// <summary>
        /// Tcp client
        /// </summary>
        private TcpClient _client;

        /// <summary>
        /// Name of sender
        /// </summary>
        private string _name;

        /// <summary>
        /// Stream
        /// </summary>
        private NetworkStream _stream;

        /// <summary>
        /// CSV writer
        /// </summary>
        private CSVWriter _writer;

        /// <summary>
        /// Constructor. Sets server address and proxy address
        /// </summary>
        /// <param name="serverAddress">Server address. End connection</param>
        /// <param name="serverPort">Server port. End connection</param>
        /// <param name="proxyAddress">Proxy address. Intermediate connection</param>
        /// <param name="proxyPort">Proxy address. Intermediate connectio</param>
        /// <param name="name">Name of sender</param>
        public BaseSender(string serverAddress, int serverPort, string proxyAddress, int proxyPort, string name)
        {
            _serverAddress = serverAddress;
            _serverPort = serverPort;
            _proxyAddress = proxyAddress;
            _proxyPort = proxyPort;
            _name = name;
            _writer = new CSVWriter($"Sender{_name}.csv");

            _client = new TcpClient(_proxyAddress, _proxyPort);

            ProxyObject startMessage = new() { NextAddress = serverAddress, NextPort = serverPort, CallerId = _name };
            byte[] messageBytes = startMessage.Serialize();

            _stream = _client.GetStream();
            _stream.Write(messageBytes, 0, messageBytes.Length);
            _stream.Flush();
            Thread.Sleep(1000);
        }

        /// <summary>
        /// Send data of random size to proxy
        /// </summary>
        public void SendRandomSizeData()
        {
            byte[] data = new byte[1024 + new Random().Next(5000)];
            SendData(data);
        }

        /// <summary>
        /// Send X amount of data with packets of Y size
        /// </summary>
        /// <param name="dataSize">Packet size</param>
        /// <param name="totalData">Total data</param>
        public void SendTotalAmountOfData(int dataSize, int totalData)
        {
            int sentdata = 0;
            int i = 0;

            while (sentdata < totalData)
            {
                i++;
                sentdata += dataSize;
                SendSpecificSizeData(dataSize);
                if (i % 100 == 0)
                {
                    Console.WriteLine($"Sent {sentdata}/{totalData} to {_serverAddress}:{_serverPort}");
                }
                Thread.Sleep(5);
            }

            Console.WriteLine($"Successfully sent all data to {_serverAddress}:{_serverPort}");
        }

        /// <summary>
        /// Send data of specific size to proxy
        /// </summary>
        /// <param name="dataSize">DataSize</param>
        public void SendSpecificSizeData(int dataSize)
        {
            byte[] data = new byte[dataSize];
            SendData(data);
        }

        public void Stop()
        {
            _client.Close();
            _writer.Close();
        }

        /// <summary>
        /// Send data to proxy
        /// </summary>
        /// <param name="data">Data</param>
        public void SendData(byte[] data)
        {
            byte[] messageBytes = data;

            //Console.WriteLine($"Send {messageBytes.Length} bytes to proxy {_proxyAddress}:{_proxyPort} to server {_serverAddress}:{_serverPort}");

            _stream = _client.GetStream();
            _stream.Write(messageBytes, 0, messageBytes.Length);
            _stream.Flush();

            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            _writer.WriteData(timestamp, messageBytes.Length.ToString());
            

            //Console.WriteLine($"Successfully sent {messageBytes.Length} bytes to proxy {_proxyAddress}:{_proxyPort} to server {_serverAddress}:{_serverPort}");
        }
    }
}
