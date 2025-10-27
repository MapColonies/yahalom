using System;
using System.Diagnostics;
using UnityEngine;

namespace com.mapcolonies.core.Utilities
{
    public class PlatformUsageHelper
    {
        private Process _currentProcess;
        private TimeSpan _previousTotalProcessorTime;
        private DateTime _previousProcessorSamplingTime;
        private int _logicalProcessorCount;

        private const float PROCESSOR_MULTIPLIER = 100f;
        private const int BYTES_TO_MB = 1048576;

        public PlatformUsageHelper()
        {
            _currentProcess = Process.GetCurrentProcess();
            _previousTotalProcessorTime = _currentProcess.TotalProcessorTime;
            _previousProcessorSamplingTime = DateTime.UtcNow;
            _logicalProcessorCount = Environment.ProcessorCount;
        }

        public (float, double, double) GetApplicationPerformanceSnapshot()
        {
            float fps = CalculateFps();
            double allocatedMemory = CalculateAllocatedMemory();
            double cpuUsage = CalculateCpuUsage();
            return (fps, allocatedMemory, cpuUsage);
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

        private double CalculateCpuUsage()
        {
            DateTime currentTime = DateTime.UtcNow;
            TimeSpan currentTotalProcessorTime = _currentProcess.TotalProcessorTime;
            double elapsedSeconds = (currentTime - _previousProcessorSamplingTime).TotalSeconds;
            double cpuTimeUsed = (currentTotalProcessorTime - _previousTotalProcessorTime).TotalSeconds;
            double cpuUsageInPercentage = cpuTimeUsed / (elapsedSeconds * _logicalProcessorCount) * PROCESSOR_MULTIPLIER;

            _previousProcessorSamplingTime = currentTime;
            _previousTotalProcessorTime = currentTotalProcessorTime;

            return cpuUsageInPercentage;
        }
    }
}
