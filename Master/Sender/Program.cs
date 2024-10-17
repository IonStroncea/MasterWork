using SenderLibrary;
using System.Diagnostics;

namespace Sender
{
    internal class Program
    {
        /// <summary>
        /// Main program
        /// </summary>
        /// <param name="args">Arguments -a address of server -p port of server -pa proxy address -pp proxy port -t time to live in seconds</param>
        static void Main(string[] args)
        {
            string address = "";
            int port = 0;

            string proxyAddress = "";
            int proxyPort = 0;

            int ttl = -1;


            if (args.ToList().Contains("-f"))
            {
                address = args[args.ToList().IndexOf("-f") + 1];
            }
            if (args.ToList().Contains("-p"))
            {
                port = int.Parse(args[args.ToList().IndexOf("-p") + 1]);
            }

            if (args.ToList().Contains("-fp"))
            {
                proxyAddress = args[args.ToList().IndexOf("-fp") + 1];
            }
            if (args.ToList().Contains("-pp"))
            {
                proxyPort = int.Parse(args[args.ToList().IndexOf("-pp") + 1]);
            }

            if (args.ToList().Contains("-t"))
            {
                ttl = int.Parse(args[args.ToList().IndexOf("-t") + 1]);
            }

            BaseSender sender = new BaseSender(address, port, proxyAddress, proxyPort);

            if (ttl > -1)
            { 
                TimeSpan toLive = TimeSpan.FromSeconds(ttl);
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                stopwatch.Stop();
                TimeSpan elapsed = stopwatch.Elapsed;

                while (elapsed <= toLive)
                {
                    sender.SendRandomSizeData();
                    Thread.Sleep(5);
                    stopwatch.Stop();
                    elapsed = stopwatch.Elapsed;
                }
            }
            else 
            {
                while (true)
                {
                    sender.SendRandomSizeData();
                    Thread.Sleep(5);
                }
            }
        }
    }
}
