
namespace Common
{
    /// <summary>
    /// Info for proxy(next)
    /// </summary>
    public class ProxyInfo
    {
        /// <summary>
        /// Next hop address
        /// </summary>
        public string NextAddress { get; set; } = string.Empty;

        /// <summary>
        /// Next hop port
        /// </summary>
        public int NextPort { get; set; }
    }
}
