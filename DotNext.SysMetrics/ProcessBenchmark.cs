using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using BenchmarkDotNet.Attributes;
using Vostok.Sys.Metrics.Windows.Meters.Memory;
using TestProcess;

namespace DotNext.SysMetrics
{
    /*
  === Framework ===
BenchmarkDotNet=v0.11.3, OS=Windows 10.0.17134.407 (1803/April2018Update/Redstone4)
Intel Core i7-6700 CPU 3.40GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
Frequency=3328124 Hz, Resolution=300.4696 ns, Timer=TSC
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3221.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3221.0


                   Method |         Mean |      Error |     StdDev | Gen 0/1k Op | Allocated Memory/Op |
------------------------- |-------------:|-----------:|-----------:|------------:|--------------------:|
 SystemDiagnosticsProcess | 3,352.916 us | 47.2505 us | 44.1981 us |     31.2500 |            137353 B |
                   WinApi |     1.267 us |  0.0197 us |  0.0184 us |           - |                   - |

  === Core ===
BenchmarkDotNet=v0.11.3, OS=Windows 10.0.17134.407 (1803/April2018Update/Redstone4)
Intel Core i7-6700 CPU 3.40GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
Frequency=3328124 Hz, Resolution=300.4696 ns, Timer=TSC
.NET Core SDK=2.1.401
  [Host]     : .NET Core 2.1.3 (CoreCLR 4.6.26725.06, CoreFX 4.6.26725.05), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.3 (CoreCLR 4.6.26725.06, CoreFX 4.6.26725.05), 64bit RyuJIT


                   Method |         Mean |      Error |     StdDev | Allocated Memory/Op |
------------------------- |-------------:|-----------:|-----------:|--------------------:|
 SystemDiagnosticsProcess | 3,117.915 us | 58.0967 us | 48.5133 us |              3576 B |
                   WinApi |     1.248 us |  0.0248 us |  0.0232 us |                   - |
     */
    [MemoryDiagnoser]
    public class ProcessMemoryMetricsBenchmark
    {
        private Process process = Process.GetCurrentProcess();
        private ProcessMemoryMeter meter = new ProcessMemoryMeter();
        private List<TestProcessHandle> processes = new List<TestProcessHandle>();

        [GlobalSetup]
        public void Setup()
        {
            for (int i = 0; i < 100; i++)
                processes.Add(new TestProcessHandle());

        }

        [GlobalCleanup]
        public void Cleanup()
        {
            meter.Dispose();
            processes.ForEach(x => x.Dispose());
        }

        [Benchmark]
        public long SystemDiagnosticsProcess()
        {
            process.Refresh();
            return process.WorkingSet64;
        }
        
        [Benchmark]
        public long WinApi()
        {
            return meter.GetMemoryInfo().WorkingSetBytes;
        }
    }
}