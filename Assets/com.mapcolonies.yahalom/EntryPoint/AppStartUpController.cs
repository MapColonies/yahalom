using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using com.mapcolonies.core.Services;
using com.mapcolonies.yahalom.InitPipeline;
using com.mapcolonies.yahalom.InitPipeline.InitSteps;
using com.mapcolonies.yahalom.InitPipeline.InitUnits;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace com.mapcolonies.yahalom.EntryPoint
{
    public class AppStartUpController : IAsyncStartable
    {
        private readonly InitializationPipeline _pipeline;
        private readonly LifetimeScope _parentLifetimeScope;
        private readonly List<InitStep> _initSteps;

        public AppStartUpController(InitializationPipeline initializationPipeline, LifetimeScope scope)
        {
            _pipeline = initializationPipeline;
            _initSteps = new List<InitStep>
            {
                new InitStep("PreInit", StepMode.Sequential, new IInitUnit[]
                {
                    new ActionUnit("Logging Init", 0.05f, InitPolicy.Fail,
                        () => { return Cysharp.Threading.Tasks.UniTask.Delay(1000); }),
                    new ActionUnit("Local Settings", 0.05f, InitPolicy.Fail,
                        () => { return Cysharp.Threading.Tasks.UniTask.Delay(1000); })
                }),
                new InitStep("ServicesInit", StepMode.Sequential, new IInitUnit[]
                {
                    new RegisterScopeUnit("WMTS", 0.1f, scope, InitPolicy.Retry,
                        builder => { builder.Register<WmtsService>(Lifetime.Singleton); }, resolver =>
                        {
                            Task.Run(resolver.Resolve<WmtsService>().Init);
                            return default;
                        }),
                }),
                new InitStep("FeaturesInit", StepMode.Sequential, new IInitUnit[]
                {
                    new ActionUnit("Maps Feature", 0.25f, InitPolicy.Fail,
                        () => { return Cysharp.Threading.Tasks.UniTask.Delay(1000); })
                })
            };
        }

        async UniTask IAsyncStartable.StartAsync(CancellationToken cancellation = new CancellationToken())
        {
            Debug.Log("Start initializing");
            await _pipeline.RunAsync(_initSteps);
            Debug.Log("Initialized");
        }
    }
}