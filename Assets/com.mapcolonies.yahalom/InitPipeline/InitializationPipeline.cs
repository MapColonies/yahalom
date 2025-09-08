using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using com.mapcolonies.core.Services;
using com.mapcolonies.yahalom.InitPipeline.InitSteps;
using com.mapcolonies.yahalom.InitPipeline.InitUnits;
using com.mapcolonies.yahalom.Preloader;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace com.mapcolonies.yahalom.InitPipeline
{
    public class InitializationPipeline
    {
        private readonly List<InitStep> _initSteps;
        private readonly LifetimeScope _parent;
        private readonly PreloaderViewModel _preloader;

        public InitializationPipeline(PreloaderViewModel preloader, LifetimeScope scope)
        {
            _preloader = preloader;
            _parent = scope;

            //TODO: update initialization steps
            _initSteps = new List<InitStep>
            {
                new("PreInit", StepMode.Sequential, new IInitUnit[]
                {
                    new ActionUnit("Logging Init", 0.05f, InitPolicy.Fail, () =>
                    {
                        return UniTask.Delay(1000);
                    }),
                    new ActionUnit("Local Settings", 0.05f, InitPolicy.Fail, () =>
                    {
                        return UniTask.Delay(1000);
                    })
                }),
                new("ServicesInit", StepMode.Sequential, new IInitUnit[]
                {
                    new RegisterScopeUnit("WMTS", 0.1f, _parent, InitPolicy.Retry,
                        builder =>
                        {
                            builder.Register<WmtsService>(Lifetime.Singleton);
                        }, resolver =>
                        {
                            Task.Run(resolver.Resolve<WmtsService>().Init);
                            return default;
                        })
                }),
                new("FeaturesInit", StepMode.Sequential, new IInitUnit[]
                {
                    new ActionUnit("Maps Feature", 0.25f, InitPolicy.Fail, () =>
                    {
                        return UniTask.Delay(1000);
                    })
                })
            };
        }

        public async Task<UniTask> RunAsync(CancellationToken cancellationToken)
        {
            float total = _initSteps.SelectMany(s => s.InitUnits).Sum(u => u.Weight);
            float accumulated = 0f;

            foreach (InitStep step in _initSteps)
            {
                Debug.Log($"Enter Init Step {step.Name}");

                switch (step.Mode)
                {
                    case StepMode.Sequential:
                        foreach (IInitUnit initUnit in step.InitUnits)
                        {
                            await initUnit.RunAsync();
                            accumulated += initUnit.Weight / total;
                            _preloader.ReportProgress($"{step.Name} .. {initUnit.Name}", accumulated);
                        }

                        break;
                    case StepMode.Parallel:
                        float[] weights = step.InitUnits.Select(s => s.Weight / total).ToArray();
                        await UniTask.WhenAll(step.InitUnits.Select(u => u.RunAsync()));

                        float block = weights.Sum();
                        accumulated += block;
                        _preloader.ReportProgress(step.Name, accumulated);
                        break;
                    default:
                        Debug.LogError($"Unknown step mode {step.Mode}");
                        break;
                }

                Debug.Log($"Exit Init Step {step.Name}");
            }

            _preloader.ReportProgress("Complete", 1f);
            return UniTask.CompletedTask;
        }
    }
}
