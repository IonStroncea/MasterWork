using ServerLibrary;
using System.Diagnostics;

namespace Server
{
    /// <summary>
    /// Server console class
    /// </summary>
    public class Server
    {
        /// <summary>
        /// Main program
        /// </summary>
        /// <param name="args">Arguments -f address of server -p port of server -t time to live in seconds</param>
        public static void Main(string[] args)
        {
            string address = "127.0.0.1";
            int port = 9000;

            int ttl = -1;


            if (args.ToList().Contains("-f"))
            {
                address = args[args.ToList().IndexOf("-f") + 1];
            }
            if (args.ToList().Contains("-p"))
            {
                port = int.Parse(args[args.ToList().IndexOf("-p") + 1]);
            }

            if (args.ToList().Contains("-t"))
            {
                ttl = int.Parse(args[args.ToList().IndexOf("-t") + 1]);
            }

            BaseServer server = new BaseServer(address, port);
            server.Start();

            if (ttl > -1)
            {
                TimeSpan toLive = TimeSpan.FromSeconds(ttl);
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                stopwatch.Stop();
                TimeSpan elapsed = stopwatch.Elapsed;

                while (elapsed <= toLive)
                {
                    Thread.Sleep(5);
                    stopwatch.Stop();
                    elapsed = stopwatch.Elapsed;
                }
                server.Stop();
            }
            else
            {
                Console.ReadKey();
                server.Stop();
            }
        }
    }
}
