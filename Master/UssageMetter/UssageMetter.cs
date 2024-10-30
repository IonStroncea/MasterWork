using Common;
using System;
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

            CSVWriter writer = new CSVWriter("Values.csv");

            List<PerformanceCounter> memoryCounters = new();
            List<PerformanceCounter> cpuCounters = new();

            for(int i = 0; i <processes.Length; i++)
            { 
                Process process = processes[i];
                int memsize = 0; // memsize in KB
                PerformanceCounter ram = new PerformanceCounter();

                ram.CategoryName = "Process";
                ram.CounterName = "Working Set - Private";
                ram.InstanceName = i == 0 ? process.ProcessName : process.ProcessName + $"#{i}";
                ram.ReadOnly = true;
                
                memoryCounters.Add(ram);

                PerformanceCounter cpu = new PerformanceCounter("Process", "% Processor Time",
                    i == 0 ? process.ProcessName : process.ProcessName + $"#{i}", true);
                cpuCounters.Add(cpu);
                
            };

            bool work = true;
            Thread thread = new Thread(() => 
            {
                while (work)
                {
                    double memUsage = 0;
                    memoryCounters.ForEach(memory => 
                    {
                        memUsage += memory.NextValue();
                    });

                    double cpuUsage = 0.0d;
                    cpuCounters.ForEach(cpu =>
                    {
                        cpuUsage += cpu.NextValue();

                    });
                    //Console.WriteLine($"Process use : {memsize} Kb");
                    //Console.WriteLine("Process CPU % = " + pct);

                    string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    writer.WriteData(timestamp, ((int)(memUsage / 1024)).ToString(), cpuUsage.ToString());
                    
                    Thread.Sleep(1000);
                }
            });

            thread.Start();

            Console.ReadLine();
            work = false;
            thread.Join();

            memoryCounters.ForEach(counter => 
            {
                counter.Close();
                counter.Dispose();
            });

            cpuCounters.ForEach(counter =>
            {
                counter.Close();
                counter.Dispose();
            });
        }
    }
}
