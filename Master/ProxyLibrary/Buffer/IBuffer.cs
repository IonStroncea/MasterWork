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
        public void SendData();

        /// <summary>
        /// Prepares data
        /// </summary>
        public void PrepareData();
    }
}
