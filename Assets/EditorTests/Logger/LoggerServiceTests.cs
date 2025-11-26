using com.mapcolonies.core.Services.LoggerService;
using NUnit.Framework;
using UnityEngine;

namespace EditorTests.Logger
{
    public class LoggerServiceTests
    {
        private ILogHandler _originalHandler;

        [SetUp]
        public void SetUp()
        {
            _originalHandler = Debug.unityLogger.logHandler;
        }

        [TearDown]
        public void TearDown()
        {
            Debug.unityLogger.logHandler = _originalHandler;
        }

        [Test]
        public void Ctor_WhenConfigValid_ReplacesUnityLogHandlerWithLog4NetHandler()
        {
            LoggerServiceConfig config = new LoggerServiceConfig();
            config.Init();

            ILogHandler before = Debug.unityLogger.logHandler;
            LoggerService loggerService = null;

            try
            {
                loggerService = new LoggerService(config);
                Assert.IsInstanceOf<Log4NetHandler>(Debug.unityLogger.logHandler);
                Assert.IsTrue(Debug.unityLogger.logEnabled);
            }
            finally
            {
                loggerService?.Dispose();
                Assert.AreSame(before, Debug.unityLogger.logHandler);
            }
        }
    }
}
