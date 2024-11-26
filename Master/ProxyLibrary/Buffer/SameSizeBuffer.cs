
using Common;
using System.Net.Sockets;

namespace ProxyLibrary.Buffer
{
    /// <summary>
    /// Buffer that sends data of same size
    /// </summary>
    public class SameSizeBuffer : AbstractBuffer
    {
        /// <summary>
        /// Size of a message
        /// </summary>
        private int _packetSize;

        /// <summary>
        /// Constructor. Specifies client of connection and packet size
        /// </summary>
        /// <param name="client">Client</param>
        /// <param name="packetSize">Size of each message</param>
        /// <param name="id">app id</param>
        /// <param name="sender">Sender to use</param>
        public SameSizeBuffer(TcpClient client, int packetSize, string id, ProxySender? sender = null) : base(client, id, sender)
        {
            _packetSize = packetSize;
        }


        /// <imheritdoc/>
        public override List<ProxyData> PrepareData(ProxyData message)
        {
            List<ProxyData> resultList = new();

            for (int i = 0; i < message.Data.Length; i += _packetSize)
            {
                byte[] data = new byte[_packetSize];
                if (i + _packetSize <= message.Data.Length)
                {
                    Array.Copy(message.Data, i, data, 0, _packetSize);
                }
                else
                {
                    Array.Copy(message.Data, i, data, 0, message.Data.Length - i);
                }

                resultList.Add(new ProxyData
                {
                    Data = data
                });
            }

            return resultList;
        }
    }
}
