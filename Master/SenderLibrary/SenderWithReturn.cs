
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
            while (waitRead)
            {
                while (_stream.DataAvailable)
                {
                    byte[] buffer = Array.Empty<byte>();
                    int read = 0;
                    while (_stream.DataAvailable)
                    {
                        byte[] buffer2 = new byte[1024];
                        read += _stream.Read(buffer2, 0, 1024);
                        buffer = buffer.Concat(buffer2).ToArray();
                    }

                    buffer = buffer.Take(read).ToArray();

                    if (buffer.Length == messageLength)
                    {
                        string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        _writer.WriteData(timestamp, buffer.Length.ToString(), "returned");
                        waitRead = false;
                        Console.WriteLine($"Received back data {buffer.Length}");
                    }

                    Thread.Sleep(2);
                }
            }
        }
    }
}
