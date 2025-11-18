using System.Collections.Generic;
using System.Linq;
using com.mapcolonies.yahalom.InitPipeline.InitSteps;
using com.mapcolonies.yahalom.InitPipeline.InitUnits;
using com.mapcolonies.yahalom.Preloader;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace com.mapcolonies.yahalom.InitPipeline
{
    public class InitializationPipeline
    {
        private readonly List<InitStep> _initSteps;
        private readonly LifetimeScope _parent;
        private readonly PreloaderViewModel _preloader;

        public InitializationPipeline(PreloaderViewModel preloader)
        {
            _preloader = preloader;
        }

        public async UniTask RunAsync(
            IEnumerable<InitStep> initSteps,
            float startPercentage = 0f,
            float endPercentage = 1f)
        {
            float total = initSteps.SelectMany(s => s.InitUnits).Sum(u => u.Weight);
            float accumulated = 0f;

            foreach (InitStep step in initSteps)
            {
                Debug.Log($"Enter Init Step {step.Name}");

                switch (step.Mode)
                {
                    case StepMode.Sequential:
                        foreach (IInitUnit initUnit in step.InitUnits)
                        {
                            await initUnit.RunAsync();

                            accumulated += initUnit.Weight / total;
                            float mappedProgress = Mathf.Lerp(startPercentage, endPercentage, accumulated);

                            _preloader.ReportProgress($"{step.Name} .. {initUnit.Name}", mappedProgress);
                        }
                        break;

                    case StepMode.Parallel:
                        float[] weights = step.InitUnits.Select(s => s.Weight / total).ToArray();
                        await UniTask.WhenAll(step.InitUnits.Select(u => u.RunAsync()));

                        accumulated += weights.Sum();
                        float mappedStepProgress = Mathf.Lerp(startPercentage, endPercentage, accumulated);

                        _preloader.ReportProgress(step.Name, mappedStepProgress);
                        break;

                    default:
                        Debug.LogError($"Unknown step mode {step.Mode}");
                        break;
                }

                Debug.Log($"Exit Init Step {step.Name}");
            }

            _preloader.ReportProgress("Complete", endPercentage);
        }

    }
}
