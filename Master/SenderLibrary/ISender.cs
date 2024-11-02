namespace SenderLibrary
{
    /// <summary>
    /// Sender interface
    /// </summary>
    public interface ISender
    {
        /// <summary>
        /// Send data to proxy
        /// </summary>
        /// <param name="data">Data</param>
        void SendData(byte[] data);

        /// <summary>
        /// Send data of random size to proxy
        /// </summary>
        void SendRandomSizeData();

        /// <summary>
        /// Send data of specific size to proxy
        /// </summary>
        /// <param name="dataSize">DataSize</param>
        void SendSpecificSizeData(int dataSize);

        /// <summary>
        /// Send X amount of data with packets of Y size
        /// </summary>
        /// <param name="dataSize">Packet size</param>
        /// <param name="totalData">Total data</param>
        void SendTotalAmountOfData(int dataSize, int totalData);

        /// <summary>
        /// Stops sender
        /// </summary>
        void Stop();
    }
}