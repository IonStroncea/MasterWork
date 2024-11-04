using ProxyLibrary;
using ProxyLibrary.Handler;
using System.Collections.Generic;
using System.Diagnostics;

namespace Proxy
{
    /// <summary>
    /// Proxy console app
    /// </summary>
    public class Proxy
    {
        /// <summary>
        /// Main program
        /// </summary>
        /// <param name="args">
        /// Arguments -f address of server 
        /// -p port of server 
        /// -t time to live in seconds
        /// -buffer buffer type. Default(default) or SameSize
        /// -size packet size
        /// -handler handler type.RegularHandler(default) or SimultaniousHandler
        /// -tokens tokens per turn
        /// -wait wait time
        /// -return if return values
        /// </param>
        public static void Main(string[] args)
        {
            string address = "127.0.0.1";
            string bufferType = "Default";
            string handlerType = "";
            int port = 10000;
            int packetSize = 1000;
            int tokensPerTurn = 5;
            int timeToWait = 300;
            int copies = 1;

            bool returnValues = false;

            int ttl = -1;

            if (args.ToList().Contains("-copies"))
            {
                copies = int.Parse(args[args.ToList().IndexOf("-copies") + 1]);
            }
            if (args.ToList().Contains("-return"))
            {
                returnValues = true;
            }
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

            if (args.ToList().Contains("-buffer"))
            {
                bufferType = args[args.ToList().IndexOf("-buffer") + 1];
            }

            if (args.ToList().Contains("-size"))
            {
                packetSize = int.Parse(args[args.ToList().IndexOf("-size") + 1]);
            }

            if (args.ToList().Contains("-handler"))
            {
                handlerType = args[args.ToList().IndexOf("-handler") + 1];
            }

            if (args.ToList().Contains("-tokens"))
            {
                tokensPerTurn = int.Parse(args[args.ToList().IndexOf("-tokens") + 1]);
            }

            if (args.ToList().Contains("-wait"))
            {
                timeToWait = int.Parse(args[args.ToList().IndexOf("-wait") + 1]);
            }

            List<AbstractHandler> handlers = new ();
            List<ProxyReceiver> receivers = new ();

            BufferEnum bufferEnum = (BufferEnum)Enum.Parse(typeof(BufferEnum), bufferType, true);
            for (int i = 0; i < copies; i++)
            {

                AbstractHandler? handler;

                if (handlerType.Equals("simultanious"))
                {
                    handler = new SimultaniousHandler(tokensPerTurn, timeToWait);
                }
                else
                {
                    handler = new RegularHandler(tokensPerTurn);
                }


                ProxyReceiver proxy = new ProxyReceiver(address, port + i, bufferEnum, packetSize, handler, returnValues);

                handlers.Add(handler);
                receivers.Add(proxy);
                Console.WriteLine($"Started proxy at {address}:{port + i}");
            }

            receivers.ForEach(proxy => proxy.Start());

           
            if (ttl > -1)
            {
                TimeSpan toLive = TimeSpan.FromSeconds(ttl);
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                stopwatch.Stop();
                TimeSpan elapsed = stopwatch.Elapsed;

                while (elapsed <= toLive)
                {
                    Thread.Sleep(1);
                    stopwatch.Stop();
                    elapsed = stopwatch.Elapsed;
                }

                receivers.ForEach(proxy => proxy.Stop());
                handlers.ForEach(handler =>  handler.Stop());
            }
            else
            {
                Console.ReadKey();

                receivers.ForEach(proxy => proxy.Stop());
                handlers.ForEach(handler => handler.Stop());
            }
        }
    }
}
