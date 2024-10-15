
using ProxyLibrary.Buffer;
using System.Collections.Concurrent;

namespace ProxyLibrary.Handler
{
    /// <summary>
    /// Handler that handles when data is send
    /// </summary>
    public abstract class AbstractHandler
    {
        /// <summary>
        /// Thread that sends data
        /// </summary>
        private Thread _thread;

        /// <summary>
        /// Disposed flag
        /// </summary>
        private volatile bool _disposed;

        /// <summary>
        /// Buffers to handle
        /// </summary>
        protected BlockingCollection<IBuffer> _buffers = new();

        /// <summary>
        /// Tokens per turn
        /// </summary>
        protected int _tokensPerTurn;

        /// <summary>
        /// Constructor. Starts thread
        /// </summary>
        public AbstractHandler(int tokensPerTurn)
        {
            _tokensPerTurn = tokensPerTurn;
            _thread = new(() => { ThreadLoop(); }) ;
            _thread.Start();
        }

        /// <summary>
        /// Adds buffer to list
        /// </summary>
        /// <param name="buffer">Buffer to add</param>
        public void AddBuffer(IBuffer buffer)
        {
            _buffers.Add(buffer);
        }

        /// <summary>
        /// Stops handler
        /// </summary>
        public void Stop()
        {
            _disposed = true;
            _thread.Join();
        }

        /// <summary>
        /// Thread loop
        /// </summary>
        private void ThreadLoop()
        {
            while (!_disposed) 
            {
                Send();
                Thread.Sleep(5);
            }
        }

        /// <summary>
        /// Send data
        /// </summary>
        public abstract void Send();
    }
}
