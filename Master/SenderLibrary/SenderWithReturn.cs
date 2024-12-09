
using Common;
using System.Globalization;
using System.IO;

namespace SenderLibrary
{
    /// <summary>
    /// Sender that waits for return before sending new data
    /// </summary>
    public class SenderWithReturn : BaseSender
    {
        /// <inheritdoc/>
        public SenderWithReturn(string serverAddress, int serverPort, string name, List<ProxyInfo> proxyInfos) : base(serverAddress, serverPort, name, proxyInfos)
        {
        }

        /// <inheritdoc/>
        protected override void WaitResponse(int messageLength)
        {
            bool waitRead = true;
            int totalRead = 0;
            while (waitRead)
            {
                int availableData = _client.Available;
                if (availableData > 0)
                {
                    totalRead += availableData;
                    _stream = _client.GetStream();
                    byte[] buffer = new byte[availableData];
                    
                    _stream.Read(buffer, 0, availableData);

                    if (totalRead >= messageLength)
                    {
                        if (_writer != null)
                        {
                            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            _writer.WriteData(timestamp, buffer.Length.ToString(), "returned");
                            Console.WriteLine($"{_name} Received back data {buffer.Length}");
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
