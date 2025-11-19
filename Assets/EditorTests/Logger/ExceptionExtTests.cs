using System;
using com.mapcolonies.core.Services.LoggerService.Extensions;
using NUnit.Framework;

namespace EditorTests.Logger
{
    public class ExceptionExtTests
    {
        [Test]
        public void UnityMessageWithStack_WhenExceptionProvided_ContainsMessageAndStack()
        {
            string result = "";

            try
            {
                ThrowTestException();
            }
            catch (Exception ex)
            {
                result = ex.UnityMessageWithStack();
            }

            Assert.IsNotNull(result);
            StringAssert.Contains("Test exception message", result);
            StringAssert.Contains(nameof(ThrowTestException), result);
        }

        private void ThrowTestException()
        {
            throw new InvalidOperationException("Test exception message");
        }
    }
}
