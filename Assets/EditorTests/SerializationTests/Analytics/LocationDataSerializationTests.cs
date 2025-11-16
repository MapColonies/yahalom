using System.Runtime.Serialization;
using com.mapcolonies.core.Services.Analytics.Model;
using NUnit.Framework;

namespace EditorTests.SerializationTests.Analytics
{
    public class LocationDataSerializationTests
    {
        [Test]
        public void LocationData_Serializes_Expected_Fields()
        {
            LocationData data = LocationData.Create(34.5, 31.7);
            SerializationInfo info = new SerializationInfo(typeof(LocationData), new FormatterConverter());
            data.GetObjectData(info, new StreamingContext());

            Assert.AreEqual(34.5, info.GetDouble(nameof(LocationData.Longitude)));
            Assert.AreEqual(31.7, info.GetDouble(nameof(LocationData.Latitude)));
        }
    }
}
