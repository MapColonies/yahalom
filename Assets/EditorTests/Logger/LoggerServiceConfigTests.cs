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
            Assert.IsTrue(config.ServiceEnabled);
            Assert.IsTrue(config.EnableConsole);
            Assert.AreEqual("Logger/log4net.xml", config.Log4NetConfigXml);
            Assert.AreEqual("DEBUG", config.MinConsoleLogLevel);
        }
    }
}
