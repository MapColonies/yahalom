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
        

    }
}