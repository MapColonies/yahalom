using System.Reflection;
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
        public void Ctor_WhenServiceDisabled_DoesNotReplaceUnityLogHandler()
        {
            LoggerServiceConfig config = new LoggerServiceConfig();
            ILogHandler before = Debug.unityLogger.logHandler;

            using (LoggerService loggerService = new LoggerService(config))
            {
                Assert.AreSame(before, Debug.unityLogger.logHandler);
            }
        }

        [Test]
        public void Ctor_WhenLog4NetInitializationFails_RestoresOriginalHandler()
        {
            LoggerServiceConfig config = CreateConfigWithReflection(true, true, "DEBUG", "Logger/non_existing_file.xml");
            ILogHandler before = Debug.unityLogger.logHandler;

            using (LoggerService loggerService = new LoggerService(config))
            {
                Assert.AreSame(before, Debug.unityLogger.logHandler);
            }
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

        private LoggerServiceConfig CreateConfigWithReflection(bool serviceEnabled, bool enableConsole, string minLogLevel, string log4NetConfigXml)
        {
            LoggerServiceConfig config = new LoggerServiceConfig();
            SetPrivateProperty(config, "ServiceEnabled", serviceEnabled);
            SetPrivateProperty(config, "EnableConsole", enableConsole);
            SetPrivateProperty(config, "MinLogLevel", minLogLevel);
            SetPrivateProperty(config, "Log4NetConfigXml", log4NetConfigXml);
            return config;
        }

        private void SetPrivateProperty<T>(object target, string propName, T value)
        {
            PropertyInfo prop = target.GetType().GetProperty(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            prop.SetValue(target, value);
        }
    }
}
