using System.Diagnostics;

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
        /// -return if return values
        /// -multiple multiple tunnels
        /// </param>
        public static void Main(string[] args)
        {
            string bufferType = "Default";
            string handlerType = "";
            int packetSize = 4000;        
            int timeToWait = 1;
            int tokensPerTurn = 100;
            int end = 3;
            int sendersCopies = 333;
            int proxies = 1;
            int proxyCopies = 6;
            bool multiple = true;

            bool returnValues = false;

            if (args.ToList().Contains("-multiple"))
            {
                multiple = true;
            }
            if (args.ToList().Contains("-end"))
            {
                end = int.Parse(args[args.ToList().IndexOf("-end") + 1]);
            }
            if (args.ToList().Contains("-sendersCopies"))
            {
                sendersCopies = int.Parse(args[args.ToList().IndexOf("-sendersCopies") + 1]);
            }
            if (args.ToList().Contains("-proxyCopies"))
            {
                proxyCopies = int.Parse(args[args.ToList().IndexOf("-proxyCopies") + 1]);
            }
            if (args.ToList().Contains("-return"))
            {
                returnValues = true;
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
            string returnValuesString = returnValues ? "-return" : "";

            if (!multiple)
            {
                for (int i = 0; i < end; i++)
                {
                    Process server = new Process();
                    server.StartInfo.FileName = "Server.exe";
                    server.StartInfo.Arguments = $"/c -f {address} -p {serverPort + i} {returnValuesString}";

                    servers.Add(server);
                }
            }
            else 
            {
                for (int i = 0; i < end; i++)
                {
                    Process server = new Process();
                    server.StartInfo.FileName = "Server.exe";
                    server.StartInfo.Arguments = $"/c -multiple -f {address} -p {serverPort + i} {returnValuesString}";

                    servers.Add(server);
                }
            }

            Random random = new();

            if (!multiple)
            {
                for (int i = 0; i < end; i++)
                {
                    Process sender = new Process();
                    sender.StartInfo.FileName = "Sender.exe";
                    string proxiesString = "";
                    int currentCopies = sendersCopies;

                    for (int j = 0; j < proxies || j < proxyCopies; j++)
                    {
                        proxiesString += $" 127.0.0.1 {10000 + j}";
                    }

                    sender.StartInfo.Arguments = $"/c -multiple -copies {currentCopies} {returnValuesString} -p {serverPort + i} -total {1000000} -size {(int)(1024 + random.NextInt64(2048))} -n {i + 1} -nrOfProxies {Math.Max(proxies, proxyCopies)} -proxies{proxiesString}";


                    senders.Add(sender);
                }
            }
            else 
            {
                for (int i = 0; i < end; i++)
                {
                    Process sender = new Process();
                    sender.StartInfo.FileName = "Sender.exe";
                    string proxiesString = "";
                    int currentCopies = sendersCopies;

                    for (int j = 0; j < proxies / 2 || j < proxyCopies / 2; j++)
                    {
                        proxiesString += $" 127.0.0.1 {10000 + random.NextInt64(Math.Max(proxies, proxyCopies))}";
                    }

                    proxiesString += $" 127.0.0.1 {serverPort + i}";

                    proxiesString += $" 127.0.0.1 {10000 + 1}";
                    proxiesString += $" 127.0.0.1 {10000 + 5}";
                    proxiesString += $" 127.0.0.1 {10000 + 3}";

                    proxiesString += $" 127.0.0.1 {10000 + 0}";
                    proxiesString += $" 127.0.0.1 {10000 + 4}";
                    proxiesString += $" 127.0.0.1 {10000 + 2}";

                    for (int j = 0; j < proxies / 2 || j < proxyCopies / 2; j++)
                    {
                        proxiesString += $" 127.0.0.1 {10000 + random.NextInt64(Math.Max(proxies, proxyCopies))}";
                    }

                    sender.StartInfo.Arguments = $"/c -multiple -nultipleListen 127.0.0.1 -multiplePort {13000 + i * currentCopies} -copies {currentCopies} {returnValuesString} -p {13000 + i * currentCopies} -total {1000000} -size {(int)(1024 + random.NextInt64(2048))} -n {i + 1} -nrOfProxies {Math.Max(proxies, proxyCopies)} -proxies{proxiesString}";

                    senders.Add(sender);
                }
            }

            List<Process> proxiesList = new();

            for (int i = 0; i < proxies; i++)
            {
                Process proxy = new();
                proxy.StartInfo.FileName = "Proxy.exe";
                proxy.StartInfo.Arguments = $"/c -copies {proxyCopies} {returnValuesString} -buffer {bufferType} -size {packetSize} -handler {handlerType} -tokens {tokensPerTurn} -wait {timeToWait} -p {10000 + i}";
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

            //senders.ForEach(sender => sender.Start());
            Task.Factory.StartNew(() => senders.ForEach(task => task.Start()));

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
