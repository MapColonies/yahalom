using System.IO;
using System.Threading.Tasks;
using com.mapcolonies.core.Services.Analytics.Enums;
using com.mapcolonies.core.Services.Analytics.Managers;
using com.mapcolonies.core.Services.Analytics.Model;
using NUnit.Framework;
using UnityEngine;

namespace EditorTests.Analytics
{
    public class AnalyticsManagerFileWriteTests
    {
        private string _logDirPath;
        private string _logFilePath;
        private AnalyticsManager _am;

        [SetUp]
        public void Setup()
        {
            _logDirPath = Path.Combine(Application.persistentDataPath, AnalyticsManager.AnalyticsFileName);

            if (Directory.Exists(_logDirPath))
            {
                Directory.Delete(_logDirPath, true);
            }
        }

        [TearDown]
        public void Teardown()
        {
            _am?.Dispose();
            _am = null;

            if (Directory.Exists(_logDirPath))
            {
                Directory.Delete(_logDirPath, true);
            }
        }

        [Test]
        public async Task Publish_Writes_JSON_Line_To_Session_Log_File()
        {
            _am = new AnalyticsManager();
            _am.Initialize();
            _logFilePath = Path.Combine(_logDirPath, $"session-{_am.SessionId}.log");

            LayerData msgParams = LayerData.Create("imagery", "layer-abc");
            LogObject log = LogObject.Create(_am.SessionId,LogType.Log,
                AnalyticsMessageTypes.LayerUseStarted.ToString(),
                msgParams,
                "General",
                AnalyticsMessageTypes.LayerUseStarted);

            await _am.Publish(log);

            Assert.IsTrue(File.Exists(_logFilePath), $"Log file was not created at {_logFilePath}");

            string content = await File.ReadAllTextAsync(_logFilePath);
            StringAssert.Contains("\"LayerDomain\":\"imagery\"", content);
            StringAssert.Contains("\"UniqueLayerId\":\"layer-abc\"", content);
            StringAssert.Contains("\"MessageType\":" + (int)AnalyticsMessageTypes.LayerUseStarted, content);
            StringAssert.Contains("\"Severity\":\"Log\"", content);
        }

        [Test]
        public async Task Publish_Creates_Directory_If_Not_Exists()
        {
            Assert.IsFalse(Directory.Exists(_logDirPath), "Directory should not exist before test");

            _am = new AnalyticsManager();
            _am.Initialize();

            _logFilePath = Path.Combine(_logDirPath, $"session-{_am.SessionId}.log");

            LayerData msgParams = LayerData.Create("imagery", "layer-xyz");
            LogObject log = LogObject.Create(_am.SessionId,LogType.Log,
                AnalyticsMessageTypes.LayerUseStarted.ToString(),
                msgParams,
                "General",
                AnalyticsMessageTypes.LayerUseStarted);

            await _am.Publish(log);

            Assert.IsTrue(Directory.Exists(_logDirPath), "Directory should be created");
            Assert.IsTrue(File.Exists(_logFilePath), $"Log file was not created at {_logFilePath}");

            string content = await File.ReadAllTextAsync(_logFilePath);
            StringAssert.Contains("\"LayerDomain\":\"imagery\"", content);
        }
    }
}
