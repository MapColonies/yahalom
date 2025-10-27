using System;
using System.Collections;
using com.mapcolonies.core.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PlayModeTests.Analytics
{
    public class PlatformUsageManagerPlayModeTests
    {
        [UnityTest]
        public IEnumerator GetApplicationPerformanceSnapshot_Returns_Sane_Values()
        {
            (DateTime startTime, TimeSpan startSpan) = PlatformUsageHelper.GetInitialProcessorTimes();

            yield return new WaitForSeconds(0.1f);

            (float fps, double memMb, double cpuPct, _, _) = PlatformUsageHelper.GetApplicationPerformanceSnapshot(startTime, startSpan);
            Assert.Greater(fps, 0f, "FPS should be > 0");

            Assert.GreaterOrEqual(memMb, 0d);
            Assert.Less(memMb, 1024d * 32d, "Allocated memory seems unreasonably large for a test run");

            Assert.IsFalse(double.IsNaN(cpuPct));
            Assert.IsFalse(double.IsInfinity(cpuPct));
        }
    }
}
