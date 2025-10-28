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
        private AnalyticsManager _am;

        [SetUp]
        public void Setup()
        {
            _am = new AnalyticsManager();
            _am.Initialize();

            string dir = Path.Combine(Application.persistentDataPath, "AnalyticsLogs");
            _logPath = Path.Combine(dir, $"session-{_am.SessionId}.log");
            if (File.Exists(_logPath)) File.Delete(_logPath);
        }

        [Test]
        public async Task PublishApplicationPerformance_Writes_Performance_Log()
        {
            Type type = typeof(UsageAnalyticsManager);
            ConstructorInfo ctor = type.GetConstructor(new[] { typeof(IAnalyticsManager) });
            Assert.NotNull(ctor, "Expected ctor(IAnalyticsManager) on UsageAnalyticsManager");

            object instance = ctor.Invoke(new object[] { _am });

            MethodInfo mi = type.GetMethod("PublishApplicationPerformance", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(mi, "Expected private method PublishApplicationPerformance");

            mi.Invoke(instance, new object[] { 30f, 123.0d, 45.6d });

            await Task.Delay(100);

            Assert.IsTrue(File.Exists(_logPath), "Log file should exist");
            string content = await File.ReadAllTextAsync(_logPath);
            StringAssert.Contains("\"Fps\":30", content);
            StringAssert.Contains("\"AllocatedMemoryInMB\":123", content);
            StringAssert.Contains("\"CpuUsagePercentage\":45.6", content);
        }
    }
}
