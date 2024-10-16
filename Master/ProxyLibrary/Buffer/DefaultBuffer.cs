using Common;
using System.Net.Sockets;

namespace ProxyLibrary.Buffer
{
    /// <summary>
    /// Default buffer
    /// </summary>
    public class DefaultBuffer : AbstractBuffer
    {
        /// <inheritdoc/>
        public DefaultBuffer(TcpClient client) : base(client)
        {
        }

        /// <inheritdoc/>
        public override List<ProxyObject> PrepareData()
        {
            return [_objectToSend];
        }
    }
}
