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
            List<long> previousCpuUssages = new();
            PerformanceCounter idleCpuUsage = new PerformanceCounter("Processor", "% Idle Time", "_Total", true);

            for (int i = 0; i <processes.Length; i++)
            { 
                Process process = processes[i];
                int memsize = 0; // memsize in KB
                PerformanceCounter ram = new PerformanceCounter();

                ram.CategoryName = "Process";
                ram.CounterName = "Working Set - Private";
                ram.InstanceName = i == 0 ? process.ProcessName : process.ProcessName + $"#{i}";
                ram.ReadOnly = true;
                
                memoryCounters.Add(ram);
                previousCpuUssages.Add(0);
/*                PerformanceCounter cpu = new PerformanceCounter("Process", "% Processor Time",
                    i == 0 ? process.ProcessName : process.ProcessName + $"#{i}", true);
                cpuCounters.Add(cpu);*/

            };

            bool work = true;
            Thread thread = new Thread(() => 
            {
                long oldIdleCpuUsage = 0;
                double cpuUsagePercent = 0.0d;
                Stopwatch sw = Stopwatch.StartNew();
                while (work)
                {
                    double memUsage = 0;
                    memoryCounters.ForEach(memory => 
                    {
                        memUsage += memory.NextValue();
                    });

/*                    double cpuUsage1 = 0.0d;
                     cpuCounters.ForEach(cpu =>
                    {
                        cpuUsage1 += cpu.NextValue();

                    });
                    Console.WriteLine(cpuUsage1 / 6);*/
                    double cpuUsage = 0.0d;
                    sw.Stop();
                    for (int i = 0; i < processes.Length; i++)
                    {
                        Process process = processes[i];
                        long NewCpuUsage = (long)process.TotalProcessorTime.TotalMilliseconds;
                        long msPassed = sw.ElapsedMilliseconds;
                        cpuUsage += (NewCpuUsage - previousCpuUssages[i]) * 1.0d / (Environment.ProcessorCount * msPassed);
                        previousCpuUssages[i] = NewCpuUsage;
                    }
                    cpuUsage *= 100;
                    sw.Restart();
                    //Console.WriteLine($"Process use : {memUsage/(1024*1024)} Mb");
                    //Console.WriteLine("Process CPU % = " + cpuUsage );

                    string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    writer.WriteData(timestamp, ((int)(memUsage / (1024*1024))).ToString(), cpuUsage.ToString());
                    
                    Thread.Sleep(1500);
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

/*            cpuCounters.ForEach(counter =>
            {
                counter.Close();
                counter.Dispose();
            });*/
        }
    }
}
