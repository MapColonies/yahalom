using System;
using System.Diagnostics;
using UnityEngine;

namespace com.mapcolonies.core.Utilities
{
    public class PlatformUsageManager
    {
        private Process _currentProcess;
        private TimeSpan _previousTotalProcessorTime;
        private DateTime _previousProcessorSamplingTime;
        private int _logicalProcessorCount;

        private const float PROCESSOR_MULTIPLIER = 100f;
        private const int BYTES_TO_MB = 1048576;

        public void Init()
        {
            InitGpuUsageSampling();
        }

        private void InitGpuUsageSampling()
        {
            _currentProcess = Process.GetCurrentProcess();
            _previousTotalProcessorTime = _currentProcess.TotalProcessorTime;
            _previousProcessorSamplingTime = DateTime.UtcNow;
            _logicalProcessorCount = Environment.ProcessorCount;
        }

        public (float, double, double) GetApplicationPerformanceSnapshot()
        {
            var fps = CalculateFps();
            var allocatedMemory = CalculateAllocatedMemory();
            var cpuUsage = CalculateCpuUsage();

            return (fps, allocatedMemory, cpuUsage);
        }

        private float CalculateFps()
        {
            var fps = 1f / Time.deltaTime;
            return fps;
        }

        private double CalculateAllocatedMemory()
        {
            var allocatedMemoryInMb = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / BYTES_TO_MB;
            return allocatedMemoryInMb;
        }

        private double CalculateCpuUsage()
        {
            var currentTime = DateTime.UtcNow;
            var currentTotalProcessorTime = _currentProcess.TotalProcessorTime;
            var elapsedSeconds = (currentTime - _previousProcessorSamplingTime).TotalSeconds;
            var cpuTimeUsed = (currentTotalProcessorTime - _previousTotalProcessorTime).TotalSeconds;
            var cpuUsageInPercentage = cpuTimeUsed / (elapsedSeconds * _logicalProcessorCount) * PROCESSOR_MULTIPLIER;

            _previousProcessorSamplingTime = currentTime;
            _previousTotalProcessorTime = currentTotalProcessorTime;

            return cpuUsageInPercentage;
        }
    }
}
