using System;
using com.mapcolonies.core.Services.LoggerService;
using NUnit.Framework;
using UnityEngine;

namespace EditorTests.Logger
{
    public class Log4NetHandlerTests
    {
        [Test]
        public void LogException_WhenExceptionIsNull_DoesNotThrow()
        {
            Log4NetHandler handler = new Log4NetHandler();
            Assert.DoesNotThrow(() => handler.LogException(null, null));
        }

        [Test]
        public void LogException_WhenExceptionProvided_DoesNotThrow()
        {
            Log4NetHandler handler = new Log4NetHandler();
            Assert.DoesNotThrow(() => handler.LogException(new Exception("error"), null));
        }

        [Test]
        public void LogFormat_DoesNotThrow_ForSimpleLog()
        {
            Log4NetHandler handler = new Log4NetHandler();
            Assert.DoesNotThrow(() =>
                handler.LogFormat(LogType.Log, null, "Hello {0}", "World"));
        }
    }
}
