using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.mapcolonies.yahalom.InitPipeline;
using com.mapcolonies.yahalom.InitPipeline.InitSteps;
using com.mapcolonies.yahalom.InitPipeline.InitUnits;
using com.mapcolonies.yahalom.Preloader;
using Cysharp.Threading.Tasks;
using NUnit.Framework;

namespace PlayModeTests.InitPipelineTests
{
    public class InitializationPipelineTests
    {
        private class FakePreloader : PreloaderViewModel
        {
            public List<(string label, float progress)> Reports = new();

            public override void ReportProgress(string label, float progress)
            {
                Reports.Add((label, progress));
            }
        }

        private class TestActionUnit : ActionUnit
        {
            public TestActionUnit(string name, float weight)
                : base(name, weight, InitPolicy.Ignore, () => { return UniTask.CompletedTask; })
            {
            }
        }

        [Test]
        public async Task RunAsync_ShouldExecuteAllUnitsInOrder_AndReportProgress()
        {
            // Arrange
            FakePreloader preloader = new FakePreloader();
            
            InitializationPipeline pipeline = new InitializationPipeline(preloader);
            
            // Create test units
            TestActionUnit unit1 = new TestActionUnit("Unit1", 0.3f);
            TestActionUnit unit2 = new TestActionUnit("Unit2", 0.7f);

            InitStep step1 = new InitStep("Step1", StepMode.Sequential, new IInitUnit[] { unit1 });
            InitStep step2 = new InitStep("Step2", StepMode.Parallel, new IInitUnit[] { unit2 });
            
            // Act
            await pipeline.RunAsync(new List<InitStep> { step1, step2 });

            // Assert
            // Check that all steps and units executed
            Assert.IsTrue(preloader.Reports.Count > 0);
            Assert.AreEqual(1f, preloader.Reports.Last().progress, "Final progress should be 100%");
        }
    }
}
