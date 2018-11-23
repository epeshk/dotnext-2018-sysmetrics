using System;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using Vostok.Sys.Metrics.PerfCounters;
using Vostok.Sys.Metrics.Windows.Meters.CPU;
using Vostok.Sys.Metrics.Windows.Meters.Memory;

namespace DotNext.SysMetrics
{
    /*
BenchmarkDotNet=v0.11.3, OS=Windows 10.0.17134.407 (1803/April2018Update/Redstone4)
Intel Core i7-6700 CPU 3.40GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
Frequency=3328124 Hz, Resolution=300.4696 ns, Timer=TSC
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3221.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3221.0


                       Method |        Mean |      Error |      StdDev |
----------------------------- |------------:|-----------:|------------:|
                  PerfCounter |    42.47 us |  0.4453 us |   0.4165 us |
 WinApiPerformanceInformation | 4,494.44 us | 89.8597 us | 113.6438 us |

     */
    public class GlobalMemoryMetricsBenchmark
    {
        [Benchmark]
        public void PerfCounter() => meter.GetGlobalMemoryInfo();

        [Benchmark]
        public void WinApiPerformanceInformation() => GetPerformanceInfo(out _);
    
        private GlobalMemoryMeter meter = new GlobalMemoryMeter(PerformanceCounterFactory.Default);
        
        private static unsafe bool GetPerformanceInfo(out PERFORMANCE_INFORMATION performanceInfo)
            => GetPerformanceInfo(out performanceInfo, sizeof(PERFORMANCE_INFORMATION));
        
        [DllImport("psapi.dll", SetLastError = true)]
        private static extern bool GetPerformanceInfo(
            out PERFORMANCE_INFORMATION pPerformanceInformation,
            [In] int cb
        );
        
        // ReSharper disable once InconsistentNaming
        private struct PERFORMANCE_INFORMATION
        {
            public int cb;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonpaged;
            public IntPtr PageSize;
            public int HandleCount;
            public int ProcessCount;
            public int ThreadCount;
        }
    }
}