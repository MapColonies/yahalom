using System.Collections;
using System.Reflection;
using com.mapcolonies.core.Services.LoggerService;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PlayModeTests.Logger
{
    internal class LoggerServicePlayModeLoggingTests
    {
        private ILogHandler _originalHandler;

        [SetUp]
        public void SetUp()
        {
            _originalHandler = Debug.unityLogger.logHandler;
            LogAssert.ignoreFailingMessages = true;
        }

        [TearDown]
        public void TearDown()
        {
            LogAssert.ignoreFailingMessages = false;
            MethodInfo m = typeof(LoggerInitializer).GetMethod("Dispose", BindingFlags.NonPublic | BindingFlags.Static);
            m?.Invoke(null, null);
            Debug.unityLogger.logHandler = _originalHandler;
        }

        [UnityTest]
        public IEnumerator Logging_FromMonoBehaviour_DoesNotThrow()
        {
            LoggerInitializer.Init();
            Assert.IsInstanceOf<Log4NetHandler>(Debug.unityLogger.logHandler);

            GameObject go = new GameObject("LoggerTestObject");
            go.AddComponent<TestLoggerBehaviour>();

            yield return null;
            yield return null;
        }

        private class TestLoggerBehaviour : MonoBehaviour
        {
            private void Start()
            {
                Debug.Log("PlayMode logger test - Log");
                Debug.LogWarning("PlayMode logger test - Warning");
                Debug.LogError("PlayMode logger test - Error");
            }
        }
    }
}
