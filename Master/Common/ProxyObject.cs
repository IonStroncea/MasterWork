using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Linq;

namespace Common
{
    /// <summary>
    /// Object that is sent to proxy
    /// </summary>
    public class ProxyObject
    {
        /// <summary>
        /// Next hop address
        /// </summary>
        public string NextAddress { get; set; } = string.Empty;

        /// <summary>
        /// Next hop port
        /// </summary>
        public int NextPort { get; set; }

        /// <summary>
        /// Caller/sender id
        /// </summary>
        public string CallerId { get; set; } = string.Empty;

        /// <summary>
        /// Nr of next proxyes
        /// </summary>
        public int NrOfNextProxies { get; set; }

        /// <summary>
        /// List of next ned points
        /// </summary>
        public List<ProxyInfo> NextEndPoints { get; set; }

        /// <summary>
        /// Serialize object
        /// </summary>
        /// <returns></returns>
        public byte[] Serialize()
        {
            using (MemoryStream m = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(m))
                {
                    writer.Write(NextAddress);
                    writer.Write(CallerId);
                    writer.Write(NextPort);
                    writer.Write(NrOfNextProxies);
                    for (int i = 0; i < NrOfNextProxies; i++)
                    {
                        writer.Write(NextEndPoints[i].NextAddress);
                        writer.Write(NextEndPoints[i].NextPort);
                    }
                }
                return m.ToArray();
            }
        }

        /// <summary>
        /// Desirialize object
        /// </summary>
        /// <param name="data">Serialized object</param>
        /// <returns>Object</returns>
        public static ProxyObject Desserialize(byte[] data)
        {
            ProxyObject result = new ProxyObject();
            using (MemoryStream m = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(m))
                {
                    result.NextAddress = reader.ReadString();
                    result.CallerId = reader.ReadString();
                    result.NextPort = reader.ReadInt32();
                    result.NrOfNextProxies = reader.ReadInt32();
                    result.NextEndPoints = new();
                    for (int i = 0; i < result.NrOfNextProxies; i++)
                    {
                        ProxyInfo info = new();

                        info.NextAddress = reader.ReadString();
                        info.NextPort = reader.ReadInt32();

                        result.NextEndPoints.Add(info);
                    }
                }
            }
            return result;
        }
    }
}
