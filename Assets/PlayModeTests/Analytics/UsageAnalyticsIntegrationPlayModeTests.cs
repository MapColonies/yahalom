using System.Collections;
using System.IO;
using System.Reflection;
using com.mapcolonies.core.Services.Analytics.Managers;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PlayModeTests.Analytics
{
    public class UsageAnalyticsIntegrationPlayModeTests
    {
        private string _logDir;
        private string _logPath;

        private class UsageAnalyticsShim : UsageAnalyticsManager
        {
            public void ForceOnePublish()
            {
                MethodInfo mi = typeof(UsageAnalyticsManager).GetMethod("PublishApplicationPerformance", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(this, new object[] { 25f, 200.0d, 15.0d });
            }
        }

        [UnitySetUp]
        public IEnumerator Setup()
        {
            AnalyticsManager am = new AnalyticsManager();
            am.Initialize();
            _logDir = Path.Combine(Application.persistentDataPath, "AnalyticsLogs");
            _logPath = Path.Combine(_logDir, $"session-{AnalyticsManager.SessionId}.log");
            if (File.Exists(_logPath)) File.Delete(_logPath);
            yield return null;
        }

        [UnityTest]
        public IEnumerator TimerController_To_FileWrite_EndToEnd_Works()
        {
            UsageAnalyticsShim mgr = new UsageAnalyticsShim();
            mgr.Initialize();
            mgr.ForceOnePublish();

            yield return new WaitForSeconds(0.2f);

            Assert.IsTrue(File.Exists(_logPath), "Expected analytics log file to exist.");
            string content = File.ReadAllText(_logPath);
            StringAssert.Contains("\"Fps\": 25", content);
            StringAssert.Contains("\"AllocatedMemoryInMB\": 200", content);
            StringAssert.Contains("\"CpuUsagePercentage\": 15", content);

            mgr.ForceOnePublish();
            yield return new WaitForSeconds(0.1f);

            string secondContent = File.ReadAllText(_logPath);
            int lineCount = secondContent.Split('\n').Length;
            Assert.GreaterOrEqual(lineCount, 2, "Expected at least 2 JSON lines after second publish.");
        }
    }
}
