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
        public DefaultBuffer(TcpClient client, string id) : base(client, id)
        {
        }


        /// <inheritdoc/>
        public override List<ProxyData> PrepareData()
        {
            return [_objectToSend];
        }
    }
}
