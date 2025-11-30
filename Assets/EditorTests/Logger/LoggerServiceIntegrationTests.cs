using System.IO;
using com.mapcolonies.core.Services.LoggerService;
using NUnit.Framework;
using UnityEngine;

namespace EditorTests.Logger
{
    public class LoggerServiceIntegrationTests
    {
        [Test]
        public void DebugLog_WritesToInfoLogFile()
        {
            LoggerServiceConfig config = new LoggerServiceConfig();
            config.Init();

            Assert.IsTrue(config.Settings.ServiceEnabled, "ServiceEnabled must be true for this integration test.");
            Assert.IsFalse(string.IsNullOrEmpty(config.Settings.Log4NetConfigXml), "Log4NetConfigXml must point to a valid XML file.");

            string logsDir = config.GetSystemLogsDirectory();
            Directory.CreateDirectory(logsDir);

            foreach (string file in Directory.GetFiles(logsDir, "info.log*"))
            {
                File.Delete(file);
            }

            ILogHandler originalHandler = Debug.unityLogger.logHandler;

            using (new LoggerService(config))
            {
                Assert.IsInstanceOf<Log4NetHandler>(Debug.unityLogger.logHandler, "LoggerService must replace Unity's log handler with Log4NetHandler when initialized.");

                Debug.Log("IntegrationTest: DebugLog_WritesToInfoLogFile");
            }

            Debug.unityLogger.logHandler = originalHandler;

            string[] infoFiles = Directory.GetFiles(logsDir, "info.log*");
            Assert.IsNotEmpty(infoFiles, $"No info.log* files were found after logging. Log directory: {logsDir}");

            FileInfo fi = new FileInfo(infoFiles[0]);
            Assert.Greater(fi.Length, 0, $"The log file '{fi.FullName}' is empty, but it should contain at least one log entry.");
        }
    }
}
