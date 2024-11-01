using Common;

namespace ProxyLibrary
{
    /// <summary>
    /// Sender class for onion encrition
    /// </summary>
    public class EncryptionProxySender : ProxySender
    {
        /// <summary>
        /// Constructor. Sets server address and proxy address
        /// </summary>
        /// <param name="proxyObject">Proxy object</param>
        /// <param name="id">app id</param>
        public EncryptionProxySender(ProxyObject proxyObject, string id) : base(proxyObject, id)
        {
            
        }

        /// <inheritdoc/>
        protected override ProxyObject DecryptProxyObject(ProxyObject message)
        {
            message.NextAddress = Encryptor.DecryptString(_appId, message.NextAddress);
            Console.WriteLine(message.NextAddress);
            return message;
        }
    }
}
