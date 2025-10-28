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
        [Test]
        public async Task Publish_Writes_JSON_Line_To_Session_Log_File()
        {
            string logDirPath = Path.Combine(Application.persistentDataPath, AnalyticsManager.AnalyticsFileName);
            if (Directory.Exists(logDirPath)) Directory.Delete(logDirPath, true);

            AnalyticsManager am = new AnalyticsManager();

            try
            {
                am.Initialize();
                string logFilePath = Path.Combine(logDirPath, $"session-{am.SessionId}.log");

                LayerData msgParams = LayerData.Create("imagery", "layer-abc");
                LogObject log = LogObject.Create(
                    am.SessionId,
                    LogType.Log,
                    AnalyticsMessageTypes.LayerUseStarted.ToString(),
                    msgParams,
                    "General",
                    AnalyticsMessageTypes.LayerUseStarted);

                await am.Publish(log);

                Assert.IsTrue(File.Exists(logFilePath), $"Log file was not created at {logFilePath}");

                string content = await File.ReadAllTextAsync(logFilePath);
                StringAssert.Contains("\"LayerDomain\":\"imagery\"", content);
                StringAssert.Contains("\"UniqueLayerId\":\"layer-abc\"", content);
                StringAssert.Contains("\"MessageType\":" + (int)AnalyticsMessageTypes.LayerUseStarted, content);
                StringAssert.Contains("\"Severity\":\"Log\"", content);
            }
            finally
            {
                am.Dispose();
                if (Directory.Exists(logDirPath)) Directory.Delete(logDirPath, true);
            }
        }

        [Test]
        public async Task Publish_Creates_Directory_If_Not_Exists()
        {
            string logDirPath = Path.Combine(Application.persistentDataPath, AnalyticsManager.AnalyticsFileName);
            if (Directory.Exists(logDirPath)) Directory.Delete(logDirPath, true);
            Assert.IsFalse(Directory.Exists(logDirPath), "Directory should not exist before test");

            AnalyticsManager am = new AnalyticsManager();

            try
            {
                am.Initialize();
                string logFilePath = Path.Combine(logDirPath, $"session-{am.SessionId}.log");

                LayerData msgParams = LayerData.Create("imagery", "layer-xyz");
                LogObject log = LogObject.Create(
                    am.SessionId,
                    LogType.Log,
                    AnalyticsMessageTypes.LayerUseStarted.ToString(),
                    msgParams,
                    "General",
                    AnalyticsMessageTypes.LayerUseStarted);

                await am.Publish(log);

                Assert.IsTrue(Directory.Exists(logDirPath), "Directory should be created");
                Assert.IsTrue(File.Exists(logFilePath), $"Log file was not created at {logFilePath}");

                string content = await File.ReadAllTextAsync(logFilePath);
                StringAssert.Contains("\"LayerDomain\":\"imagery\"", content);
            }
            finally
            {
                am.Dispose();
                if (Directory.Exists(logDirPath)) Directory.Delete(logDirPath, true);
            }
        }
    }
}
