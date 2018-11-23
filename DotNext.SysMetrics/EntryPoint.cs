using BenchmarkDotNet.Running;

namespace DotNext.SysMetrics
{
    public class EntryPoint
    {
        public static void Main(string[] args)
        {
//            BenchmarkRunner.Run<PerformanceCountersBenchmark>();
//            BenchmarkRunner.Run<ProcessMemoryMetricsBenchmark>();
//            BenchmarkRunner.Run<GlobalMemoryMetricsBenchmark>();
            BenchmarkRunner.Run<CpuUsageMetricsBenchmark>();
        }
    }
}