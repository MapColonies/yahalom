using System.Runtime.Serialization;
using com.mapcolonies.core.Services.Analytics.Interfaces;

namespace com.mapcolonies.core.Services.Analytics.Model
{
    public class ApplicationData : IAnalyticLogParameter
    {
        public string ApplicationName { get; private set; }
        public string ApplicationVersion { get; private set; }

        private ApplicationData(string applicationName, string applicationVersion)
        {
            ApplicationName = applicationName;
            ApplicationVersion = applicationVersion;
        }

        public static ApplicationData Create(string applicationName, string applicationVersion)
        {
            return new ApplicationData(applicationName, applicationVersion);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(ApplicationName), ApplicationName);
            info.AddValue(nameof(ApplicationVersion), ApplicationVersion);
        }
    }
}
