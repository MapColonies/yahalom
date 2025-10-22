using System.Runtime.Serialization;
using com.mapcolonies.core.Services.Analytics.Interfaces;

namespace com.mapcolonies.core.Services.Analytics.Model
{
    public class ApplicationUsageData : IAnalyticLogParameter
    {
        public double Time { get; private set; }

        private ApplicationUsageData(double time)
        {
            Time = time;
        }

        public static ApplicationUsageData Create(double totalAppTime)
        {
            return new ApplicationUsageData(totalAppTime);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Time), Time);
        }
    }
}
