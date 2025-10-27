using System;
using System.Runtime.Serialization;

namespace com.mapcolonies.core.Services.Analytics.Model
{
    public class ApplicationUsageData : IAnalyticLogParameter
    {
        public TimeSpan Time { get; private set; }

        private ApplicationUsageData(TimeSpan time)
        {
            Time = time;
        }

        public static ApplicationUsageData Create(TimeSpan totalAppTime)
        {
            return new ApplicationUsageData(totalAppTime);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Time), Time);
        }
    }
}
