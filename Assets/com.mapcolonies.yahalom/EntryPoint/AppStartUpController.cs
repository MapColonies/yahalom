using System.Collections.Generic;
using System.Threading;
using com.mapcolonies.yahalom.AppSettings;
using com.mapcolonies.yahalom.Configuration;
using com.mapcolonies.yahalom.InitPipeline;
using com.mapcolonies.yahalom.InitPipeline.InitSteps;
using com.mapcolonies.yahalom.InitPipeline.InitUnits;
using com.mapcolonies.yahalom.ReduxStore;
using Cysharp.Threading.Tasks;
using Unity.AppUI.Redux;
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
        private readonly LifetimeScope _scope;

        public AppStartUpController(InitializationPipeline initializationPipeline, LifetimeScope scope)
        {
            _scope = scope;
            _pipeline = initializationPipeline;
            _initSteps = new List<InitStep>
            {
                new InitStep("PreInit", StepMode.Sequential, new IInitUnit[]
                {
                    new ActionUnit("Redux Store", 0.1f, InitPolicy.Fail,
                        () =>
                        {
                            ReduxStoreManager reduxStore = scope.Container.Resolve<ReduxStoreManager>();
                            return reduxStore.Create();
                        })
                }),
                new InitStep("ServicesInit", StepMode.Sequential, new IInitUnit[]
                {
                    new ActionUnit("App Settings", 0.1f, InitPolicy.Fail,
                        () =>
                        {
                            AppSettingsManager appSettings = scope.Container.Resolve<AppSettingsManager>();
                            return appSettings.Load();
                        }),
                    new ActionUnit("Configuration", 0.1f, InitPolicy.Fail,
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

            ReduxStoreManager reduxStore = _scope.Container.Resolve<ReduxStoreManager>();
            var config = reduxStore.Store.GetState<ConfigurationState>(ConfigurationReducer.SliceName);

            Debug.Log("Initialized");
        }
    }
}
