using System.Collections;
using System.Reflection;
using com.mapcolonies.core.Services.LoggerService;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PlayModeTests.Logger
{
    public class LoggerInitializerPlayModeTests
    {
        private ILogHandler _originalHandler;

        [SetUp]
        public void SetUp()
        {
            _originalHandler = Debug.unityLogger.logHandler;
            Log4NetHandler.ApplicationDataPath = null;
            Log4NetHandler.UnityVersion = null;
        }

        [TearDown]
        public void TearDown()
        {
            TryCallDispose();
            Debug.unityLogger.logHandler = _originalHandler;
        }

        [UnityTest]
        public IEnumerator Init_SetsHandlerAndStaticFields_AndDisposeRestoresHandler()
        {
            LoggerInitializer.Init();
            yield return null;

            Assert.IsInstanceOf<Log4NetHandler>(Debug.unityLogger.logHandler);
            Assert.AreEqual(Application.dataPath, Log4NetHandler.ApplicationDataPath);
            Assert.NotNull(Log4NetHandler.UnityVersion);

            TryCallDispose();
            Assert.AreSame(_originalHandler, Debug.unityLogger.logHandler);
        }

        private void TryCallDispose()
        {
            MethodInfo m = typeof(LoggerInitializer).GetMethod("Dispose", BindingFlags.NonPublic | BindingFlags.Static);
            m?.Invoke(null, null);
        }
    }
}
