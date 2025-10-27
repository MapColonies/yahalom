using System;
using System.Diagnostics;
using UnityEngine;

namespace com.mapcolonies.core.Utilities
{
    public static class PlatformUsageHelper
    {
        private static Process _currentProcess;
        private static int _logicalProcessorCount;

        private const float PROCESSOR_MULTIPLIER = 100f;
        private const int BYTES_TO_MB = 1048576;

        static PlatformUsageHelper()
        {
            _currentProcess = Process.GetCurrentProcess();
            _logicalProcessorCount = Environment.ProcessorCount;
        }

        public static (float, double, double, DateTime, TimeSpan) GetApplicationPerformanceSnapshot(DateTime previousProcessorSamplingTime,
            TimeSpan previousTotalProcessorTime)
        {
            float fps = CalculateFps();
            double allocatedMemory = CalculateAllocatedMemory();
            (double cpuUsage, DateTime newSamplingTime, TimeSpan newProcessorTime) = CalculateCpuUsage(previousProcessorSamplingTime, previousTotalProcessorTime);
            return (fps, allocatedMemory, cpuUsage, newSamplingTime, newProcessorTime);
        }

        public static (DateTime, TimeSpan) GetInitialProcessorTimes()
        {
            if (_currentProcess == null)
            {
                _currentProcess = Process.GetCurrentProcess();
            }

            return (DateTime.UtcNow, _currentProcess.TotalProcessorTime);
        }

        private static float CalculateFps()
        {
            float fps = 1f / Time.deltaTime;
            return fps;
        }

        private static double CalculateAllocatedMemory()
        {
            long allocatedMemoryInMb = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / BYTES_TO_MB;
            return allocatedMemoryInMb;
        }

        private static (double, DateTime, TimeSpan) CalculateCpuUsage(DateTime previousProcessorSamplingTime,
            TimeSpan previousTotalProcessorTime)
        {
            DateTime currentTime = DateTime.UtcNow;
            TimeSpan currentTotalProcessorTime = _currentProcess.TotalProcessorTime;
            double elapsedSeconds = (currentTime - previousProcessorSamplingTime).TotalSeconds;
            double cpuTimeUsed = (currentTotalProcessorTime - previousTotalProcessorTime).TotalSeconds;
            double cpuUsageInPercentage = cpuTimeUsed / (elapsedSeconds * _logicalProcessorCount) * PROCESSOR_MULTIPLIER;

            return (cpuUsageInPercentage, currentTime, currentTotalProcessorTime);
        }
    }
}
