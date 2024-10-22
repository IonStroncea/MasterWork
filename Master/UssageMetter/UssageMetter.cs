using System.Diagnostics;

namespace UssageMetter
{
    /// <summary>
    /// Ussage metter
    /// </summary>
    public static class UssageMetter
    {
        /// <summary>
        /// Main app
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            bool found = false;
            Process[] processes = Array.Empty<Process>();
            while (!found)
            {
                processes = Process.GetProcessesByName("Proxy");
                found = processes.Length > 0;

                Thread.Sleep(500);
            }

            
            Process proc = processes[0];
            int memsize = 0; // memsize in KB
            PerformanceCounter PC = new PerformanceCounter();
            PC.CategoryName = "Process";
            PC.CounterName = "Working Set - Private";
            PC.InstanceName = proc.ProcessName;

            bool work = true;
            Thread thread = new Thread(() => 
            {
                while (work)
                {
                    memsize = Convert.ToInt32(PC.NextValue()) / (int)(1024);
                    Console.WriteLine($"Process use : {memsize} Kb");
                    Thread.Sleep(1000);
                }
            });

            thread.Start();

            Console.ReadLine();
            work = false;
            thread.Join();

            PC.Close();
            PC.Dispose();
        }
    }
}
