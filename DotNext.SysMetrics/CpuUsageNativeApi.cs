
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace DotNext.SysMetrics
{
    internal static class CpuUsageNativeApi
    {
        public static ulong GetSystemTime()
        {
            if (GetSystemTimes(out _, out var kernelTime, out var userTime))
                return kernelTime.ToULong() + userTime.ToULong();

            throw new Win32Exception();
        }

        public static ulong GetSystemUsedTime()
        {
            if (GetSystemTimes(out var idleTime, out var kernelTime, out var userTime))
                return kernelTime.ToULong() + userTime.ToULong() - idleTime.ToULong();

            throw new Exception();
        }

        public static ulong GetProcessTime(SafeHandle hProcess)
        {
            if (GetProcessTimes(hProcess, out _, out _, out var kernelTime, out var userTime))
                return kernelTime.ToULong() + userTime.ToULong();

            throw new Exception();
        }
        
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetProcessTimes(
            SafeHandle hProcess,
            out FILETIME creationTime,
            out FILETIME exitTime,
            out FILETIME kernelTime,
            out FILETIME userTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetSystemTimes(
            out FILETIME idleTime,
            out FILETIME kernelTime,
            out FILETIME userTime);
        
        public static ulong ToULong(this FILETIME filetime)
        {
            var high = (ulong)filetime.dwHighDateTime;
            var low = (ulong)filetime.dwLowDateTime;
            return (high << 32) | low;
        }
    }
}