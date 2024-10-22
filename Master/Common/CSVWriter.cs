
namespace Common
{
    /// <summary>
    /// Writes values to csv
    /// </summary>
    public class CSVWriter
    {
        /// <summary>
        /// File to write to
        /// </summary>
        private string _fileName;

        /// <summary>
        /// Writer to file
        /// </summary>
        private StreamWriter _writer;

        /// <summary>
        /// Constructor. Sets file name and starts stream writer
        /// </summary>
        /// <param name="fileName">File</param>
        public CSVWriter(string fileName)
        {
            _fileName = fileName;

            _writer = new StreamWriter(fileName, false);
        }

        /// <summary>
        /// Write line of data
        /// </summary>
        /// <param name="values">Values to write</param>
        public void WriteData(params string[] values)
        {
            string lineToWrite = string.Join(",", values);

            _writer.WriteLine(lineToWrite);
        }
    }
}
