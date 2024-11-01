
using Common;

namespace SenderLibrary
{
    /// <summary>
    /// Base sender for encryption times
    /// </summary>
    public class EncryptionBaseSender : BaseSender
    {
        /// <inheritdoc/>
        public EncryptionBaseSender(string serverAddress, int serverPort, string name, List<ProxyInfo> proxyInfos) : base(serverAddress, serverPort, name, proxyInfos)
        {
        }

        /// <inheritdoc/>
        protected override List<ProxyInfo> EncryptListOfProxies(List<ProxyInfo> proxyInfos)
        {
            for (int i = 0; i < proxyInfos.Count; i++)
            {
                if (i == 0)
                {
                    
                }
                else
                {
                    proxyInfos[i].NextAddress = Encryptor.EncryptString(proxyInfos[i - 1].NextPort.ToString(), proxyInfos[i].NextAddress);
                }
            }
            return proxyInfos;
        }
    }
}
