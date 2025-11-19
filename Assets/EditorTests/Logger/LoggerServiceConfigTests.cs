using System.IO;
using com.mapcolonies.core.Services.LoggerService;
using NUnit.Framework;

namespace EditorTests.Logger
{
    public class LoggerServiceConfigTests
    {
        [Test]
        public void Init_WhenJsonExists_LoadsAllProperties()
        {
            LoggerServiceConfig config = new LoggerServiceConfig();
            config.Init();
            Assert.IsTrue(config.ServiceEnabled);
            Assert.IsTrue(config.EnableConsole);
            Assert.AreEqual("Logger/log4net.xml", config.Log4NetConfigXml);
            Assert.AreEqual("DEBUG", config.MinLogLevel);
        }

        [Test]
        public void GetSystemLogsDirectory_ReturnsNonEmptyPath()
        {
            LoggerServiceConfig config = new LoggerServiceConfig();
            string logsDir = config.GetSystemLogsDirectory();
            Assert.IsFalse(string.IsNullOrEmpty(logsDir));
            Assert.IsTrue(Path.IsPathRooted(logsDir));
        }

        [Test]
        public void GetHttpPersistenceDirectory_UsesLogsDirectoryOfflineSubfolder()
        {
            LoggerServiceConfig config = new LoggerServiceConfig();
            string httpDir = config.GetHttpPersistenceDirectory();
            Assert.IsFalse(string.IsNullOrEmpty(httpDir));
            StringAssert.Contains("offline", httpDir.ToLowerInvariant());
        }
    }
}
