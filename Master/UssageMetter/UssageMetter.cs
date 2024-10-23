using Common;
using System.Diagnostics;
using System.Globalization;

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
            PerformanceCounter myAppCpu = new PerformanceCounter("Process", "% Processor Time", proc.ProcessName, true);
            CSVWriter writer = new CSVWriter("Values.csv");

            bool work = true;
            Thread thread = new Thread(() => 
            {
                while (work)
                {
                    memsize = Convert.ToInt32(PC.NextValue()) / (int)(1024);
                    double pct = myAppCpu.NextValue();
                    //Console.WriteLine($"Process use : {memsize} Kb");
                    //Console.WriteLine("Process CPU % = " + pct);

                    string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    writer.WriteData(timestamp, memsize.ToString(), pct.ToString());
                    
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
