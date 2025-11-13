using System.Runtime.Serialization;
using com.mapcolonies.core.Services.Analytics.Model;
using NUnit.Framework;

namespace EditorTests.Analytics.SerializationTests
{
    public class PerformanceDataSerializationTests
    {
        [Test]
        public void PerformanceData_Serializes_Expected_Fields()
        {
            PerformanceData data = PerformanceData.Create(58.9f, 1024.5, 23.3);
            SerializationInfo info = new SerializationInfo(typeof(PerformanceData), new FormatterConverter());
            data.GetObjectData(info, new StreamingContext());

            Assert.AreEqual(58.9f, info.GetSingle(nameof(PerformanceData.Fps)));
            Assert.AreEqual(1024.5, info.GetDouble(nameof(PerformanceData.AllocatedMemoryInMB)));
            Assert.AreEqual(23.3, info.GetDouble(nameof(PerformanceData.CpuUsagePercentage)));
        }
    }
}
