using System;
using System.Reflection;
using System.Threading.Tasks;
using com.mapcolonies.yahalom.InitPipeline.InitUnits;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PlayModeTests.InitPipelineTests
{
    public class InitUnitsTests
    {
        private LifetimeScope _parentScope;

        #region Test Controll Flow

        [SetUp]
        public void SetUp()
        {
            // Create a GameObject with a LifetimeScope to act as the parent
            GameObject go = new GameObject("ParentScope");
            _parentScope = go.AddComponent<LifetimeScope>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_parentScope != null)
            {
                GameObject.DestroyImmediate(_parentScope.gameObject);
                _parentScope = null;
            }
        }

        #endregion
        
        #region ActionUnitTests

        [Test(Description = "Ensures that the provided action is executed when RunAsync is called.")]
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

        [Test(Description = "Verifies that RunAsync throws ArgumentNullException if the action is null.")]
        public void RunAsync_NullAction_ShouldThrow()
        {
            ActionUnit unit = new ActionUnit("Test", 1f, InitPolicy.Fail, () => throw new ArgumentNullException());

            Assert.ThrowsAsync<ArgumentNullException>(async () => { await unit.RunAsync(); });
        }

        [Test(Description = "Checks that exceptions are propagated when InitPolicy is set to Fail.")]
        public void RunAsync_WithFailPolicy_ShouldPropagateException()
        {
            ActionUnit unit = new ActionUnit("Test", 1f, InitPolicy.Fail,
                () => { throw new InvalidOperationException("boom"); });

            Assert.ThrowsAsync<InvalidOperationException>(async () => await unit.RunAsync());
        }

        [Test(Description = "Verifies that exceptions are swallowed when InitPolicy is set to Ignore.")]
        public async Task RunAsync_WithContinuePolicy_ShouldSwallowException()
        {
            ActionUnit unit = new ActionUnit("Test", 1f, InitPolicy.Ignore,
                () => { throw new InvalidOperationException("boom"); });

            // Should not throw
            await unit.RunAsync();
        }

        [Test(Description = "Ensures that actions are retried once when InitPolicy is Retry and first attempt fails.")]
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

        [Test(Description = "Ensures that actions are retried once when InitPolicy is Retry and two attempts fails.")]
        public async Task Test_RunAsync_RetryPolicy_ShouldRetryAndFail()
        {
            // Arrange
            int callCount = 0;
            Func<UniTask> action = () =>
            {
                throw new InvalidOperationException("First call fails");
            };

            ActionUnit unit = new ActionUnit("RetryUnit", 1.0f, InitPolicy.Retry, action);

            // Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await unit.RunAsync());
        }
        
        #endregion

        #region RegisterScopeUnitTests

        [Test(Description =
            "Verifies that RunAsync creates a child scope, executes afterBuild callback, and registers dependencies.")]
        public async Task RunAsync_ShouldCreateChildScope_AndRunAfterBuild()
        {
            // Arrange
            bool afterBuildExecuted = false;

            Func<IObjectResolver, UniTask> afterBuild = resolver =>
            {
                afterBuildExecuted = true;
                return UniTask.CompletedTask;
            };

            RegisterScopeUnit unit = new RegisterScopeUnit(
                "TestScope",
                1.0f,
                _parentScope,
                InitPolicy.Fail,
                installers: builder => { builder.RegisterInstance("HelloWorld"); },
                afterBuild: afterBuild
            );

            // Act
            await unit.RunAsync();

            // Assert
            Assert.IsTrue(afterBuildExecuted, "AfterBuild should be executed");

            // Ensure child scope contains the registered dependency
            LifetimeScope str = unit.GetType().GetField("_child",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(unit) as LifetimeScope;

            Assert.NotNull(str, "Child scope should have been created");
            Assert.AreEqual("HelloWorld", str.Container.Resolve<string>());

            // Cleanup
            unit.Dispose();
        }

        [Test(Description = "Checks that Dispose properly disposes and nulls the child scope.")]
        public async Task Dispose_ShouldCleanChildScope()
        {
            // Arrange
            RegisterScopeUnit unit = new RegisterScopeUnit(
                "DisposableScope",
                1.0f,
                _parentScope,
                InitPolicy.Fail,
                installers: builder => { builder.RegisterInstance(123); });

            await unit.RunAsync();

            FieldInfo childField = unit.GetType().GetField("_child",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            LifetimeScope child = childField?.GetValue(unit) as LifetimeScope;

            Assert.NotNull(child);

            // Act
            unit.Dispose();

            // Assert
            object disposedChild = childField?.GetValue(unit);
            Assert.IsNull(disposedChild, "Child scope should be nulled after dispose");
        }

        #endregion
    }
}