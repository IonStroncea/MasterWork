using Common;

namespace ProxyLibrary.Buffer
{
    /// <summary>
    /// Buffer interface. Layer between receiver and sender
    /// </summary>
    public interface IBuffer
    {
        /// <summary>
        /// Reads data from connection buffer
        /// </summary>
        public void ReadData();

        /// <summary>
        /// Sends data to next destination
        /// </summary>
        /// <param name="tokens">Tokens allocated</param>
        public void SendData(int tokens);

        /// <summary>
        /// Prepares data
        /// </summary>
        public List<ProxyData> PrepareData();

        /// <summary>
        /// Stop
        /// </summary>
        public void Stop();
    }
}
