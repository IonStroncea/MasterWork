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
        public string NextAddress = string.Empty;

        /// <summary>
        /// Next hop port
        /// </summary>
        public int NextPort;

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
                    writer.Write(NextPort);
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
                    result.NextPort = reader.ReadInt32();
                }
            }
            return result;
        }
    }
}
