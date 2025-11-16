using System.Runtime.Serialization;
using com.mapcolonies.core.Services.Analytics.Model;
using NUnit.Framework;

namespace EditorTests.SerializationTests.Analytics
{
    public class ApplicationDataSerializationTests
    {
        [Test]
        public void ApplicationData_Serializes_Expected_Fields()
        {
            ApplicationData data = ApplicationData.Create("Yahalom", "1.2.3");
            SerializationInfo info = new SerializationInfo(typeof(ApplicationData), new FormatterConverter());
            data.GetObjectData(info, new StreamingContext());

            Assert.AreEqual("Yahalom", info.GetString(nameof(ApplicationData.ApplicationName)));
            Assert.AreEqual("1.2.3", info.GetString(nameof(ApplicationData.ApplicationVersion)));
        }
    }
}
