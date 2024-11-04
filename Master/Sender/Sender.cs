using Common;
using SenderLibrary;
using System.Diagnostics;

namespace Sender
{
    /// <summary>
    /// Sender console class
    /// </summary>
    public class Sender
    {
        /// <summary>
        /// Main program
        /// </summary>
        /// <param name="args">Arguments:
        /// -f address of server 
        /// -p port of server 
        /// -t time to live in seconds 
        /// -n name 
        /// -total totalDataSize
        /// -size dataSize
        /// -nrOfProxies number of proxies after first
        /// -proxies list of proxies addresses and ports
        /// -return if return values
        /// -copies copies of senders
        /// </param>
        public static void Main(string[] args)
        {
            string address = "127.0.0.1";
            int port = 9000;
            string name = "Sender";

            int ttl = -1;
            int totalDataSize = -1;
            int dataSize = 1024;
            int nrOfProxies = 1;
            bool returnValues = false;
            int copies = 1;

            List<ProxyInfo> nextProxies =
            [
                new ProxyInfo
                {
                    NextAddress = "127.0.0.1",
                    NextPort = 10000,
                }
            ];

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
            if (args.ToList().Contains("-nrOfProxies"))
            {
                nrOfProxies = int.Parse(args[args.ToList().IndexOf("-nrOfProxies") + 1]);
            }
            if (args.ToList().Contains("-proxies"))
            {
                nextProxies = new();
                int pointer = args.ToList().IndexOf("-proxies");
                for (int i = 0; i < nrOfProxies; i++)
                {
                    nextProxies.Add(new ProxyInfo
                    {                       
                        NextAddress = args[pointer + i*2 + 1],
                        NextPort = int.Parse(args[pointer + i * 2 + 2])
                    });
                }
            }
            if (args.ToList().Contains("-p"))
            {
                port = int.Parse(args[args.ToList().IndexOf("-p") + 1]);
            }
            if (args.ToList().Contains("-n"))
            {
                name = args[args.ToList().IndexOf("-n") + 1];
            }

            if (args.ToList().Contains("-t"))
            {
                ttl = int.Parse(args[args.ToList().IndexOf("-t") + 1]);
            }
            if (args.ToList().Contains("-total"))
            {
                totalDataSize = int.Parse(args[args.ToList().IndexOf("-total") + 1]);
            }
            if (args.ToList().Contains("-size"))
            {
                dataSize = int.Parse(args[args.ToList().IndexOf("-size") + 1]);
            }

            List<ISender> senders = new ();

            for (int i = 0; i < copies; i++)
            {
                ISender sender = returnValues ? new SenderWithReturn(address, port, name, nextProxies) : new BaseSender(address, port, name, nextProxies);
                
                senders.Add(sender);
                //senders.Add(new EncryptionBaseSender(address, port + i, name, nextProxies));
                Console.WriteLine($"Created sender to {address}:{port}");

                if (totalDataSize > -1)
                {
                    sender.SendTotalAmountOfData(dataSize, totalDataSize);
                }
            }

            List<Task> runTasks = new();
            if (ttl > -1)
            {
               
                foreach (var sender in senders)
                {
                    var runTask = new Task(() => 
                    {
                        TimeSpan toLive = TimeSpan.FromSeconds(ttl);
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();

                        stopwatch.Stop();
                        TimeSpan elapsed = stopwatch.Elapsed;

                        while (elapsed <= toLive)
                        {
                            if (dataSize == -1)
                            {
                                sender.SendRandomSizeData();
                            }
                            else
                            {
                                sender.SendSpecificSizeData(dataSize);
                            }
                            Thread.Sleep(5);
                            stopwatch.Stop();
                            elapsed = stopwatch.Elapsed;
                        }
                    });

                    runTasks.Add(runTask);
                }
            }
            else 
            {
                foreach (var sender in senders)
                {
                    var runTask = new Task(() =>
                    {
                        while (true)
                        {
                            if (dataSize == -1)
                            {
                                sender.SendRandomSizeData();
                            }
                            else
                            {
                                sender.SendSpecificSizeData(dataSize);
                            }
                            Thread.Sleep(5);
                        }
                    });

                    runTasks.Add(runTask);
                }
            }

            runTasks.ForEach(x => x.Start());

            Task.WaitAll(runTasks.ToArray());
        }
    }
}
