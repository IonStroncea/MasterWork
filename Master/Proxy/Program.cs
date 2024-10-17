using System.Diagnostics;

namespace Proxy
{
    internal class Program
    {
        /// <summary>
        /// Main program
        /// </summary>
        /// <param name="args">Arguments -a address of server -p port of server -t time to live in seconds</param>
        static void Main(string[] args)
        {
            string address = "";
            int port = 0;

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

            BaseServer proxy = new BaseServer(address, port);
            proxy.Start();

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
                proxy.Stop();
            }
            else
            {
            }
        }
    }
}
