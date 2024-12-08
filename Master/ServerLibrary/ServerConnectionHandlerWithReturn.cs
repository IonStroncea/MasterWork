

using System.Net.Sockets;

namespace ServerLibrary
{
    /// <summary>
    /// Server handler with return data
    /// </summary>
    public class ServerConnectionHandlerWithReturn : ServerConnectionHandler
    {
        /// <inheritdoc/>
        public ServerConnectionHandlerWithReturn(TcpClient client) : base(client)
        {
        }

        /// <inheritdoc/>
        protected override void SendBack(byte[] buffer, NetworkStream stream)
        {
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();

            if (_callerId.Contains("_0"))
            {
                //Console.WriteLine($"{_callerId} Sent back data {buffer.Length}");
            }
        }
    }
}
