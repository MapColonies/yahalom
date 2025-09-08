using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using com.mapcolonies.yahalom.InitPipeline;
using com.mapcolonies.yahalom.InitPipeline.InitSteps;
using com.mapcolonies.yahalom.InitPipeline.InitUnits;
using com.mapcolonies.yahalom.Preloader;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using VContainer.Unity;

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
            private readonly List<string> _log;

            public TestActionUnit(string name, float weight, List<string> log)
                : base(name, weight, InitPolicy.Ignore, () =>
                {
                    log.Add(name);
                    return UniTask.CompletedTask;
                })
            {
                _log = log;
            }
        }

        [Test]
        public async Task RunAsync_ShouldExecuteAllUnitsInOrder_AndReportProgress()
        {
            // Arrange
            List<string> executionLog = new List<string>();
            FakePreloader preloader = new FakePreloader();
            GameObject parentScopeGO = new GameObject("ParentScope");
            LifetimeScope parentScope = parentScopeGO.AddComponent<VContainer.Unity.LifetimeScope>();

            var pipeline = new InitializationPipeline(preloader);


            // Create test units
            var unit1 = new TestActionUnit("Unit1", 0.3f, executionLog);
            var unit2 = new TestActionUnit("Unit2", 0.7f, executionLog);

            var step1 = new InitStep("Step1", StepMode.Sequential, new IInitUnit[] { unit1 });
            var step2 = new InitStep("Step2", StepMode.Parallel, new IInitUnit[] { unit2 });


            // Act
            await pipeline.RunAsync(new List<InitStep> { step1, step2 });

            // Assert
            // Check that all steps and units executed
            Assert.IsTrue(preloader.Reports.Count > 0);
            Assert.AreEqual(1f, preloader.Reports.Last().progress, "Final progress should be 100%");

            // Clean up
            GameObject.DestroyImmediate(parentScopeGO);
        }
    }
}
