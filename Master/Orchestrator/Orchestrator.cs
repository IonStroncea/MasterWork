using Sender;
using Server;
using System.Diagnostics;
using System.IO;

namespace Orchestrator
{
    /// <summary>
    /// Orchestrates entire test
    /// </summary>
    public static class Orchestrator
    {
        /// <summary>
        /// Main method
        /// </summary>
        /// <param name="args">
        /// -end number of servers and senders
        /// -t time to live in seconds
        /// -buffer buffer type. Default(default) or SameSize
        /// -size packet size
        /// -handler handler type.RegularHandler(default) or SimultaniousHandler
        /// -tokens tokens per turn
        /// -wait wait time
        /// -proxies nr of proxies
        /// </param>
        public static void Main(string[] args)
        {
            string bufferType = "Default";
            string handlerType = "RegularHandler";
            int packetSize = 1000;
            int tokensPerTurn = 5;
            int timeToWait = 300;
            int end = 1;
            int proxies = 2;

            if (args.ToList().Contains("-end"))
            {
                end = int.Parse(args[args.ToList().IndexOf("-end") + 1]);
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
            if (args.ToList().Contains("-proxies"))
            {
                proxies = int.Parse(args[args.ToList().IndexOf("-proxies") + 1]);
            }

            List<Process> servers = new();
            List<Process> senders = new();

            int serverPort = 9000;
            string address = "127.0.0.1";

            for (int i = 0; i < end; i++)
            {
                Process server = new Process();
                server.StartInfo.FileName = "Server.exe";
                server.StartInfo.Arguments = $"/c -f {address} -p {serverPort + i}";

                servers.Add(server);
            }

            for (int i = 0; i < end; i++)
            {
                Process sender = new Process();
                sender.StartInfo.FileName = "Sender.exe";
                string proxiesString= "";

                for (int j = 0; j < proxies; j++)
                {
                    proxiesString += $" 127.0.0.1 {10000 + j * 100}";
                }

                sender.StartInfo.Arguments = $"/c -p {serverPort + i} -total {5000000} -size {1024 * (1 + i * 10)} -n {i + 1} -nrOfProxies {proxies} -proxies{proxiesString}";


                senders.Add(sender);
            }

            List<Process> proxiesList = new();

            for (int i = 0; i < proxies; i++)
            {
                Process proxy = new();
                proxy.StartInfo.FileName = "Proxy.exe";
                proxy.StartInfo.Arguments = $"/c -buffer {bufferType} -size {packetSize} -handler {handlerType} -tokens {tokensPerTurn} -wait {timeToWait} -p {10000 + i * 100}";
                proxiesList.Add(proxy);
            }

            proxiesList.ForEach(proxy =>
            {
                proxy.Start();
            });

            Process ussageMetter = new();
            ussageMetter.StartInfo.FileName = "UssageMetter.exe";


            ussageMetter.Start();

            servers.ForEach(server => server.Start());

            Thread.Sleep(2000);

            senders.ForEach(sender => sender.Start());

            Console.ReadLine();

            senders.ForEach(sender =>
            {
                sender.Kill();
                sender.WaitForExit();
                sender.Dispose();
            });

            ussageMetter.Kill();
            ussageMetter.WaitForExit();
            ussageMetter.Dispose();

            proxiesList.ForEach(proxy =>
            {
                proxy.Kill();
                proxy.WaitForExit();
                proxy.Dispose();
            });

            servers.ForEach(server =>
            {
                server.Kill();
                server.WaitForExit();
                server.Dispose();
            });  
        }
    }
}
