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

        [SetUp]
        public void Setup()
        {
            new AnalyticsManager().Initialize();

            _logDirPath = Path.Combine(Application.persistentDataPath, AnalyticsManager.AnalyticsFileName);
            _logFilePath = Path.Combine(_logDirPath, $"session-{AnalyticsManager.SessionId}.log");

            if (File.Exists(_logFilePath))
            {
                File.Delete(_logFilePath);
            }
        }

        [TearDown]
        public void Teardown()
        {
            if (File.Exists(_logFilePath)) File.Delete(_logFilePath);
            if (Directory.Exists(_logDirPath) && Directory.GetFiles(_logDirPath).Length == 0) Directory.Delete(_logDirPath, true);
        }

        [Test]
        public async Task Publish_Writes_JSON_Line_To_Session_Log_File()
        {
            LayerData msgParams = LayerData.Create("imagery", "layer-abc");
            LogObject log = LogObject.Create(LogType.Log,
                AnalyticsMessageTypes.LayerUseStarted.ToString(),
                msgParams,
                "General",
                AnalyticsMessageTypes.LayerUseStarted);

            await AnalyticsManager.Publish(log);

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
            if (Directory.Exists(_logDirPath))
            {
                Directory.Delete(_logDirPath, true);
            }

            Assert.IsFalse(Directory.Exists(_logDirPath), "Directory should not exist before test");

            new AnalyticsManager().Initialize();

            LayerData msgParams = LayerData.Create("imagery", "layer-xyz");
            LogObject log = LogObject.Create(LogType.Log,
                AnalyticsMessageTypes.LayerUseStarted.ToString(),
                msgParams,
                "General",
                AnalyticsMessageTypes.LayerUseStarted);

            await AnalyticsManager.Publish(log);

            Assert.IsTrue(Directory.Exists(_logDirPath), "Directory should be created");
            Assert.IsTrue(File.Exists(_logFilePath), "Log file should be created");

            string content = await File.ReadAllTextAsync(_logFilePath);
            StringAssert.Contains("\"LayerDomain\":\"imagery\"", content);
        }
    }
}
