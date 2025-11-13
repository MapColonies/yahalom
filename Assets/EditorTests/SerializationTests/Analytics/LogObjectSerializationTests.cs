using com.mapcolonies.core.Services.Analytics.Enums;
using com.mapcolonies.core.Services.Analytics.Model;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;

namespace EditorTests.SerializationTests.Analytics
{
    public class LogObjectSerializationTests
    {
        [Test]
        public void Serialize_ToJson_Contains_Expected_Fields()
        {
            var severity = LogType.Log;
            var message = "LayerUseStarted";
            var parameters = LayerData.Create("imagery", "layer-001");
            var component = "General";
            var type = AnalyticsMessageTypes.LayerUseStarted;

            var log = LogObject.Create(severity, message, parameters, component, type);

            string json = JsonConvert.SerializeObject(log, Formatting.None);

            StringAssert.Contains("\"Severity\":\"Log\"", json);
            StringAssert.Contains("\"Message\":\"LayerUseStarted\"", json);
            StringAssert.Contains("\"Component\":\"General\"", json);
            StringAssert.Contains($"\"MessageType\":{(int)AnalyticsMessageTypes.LayerUseStarted}", json);
            StringAssert.Contains("\"LayerDomain\":\"imagery\"", json);
            StringAssert.Contains("\"UniqueLayerId\":\"layer-001\"", json);
            StringAssert.Contains("\"TimeStamp\"", json);
        }
    }
}
