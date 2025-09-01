using System;
using System.Threading.Tasks;
using com.mapcolonies.yahalom.InitPipeline.InitUnits;
using Cysharp.Threading.Tasks;
using NUnit.Framework;

namespace PlayModeTests.InitPipelineTests
{
    public class InitUnitsTests
    {
        [Test]
        public async Task RunAsync_ShouldExecuteProvidedAction()
        {
            bool executed = false;
            Func<UniTask> action = () =>
            {
                executed = true;
                return UniTask.CompletedTask;
            };
            ActionUnit unit = new ActionUnit("TestUnit", 1.0f, InitPolicy.Fail, action);

            await unit.RunAsync();

            Assert.IsTrue(executed);
        }

        [Test]
        public void RunAsync_NullAction_ShouldThrow()
        {
            ActionUnit unit = new ActionUnit("Test", 1f, InitPolicy.Fail, () => throw new ArgumentNullException());

            Assert.ThrowsAsync<ArgumentNullException>(async () => { await unit.RunAsync(); });
        }

        [Test]
        public void RunAsync_WithFailPolicy_ShouldPropagateException()
        {
            ActionUnit unit = new ActionUnit("Test", 1f, InitPolicy.Fail,
                () => { throw new InvalidOperationException("boom"); });

            Assert.ThrowsAsync<InvalidOperationException>(async () => await unit.RunAsync());
        }

        [Test]
        public async Task RunAsync_WithContinuePolicy_ShouldSwallowException()
        {
            ActionUnit unit = new ActionUnit("Test", 1f, InitPolicy.Ignore,
                () => { throw new InvalidOperationException("boom"); });

            // Should not throw
            await unit.RunAsync();
        }

        [Test]
        public async Task Test_RunAsync_RetryPolicy_ShouldRetryAndSucceed()
        {
            // Arrange
            int callCount = 0;
            Func<UniTask> action = () =>
            {
                callCount++;
                if (callCount == 1)
                {
                    throw new InvalidOperationException("First call fails");
                }

                return UniTask.CompletedTask; // second call succeeds
            };

            ActionUnit unit = new ActionUnit("RetryUnit", 1.0f, InitPolicy.Retry, action);

            // Act
            await unit.RunAsync();

            // Assert
            Assert.AreEqual(2, callCount, "Action should be executed twice (fail then retry)");
        }
    }
}