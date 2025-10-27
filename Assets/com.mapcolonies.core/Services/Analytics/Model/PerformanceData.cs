using System.Runtime.Serialization;

namespace com.mapcolonies.core.Services.Analytics.Model
{
    public class PerformanceData : IAnalyticLogParameter
    {
        public float Fps { get; private set; }
        public double AllocatedMemoryInMB { get; private set; }
        public double CpuUsagePercentage { get; private set; }

        private PerformanceData(float fps, double allocatedMemoryInMb, double cpuUsagePercentage)
        {
            Fps = fps;
            AllocatedMemoryInMB = allocatedMemoryInMb;
            CpuUsagePercentage = cpuUsagePercentage;
        }

        public static PerformanceData Create(float fps, double allocatedMemoryInMb, double cpuUsagePercentage)
        {
            return new PerformanceData(fps, allocatedMemoryInMb, cpuUsagePercentage);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Fps), Fps);
            info.AddValue(nameof(AllocatedMemoryInMB), AllocatedMemoryInMB);
            info.AddValue(nameof(CpuUsagePercentage), CpuUsagePercentage);
        }
    }
}
