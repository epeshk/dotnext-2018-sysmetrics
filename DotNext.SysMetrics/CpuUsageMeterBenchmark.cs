using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using Vostok.Sys.Metrics.PerfCounters;

namespace DotNext.SysMetrics
{
    /*
BenchmarkDotNet=v0.11.3, OS=Windows 10.0.17134.407 (1803/April2018Update/Redstone4)
Intel Core i7-6700 CPU 3.40GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
Frequency=3328124 Hz, Resolution=300.4696 ns, Timer=TSC
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3221.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3221.0


      Method |         Mean |      Error |     StdDev |
------------ |-------------:|-----------:|-----------:|
      WinApi |     4.080 us |  0.1495 us |  0.4067 us |
 PerfCounter | 3,057.072 us | 38.7816 us | 36.2763 us |

     */
    public class CpuUsageMetricsBenchmark
    {
        private CpuMeter meter = new CpuMeter(Process.GetCurrentProcess().Id);

        private IPerformanceCounter<double> cpuCounter = PerformanceCounterFactory
            .Default
            .CreateCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);

        [Benchmark]
        public double WinApi() => meter.Measure();
        
        [Benchmark]
        public double PerfCounter() => cpuCounter.Observe();
    }
}