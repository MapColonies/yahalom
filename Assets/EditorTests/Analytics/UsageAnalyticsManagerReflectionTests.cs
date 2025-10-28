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
        [Test]
        public async Task PublishApplicationPerformance_Writes_Performance_Log()
        {
            AnalyticsManager am = new AnalyticsManager();
            object instance = null;

            try
            {
                am.Initialize();

                string dir = Path.Combine(Application.persistentDataPath, AnalyticsManager.AnalyticsFileName);
                string path = Path.Combine(dir, $"session-{am.SessionId}.log");
                if (File.Exists(path)) File.Delete(path);

                Assert.IsFalse(File.Exists(path), "File should not exist before PublishApplicationPerformance invocation");

                Type type = typeof(UsageAnalyticsManager);
                ConstructorInfo ctor = type.GetConstructor(new[] { typeof(IAnalyticsManager) });
                Assert.NotNull(ctor, "Expected ctor(IAnalyticsManager) on UsageAnalyticsManager");

                instance = ctor.Invoke(new object[] { am });

                MethodInfo mi = type.GetMethod("PublishApplicationPerformance", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.NotNull(mi, "Expected private method PublishApplicationPerformance");

                mi.Invoke(instance, new object[] { 30f, 123.0d, 45.6d });

                await Task.Delay(100);

                Assert.IsTrue(File.Exists(path), "Log file should exist");
                string content = await File.ReadAllTextAsync(path);
                StringAssert.Contains("\"Fps\":30", content);
                StringAssert.Contains("\"AllocatedMemoryInMB\":123", content);
                StringAssert.Contains("\"CpuUsagePercentage\":45.6", content);
            }
            finally
            {
                (instance as IDisposable)?.Dispose();
                am.Dispose();
            }
        }
    }
}
