using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using com.mapcolonies.core.Services.Analytics.Managers;
using NUnit.Framework;
using UnityEngine;

namespace EditorTests.Analytics
{
    public class UsageAnalyticsManagerReflectionTests
    {
        private string _logPath;

        [SetUp]
        public void Setup()
        {
            AnalyticsManager am = new AnalyticsManager();
            am.Initialize();

            string dir = Path.Combine(Application.persistentDataPath, "AnalyticsLogs");
            _logPath = Path.Combine(dir, $"session-{AnalyticsManager.SessionId}.log");
            if (File.Exists(_logPath)) File.Delete(_logPath);
        }

        [Test]
        public async Task PublishApplicationPerformance_Writes_Performance_Log()
        {
            Type type = typeof(UsageAnalyticsManager);
            object instance = System.Activator.CreateInstance(type);

            MethodInfo mi = type.GetMethod("PublishApplicationPerformance", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(mi, "Expected private method PublishApplicationPerformance");

            mi.Invoke(instance, new object[] { 30f, 123.0d, 45.6d });

            await Task.Delay(100);

            Assert.IsTrue(File.Exists(_logPath), "Log file should exist");
            string content = await File.ReadAllTextAsync(_logPath);
            StringAssert.Contains("\"Fps\": 30", content);
            StringAssert.Contains("\"AllocatedMemoryInMB\": 123", content);
            StringAssert.Contains("\"CpuUsagePercentage\": 45.6", content);
        }
    }
}
