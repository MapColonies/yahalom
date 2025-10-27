using System.Collections.Generic;
using System.Threading;
using com.mapcolonies.yahalom.Configuration;
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
        private readonly LifetimeScope _parentLifetimeScope;
        private readonly InitializationPipeline _pipeline;
        private readonly List<InitStep> _initSteps;

        public AppStartUpController(InitializationPipeline initializationPipeline, LifetimeScope scope)
        {
            _pipeline = initializationPipeline;
            _initSteps = new List<InitStep>
            {
                new InitStep("ServicesInit", StepMode.Sequential, new IInitUnit[]
                {
                    new ActionUnit("Configuration Service", 1f, InitPolicy.Fail,
                        () =>
                        {
                            ConfigurationManager config = scope.Container.Resolve<ConfigurationManager>();
                            return config.Load();
                        })
                })
            };
        }

        async UniTask IAsyncStartable.StartAsync(CancellationToken cancellation = new())
        {
            Debug.Log("Start initializing");
            await _pipeline.RunAsync(_initSteps);
            Debug.Log("Initialized");
        }
    }
}
