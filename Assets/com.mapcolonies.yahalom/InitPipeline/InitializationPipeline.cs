using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using com.mapcolonies.core.Services;
using com.mapcolonies.yahalom.Preloader;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

namespace com.mapcolonies.yahalom.InitPipeline
{
    public class InitializationPipeline
    {
        private readonly PreloaderViewModel _preloader;
        private List<InitStep> _initSteps;
        private readonly LifetimeScope _parent;

        public InitializationPipeline(PreloaderViewModel preloader, LifetimeScope scope)
        {
            _preloader = preloader;
            _parent = scope;
            
            _initSteps = new List<InitStep>
            {
                new InitStep("PreInit", StepMode.Sequential, new IInitUnit[]
                {
                    new ActionUnit("Logging Init",0.05f, () => { return UniTask.Delay(100); }),
                    new ActionUnit("Local Settings",0.05f, () => { return UniTask.Delay(100); })
                }),
                new InitStep("ServicesInit", StepMode.Sequential, new IInitUnit[]
                {
                    new RegisterScopeUnit("WMTS", 0.1f, _parent, builder =>
                    {
                        builder.Register<WmtsService>(Lifetime.Singleton);
                    }, resolver =>
                    {
                        Task.Run(resolver.Resolve<WmtsService>().Init);
                        return default;
                    }),
                }),
                new InitStep("FeaturesInit", StepMode.Sequential, new IInitUnit[]
                {
                    new ActionUnit("Maps Feature",0.25f, () => { return UniTask.Delay(100); })
                })
            };
        }

        
        public async Task<UniTask> RunAsync(CancellationToken cancellationToken)
        {
            float total = _initSteps.SelectMany(s => s.InitUnits).Sum(u => u.Weight);
            float accumulated = 0f;
            
            foreach (InitStep step in _initSteps)
            {
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
                        float[] weights = step.InitUnits.Select(s => s.Weight /total).ToArray();
                        await UniTask.WhenAll(step.InitUnits.Select(u => u.RunAsync()));

                        float block = weights.Sum();
                        accumulated += block;
                        _preloader.ReportProgress(step.Name, accumulated);
                        break;
                    default:
                        break;
                }
            }
            
            _preloader.ReportProgress("Complete", 1f);
            return UniTask.CompletedTask;
        }
    }
}