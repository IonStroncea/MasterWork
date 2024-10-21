﻿using Common;
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
        public override List<ProxyData> PrepareData()
        {
            return [_objectToSend];
        }
    }
}
