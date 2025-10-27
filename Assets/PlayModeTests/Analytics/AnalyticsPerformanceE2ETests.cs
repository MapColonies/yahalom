using System.Collections;
using System.IO;
using System.Threading.Tasks;
using com.mapcolonies.core.Services.Analytics.Enums;
using com.mapcolonies.core.Services.Analytics.Managers;
using com.mapcolonies.core.Services.Analytics.Model;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PlayModeTests.Analytics
{
    public class AnalyticsPerformanceE2ETests
    {
        private string _logDir;
        private string _logPath;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            AnalyticsManager mgr = new AnalyticsManager();
            mgr.Initialize();

            _logDir  = Path.Combine(Application.persistentDataPath, "AnalyticsLogs");
            _logPath = Path.Combine(_logDir, $"session-{AnalyticsManager.SessionId}.log");
            if (File.Exists(_logPath)) File.Delete(_logPath);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Publish_Performance_Snapshot_Writes_To_File()
        {
            PerformanceData perf = PerformanceData.Create(fps: 60f, allocatedMemoryInMb: 256d, cpuUsagePercentage: 12.34d);
            LogObject log  = LogObject.Create(
                LogType.Log,
                AnalyticsMessageTypes.ConsumptionStatus.ToString(),
                perf,
                LogComponent.General,
                AnalyticsMessageTypes.ConsumptionStatus);

            Task task = AnalyticsManager.Publish(log);
            yield return new WaitUntil(() => task.IsCompleted);

            Assert.IsTrue(File.Exists(_logPath), "Log file should exist after publish");
            string content = File.ReadAllText(_logPath);

            StringAssert.Contains("\"Fps\":60", content);
            StringAssert.Contains("\"AllocatedMemoryInMB\":256", content);
            StringAssert.Contains("\"CpuUsagePercentage\":12.34", content);
            StringAssert.Contains("\"MessageType\":" + (int)AnalyticsMessageTypes.ConsumptionStatus, content);
        }
    }
}
