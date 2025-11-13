using System;
using System.Runtime.Serialization;
using com.mapcolonies.core.Services.Analytics.Model;
using NUnit.Framework;

namespace EditorTests.Analytics.SerializationTests
{
    public class ApplicationUsageDataSerializationTests
    {
        [Test]
        public void ApplicationUsageData_Serializes_Expected_Fields()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(123.45);
            ApplicationUsageData data = ApplicationUsageData.Create(timeSpan);
            SerializationInfo info = new SerializationInfo(typeof(ApplicationUsageData), new FormatterConverter());
            data.GetObjectData(info, new StreamingContext());

            TimeSpan storedTimeSpan = (TimeSpan)info.GetValue(nameof(ApplicationUsageData.Time), typeof(TimeSpan));
            Assert.AreEqual(123.45, storedTimeSpan.TotalSeconds, 0.0001);
        }
    }
}
