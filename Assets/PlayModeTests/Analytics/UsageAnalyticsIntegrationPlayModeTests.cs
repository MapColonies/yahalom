using System.Collections;
using System.IO;
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
        private UsageAnalyticsShim _mgr;
        private AnalyticsManager _am;

        private class UsageAnalyticsShim : UsageAnalyticsManager
        {
            public UsageAnalyticsShim(IAnalyticsManager analyticsManager) : base(analyticsManager)
            {
            }

            public void ForceOnePublish()
            {
                base.PublishApplicationPerformance(25f, 200.0d, 15.0d);
            }
        }

        [UnitySetUp]
        public IEnumerator Setup()
        {
            _am = new AnalyticsManager();
            _am.Initialize();

            _logDir = Path.Combine(Application.persistentDataPath, "AnalyticsLogs");
            _logPath = Path.Combine(_logDir, $"session-{_am.SessionId}.log");
            if (File.Exists(_logPath)) File.Delete(_logPath);

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            _mgr?.Dispose();
            _mgr = null;

            _am?.Dispose();
            _am = null;

            yield return null;
        }

        [UnityTest]
        public IEnumerator TimerController_To_FileWrite_EndToEnd_Works()
        {
            _mgr = new UsageAnalyticsShim(_am);
            _mgr.Initialize();
            _mgr.ForceOnePublish();

            yield return new WaitForSeconds(0.2f);

            Assert.IsTrue(File.Exists(_logPath), "Expected analytics log file to exist.");
            string content = File.ReadAllText(_logPath);
            StringAssert.Contains("\"Fps\":25", content);
            StringAssert.Contains("\"AllocatedMemoryInMB\":200", content);
            StringAssert.Contains("\"CpuUsagePercentage\":15", content);

            _mgr.ForceOnePublish();
            yield return new WaitForSeconds(0.1f);

            string secondContent = File.ReadAllText(_logPath);
            int lineCount = secondContent.Split('\n').Length;
            Assert.GreaterOrEqual(lineCount, 2, "Expected at least 2 JSON lines after second publish.");
        }
    }
}
