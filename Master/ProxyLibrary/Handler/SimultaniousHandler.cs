
using System.Diagnostics;

namespace ProxyLibrary.Handler
{
    /// <summary>
    /// Handler that sends data simoultaniously
    /// </summary>
    public class SimultaniousHandler : AbstractHandler
    {
        /// <summary>
        /// Stopwatch to measure time
        /// </summary>
        private Stopwatch _stopwatch = new();

        /// <summary>
        /// Time to wait
        /// </summary>
        private TimeSpan _timeSpanToWait;

        /// <summary>
        /// Constructor. Sets tokens and milliseconds to wait
        /// </summary>
        /// <param name="tokensPerTurn">Tokens per turn</param>
        /// <param name="milliseconds">Milliseconds</param>
        public SimultaniousHandler(int tokensPerTurn, int milliseconds) : base(tokensPerTurn)
        {
            _timeSpanToWait = TimeSpan.FromMilliseconds(milliseconds);
        }

        /// <inheritdoc/>
        public override void Send()
        {
            _stopwatch.Stop();
            TimeSpan ts = _stopwatch.Elapsed;

            if (ts > _timeSpanToWait)
            {
                List<Task> tasksToSend = new();

                foreach (var buffer in _buffers)
                {
                    tasksToSend.Add(new Task(() => { buffer.SendData(_tokensPerTurn); }));
                }

                tasksToSend.ForEach(x => x.Start()); ;

                Task.WaitAll(tasksToSend.ToArray());

                _stopwatch.Start();
            }
            else 
            {
                _stopwatch.Restart();
            }       
        }
    }
}
