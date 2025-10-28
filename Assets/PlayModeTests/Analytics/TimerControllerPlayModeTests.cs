using System.Collections;
using com.mapcolonies.core.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PlayModeTests.Analytics
{
    public class TimerControllerPlayModeTests
    {
        [UnityTest]
        public IEnumerator TimerController_Fires_Callback_On_Main_Thread()
        {
            int count = 0;

            using (var timer = new TimerController(interval: 25d, isRepeating: true))
            {
                timer.OnTimerElapsed = () =>
                {
                    count++;
                };
                timer.StartTimer();

                yield return new WaitForSeconds(0.2f);
                timer.Stop();
            }

            Assert.GreaterOrEqual(count, 3);
        }
    }
}
