using com.mapcolonies.core.Utilities;
using NUnit.Framework;

namespace EditorTests.Analytics
{
    public class TimeUtilitiesTests
    {
        [Test]
        public void SecondsToMilliseconds_Works()
        {
            Assert.AreEqual(1500d, TimeUtilities.SecondsToMilliseconds(1.5d));
            Assert.AreEqual(0d, TimeUtilities.SecondsToMilliseconds(0d));
        }

        [Test]
        public void MillisecondsToSeconds_Works()
        {
            Assert.AreEqual(2.5d, TimeUtilities.MillisecondsToSeconds(2500d));
            Assert.AreEqual(0d, TimeUtilities.MillisecondsToSeconds(0d));
        }
    }
}
