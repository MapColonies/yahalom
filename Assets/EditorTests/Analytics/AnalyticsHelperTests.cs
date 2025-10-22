using System.IO;
using System.Threading.Tasks;
using com.mapcolonies.core.Services.Analytics.Enums;
using com.mapcolonies.core.Services.Analytics.Managers;
using Domains.Analytics.Utilities;
using NUnit.Framework;
using UnityEngine;

namespace EditorTests.Analytics
{
    public class AnalyticsHelperTests
    {
        private AnalyticsManager _manager;
        private string _logFilePath;

        [SetUp]
        public void Setup()
        {
            _manager = new AnalyticsManager();
            _manager.Initialize();
            _logFilePath = Path.Combine(Application.persistentDataPath, "AnalyticsLogs", $"session-{AnalyticsManager.SessionId}.log");
            if (File.Exists(_logFilePath)) File.Delete(_logFilePath);
        }

        [Test]
        public async Task PublishLayerChange_Appends_To_Log_File()
        {
            AnalyticsHelper.PublishLayerChange(AnalyticsMessageTypes.LayerUseStarted, "basemap", "osm");
            await Task.Delay(100);

            Assert.IsTrue(File.Exists(_logFilePath));
            string content = await File.ReadAllTextAsync(_logFilePath);
            StringAssert.Contains("\"LayerDomain\": \"basemap\"", content);
            StringAssert.Contains("\"UniqueLayerId\": \"osm\"", content);
        }
    }
}
