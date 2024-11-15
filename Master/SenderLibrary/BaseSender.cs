using Common;
using System.Globalization;
using System.Net.Sockets;
using System.Text;

namespace SenderLibrary
{
    /// <summary>
    /// Base sender object
    /// </summary>
    public class BaseSender : ISender
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
        /// Tcp client
        /// </summary>
        protected TcpClient _client;

        /// <summary>
        /// Name of sender
        /// </summary>
        private string _name;

        /// <summary>
        /// Stream
        /// </summary>
        protected NetworkStream _stream;

        /// <summary>
        /// CSV writer
        /// </summary>
        protected CSVWriter _writer;

        /// <summary>
        /// Constructor. Sets server address and proxy address
        /// </summary>
        /// <param name="serverAddress">Server address. End connection</param>
        /// <param name="serverPort">Server port. End connection</param>
        /// <param name="proxyAddress">Proxy address. Intermediate connection</param>
        /// <param name="proxyPort">Proxy address. Intermediate connectio</param>
        /// <param name="name">Name of sender</param>
        public BaseSender(string serverAddress, int serverPort, string name, List<ProxyInfo> proxyInfos)
        {
            proxyInfos = new List<ProxyInfo>(proxyInfos);
            _serverAddress = serverAddress;
            _serverPort = serverPort;
            _name = name;
            _writer = new CSVWriter($"Sender{_name}.csv");

            if (proxyInfos.Count == 0)
            {
                _client = new TcpClient(serverAddress, serverPort);
                _stream = _client.GetStream();
                _stream.Write(Encoding.ASCII.GetBytes(name));
                _stream.Flush();
            }
            else
            {
                _client = new TcpClient(proxyInfos[0].NextAddress, proxyInfos[0].NextPort);
                proxyInfos.Add(new ProxyInfo { NextAddress = serverAddress, NextPort = serverPort });

                proxyInfos = EncryptListOfProxies(proxyInfos);

                ProxyObject startMessage = new()
                {
                    NextAddress = proxyInfos[1].NextAddress,
                    NextPort = proxyInfos[1].NextPort,
                    CallerId = _name,
                    NextEndPoints = proxyInfos.Skip(2).ToList(),
                    NrOfNextProxies = proxyInfos.Count - 2
                };

                byte[] messageBytes = startMessage.Serialize();

                _stream = _client.GetStream();
                _stream.Write(messageBytes, 0, messageBytes.Length);
                _stream.Flush();
            }
        }

        /// <summary>
        /// Encrypts proxy infos
        /// </summary>
        /// <param name="proxyInfos">List of proxy infos</param>
        /// <returns>Encrypted list</returns>
        protected virtual List<ProxyInfo> EncryptListOfProxies(List<ProxyInfo> proxyInfos)
        {
            return proxyInfos;
        }

        /// <inheritdoc/>
        public void SendRandomSizeData()
        {
            byte[] data = new byte[1024 + new Random().Next(5000)];
            SendData(data);
        }

        /// <inheritdoc/>
        public void SendTotalAmountOfData(int dataSize, int totalData)
        {
            int sentdata = 0;
            int i = 0;
            int percentage = 0;

            while (sentdata < totalData)
            {
                i++;
                sentdata += dataSize;
                SendSpecificSizeData(dataSize);
                double currentPercentage = ((sentdata * 1.0d) / (totalData * 1.0d)) * 10.0d;
                if ((int)currentPercentage > percentage)
                {
                    Console.WriteLine($"{_name} sent {currentPercentage * 10.0d}% of data");
                    percentage = (int)currentPercentage;
                }
                Thread.Sleep(50);
            }

            Console.WriteLine($"{_name} successfully sent all data to {_serverAddress}:{_serverPort}");
        }

        /// <inheritdoc/>
        public void SendSpecificSizeData(int dataSize)
        {
            byte[] data = new byte[dataSize];
            SendData(data);
        }

        /// <inheritdoc/>
        public void Stop()
        {
            _client.Close();
            _writer.Close();
        }

        /// <inheritdoc/>
        public void SendData(byte[] data)
        {
            byte[] messageBytes = data;

            //Console.WriteLine($"Send {messageBytes.Length} bytes to proxy {_proxyAddress}:{_proxyPort} to server {_serverAddress}:{_serverPort}");

            _stream = _client.GetStream();
            _stream.Write(messageBytes, 0, messageBytes.Length);
            _stream.Flush();

            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            _writer.WriteData(timestamp, messageBytes.Length.ToString() , "send");

            //Console.WriteLine($"Successfully sent {messageBytes.Length} bytes to proxy to server {_serverAddress}:{_serverPort}");

            WaitResponse(messageBytes.Length);
        }

        /// <summary>
        /// Wait for response
        /// </summary>
        /// <param name="messageLength">Message length</param>
        protected virtual void WaitResponse(int messageLength)
        {
            return;
        }
    }
}
