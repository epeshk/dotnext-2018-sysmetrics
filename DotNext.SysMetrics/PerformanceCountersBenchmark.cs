using System;
using System.Collections.Generic;
using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using Vostok.Sys.Metrics.PerfCounters;
using TestProcess;

namespace DotNext.SysMetrics
{
/*
BenchmarkDotNet=v0.11.3, OS=Windows 10.0.17134.407 (1803/April2018Update/Redstone4)
Intel Core i7-6700 CPU 3.40GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
Frequency=3328124 Hz, Resolution=300.4696 ns, Timer=TSC
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3221.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3221.0


                         Method |         Mean |       Error |      StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
------------------------------- |-------------:|------------:|------------:|------------:|------------:|------------:|--------------------:|
      SystemDiagnostics_Process | 19,666.32 us | 185.2377 us | 154.6819 us |    718.7500 |    531.2500 |    375.0000 |           3480020 B |
 SystemDiagnostics_NetClrMemory |  1,188.07 us |  23.4072 us |  24.0375 us |    164.0625 |     62.5000 |     31.2500 |            687461 B |
                    PDH_Process |  3,321.09 us |  39.8353 us |  35.3130 us |           - |           - |           - |                   - |
               PDH_NetClrMemory |     40.07 us |   0.6095 us |   0.5403 us |           - |           - |           - |                   - |
*/
    [MemoryDiagnoser]
    public class PerformanceCountersBenchmark
    {
        private static readonly string ProcessName = Process.GetCurrentProcess().ProcessName;
        private PerformanceCounter process;
        private PerformanceCounter netClrMemory;
        private IPerformanceCounter<double> processPDH;
        private IPerformanceCounter<double> netClrMemoryPDH;
        private List<TestProcessHandle> processes = new List<TestProcessHandle>();

        [GlobalSetup]
        public void Setup()
        {
            for (int i = 0; i < 100; i++)
                processes.Add(new TestProcessHandle());
            
            GC.Collect(2, GCCollectionMode.Forced, true);
            process = new PerformanceCounter("Process", "ID Process", ProcessName);
            netClrMemory = new PerformanceCounter(".NET CLR Memory", "Process ID", ProcessName);
            processPDH = PerformanceCounterFactory.Default
                .CreateCounter("Process", "ID Process", ProcessName);
            netClrMemoryPDH = PerformanceCounterFactory.Default
                .CreateCounter(".NET CLR Memory", "Process ID", ProcessName);
        }

        [GlobalCleanup]
        public void Cleanup() => processes.ForEach(x => x.Dispose());

        [Benchmark]
        public double SystemDiagnostics_Process()
            => process.NextValue();

        [Benchmark]
        public double SystemDiagnostics_NetClrMemory()
            => netClrMemory.NextValue();

        [Benchmark]
        public double PDH_Process()
            => processPDH.Observe();

        [Benchmark]
        public double PDH_NetClrMemory()
            => netClrMemoryPDH.Observe();
    }
}