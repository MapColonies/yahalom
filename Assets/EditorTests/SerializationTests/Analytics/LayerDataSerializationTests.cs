using System.Runtime.Serialization;
using com.mapcolonies.core.Services.Analytics.Model;
using NUnit.Framework;

namespace EditorTests.SerializationTests.Analytics
{
    public class LayerDataSerializationTests
    {
        [Test]
        public void LayerData_Serializes_Expected_Fields()
        {
            LayerData data = LayerData.Create("elevation", "lyr-42");
            SerializationInfo info = new SerializationInfo(typeof(LayerData), new FormatterConverter());
            data.GetObjectData(info, new StreamingContext());

            Assert.AreEqual("elevation", info.GetString(nameof(LayerData.LayerDomain)));
            Assert.AreEqual("lyr-42", info.GetString(nameof(LayerData.UniqueLayerId)));
        }
    }
}
