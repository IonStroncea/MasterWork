﻿
using Common;
using System.Net.Sockets;

namespace ProxyLibrary.Buffer
{
    /// <summary>
    /// Buffer that sends data segmented
    /// </summary>
    public class SegmentedBuffer : AbstractBuffer
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
        public SegmentedBuffer(TcpClient client, int packetSize) : base(client)
        {
            _packetSize = packetSize;
        }


        /// <imheritdoc/>
        public override List<ProxyObject> PrepareData()
        {
            List<ProxyObject> resultList = new();

            for (int i = 0; i < _objectToSend.Data.Length; i += _packetSize)
            {
                byte[] data = new byte[_packetSize];
                if (i + _packetSize <= _objectToSend.Data.Length)
                {
                    Array.Copy(_objectToSend.Data, i, data, 0, _packetSize);
                }
                else
                {
                    Array.Copy(_objectToSend.Data, i, data, 0, _objectToSend.Data.Length - i);
                }
                resultList.Add(new ProxyObject 
                { 
                    Data = data, 
                    NextAddress = _objectToSend.NextAddress,
                    NextPort = _objectToSend.NextPort
                });
            }

            return [_objectToSend];
        }
    }
}
