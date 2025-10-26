using System;
using com.mapcolonies.core.Utilities;
using NUnit.Framework;

namespace EditorTests.Analytics
{
    public class TimeUtilitiesTests
    {
        [Test]
        public void SecondsToMilliseconds_Works()
        {
            double seconds = 1.5d;
            double expectedMilliseconds = TimeSpan.FromSeconds(seconds).TotalMilliseconds;

            var timeSpan = TimeSpan.FromSeconds(seconds);
            double result = timeSpan.TotalMilliseconds;

            Assert.AreEqual(expectedMilliseconds, result);
            Assert.AreEqual(0d, TimeSpan.FromSeconds(0d).TotalMilliseconds);
        }

        [Test]
        public void MillisecondsToSeconds_Works()
        {
            double milliseconds = 2500d;
            double expectedSeconds = 2.5d;

            var timeSpan = TimeSpan.FromMilliseconds(milliseconds);
            double result = timeSpan.TotalSeconds;

            Assert.AreEqual(expectedSeconds, result);
            Assert.AreEqual(0d, TimeSpan.FromMilliseconds(0d).TotalSeconds);
        }
    }
}
