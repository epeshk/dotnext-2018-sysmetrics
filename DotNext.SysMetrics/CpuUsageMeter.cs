using System;
using System.Diagnostics;
using Microsoft.Win32.SafeHandles;

namespace DotNext.SysMetrics
{
    internal class CpuMeter
    {
        private SafeProcessHandle handle;
        
        public CpuMeter(int pid)
        {
            handle = Process.GetProcessById(pid).SafeHandle;
        }

        public double Measure()
        {
            var systemTime = CpuUsageNativeApi.GetSystemTime();
            var usedTime = CpuUsageNativeApi.GetProcessTime(handle);

            if (isFirstMeasure)
            {
                sysTimePrev = systemTime;
                usedTimePrev = usedTime;
                isFirstMeasure = false;
                return 0;
            }

            var sysDelta = systemTime - sysTimePrev;
            var usedDelta = usedTime - usedTimePrev;

            sysTimePrev = systemTime;
            usedTimePrev = usedTime;

            var usage = sysDelta <= 0
                ? 0
                : Math.Max(0, Math.Min(1, usedDelta / (double) sysDelta));

            return usage;
        }

        private ulong usedTimePrev;
        private ulong sysTimePrev;

        private bool isFirstMeasure = true;
    }
}