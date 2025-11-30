using com.mapcolonies.core.Services.LoggerService;
using NUnit.Framework;

namespace EditorTests.Logger
{
    public class LoggerServiceConfigTests
    {
        [Test]
        public void Init_WhenJsonExists_LoadsBasicProperties()
        {
            LoggerServiceConfig config = new LoggerServiceConfig();
            config.Init();
            Assert.IsTrue(config.Settings.ServiceEnabled);
            Assert.IsTrue(config.Settings.ConsoleEnabled);
            Assert.AreEqual("Logger/log4net.xml", config.Settings.Log4NetConfigXml);
            Assert.AreEqual("DEBUG", config.Settings.MinConsoleLogLevel);
        }
    }
}
