using System;
using System.Collections.Generic;
using System.Threading;
using com.mapcolonies.yahalom.AppSettings;
using com.mapcolonies.yahalom.Configuration;
using com.mapcolonies.core.Services.Analytics.Managers;
using com.mapcolonies.yahalom.InitPipeline;
using com.mapcolonies.yahalom.InitPipeline.InitSteps;
using com.mapcolonies.yahalom.InitPipeline.InitUnits;
using com.mapcolonies.yahalom.SceneManagement;
using com.mapcolonies.yahalom.SceneManagement.Enums;
using com.mapcolonies.yahalom.ReduxStore;
using com.mapcolonies.yahalom.UserSettings;
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
                            IReduxStoreManager reduxStore = scope.Container.Resolve<ReduxStoreManager>();
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
                    new ActionUnit("User Settings", 0.1f, InitPolicy.Fail,
                        () =>
                        {
                            UserSettingsManager userSettingsSettings = scope.Container.Resolve<UserSettingsManager>();
                            return userSettingsSettings.Load();
                        }),
                    new ActionUnit("Analytics Manager", 0.05f, InitPolicy.Fail,
                        () =>
                        {
                            AnalyticsManager analyticsManager = scope.Container.Resolve<AnalyticsManager>();
                            analyticsManager.Initialize();
                            return default;
                        }),
                    UsageAnalyticsServices(scope),
                    new ActionUnit("Configuration", 0.1f, InitPolicy.Fail,
                        () =>
                        {
                            ConfigurationManager config = scope.Container.Resolve<ConfigurationManager>();
                            return config.Load();
                        })
                }),
                new InitStep("SwitchScene", StepMode.Sequential, new IInitUnit[]
                {
                    new ActionUnit("Load Target Scene", 0.10f, InitPolicy.Fail,
                        () =>
                        {
                            var sceneController = scope.Container.Resolve<ISceneController>();
                            return sceneController.SwitchSceneAsync(Scenes.PlanningScene);
                        })
                })
            };
        }

        async UniTask IAsyncStartable.StartAsync(CancellationToken cancellation = new CancellationToken())
        {
            Debug.Log("Start initializing");
            await _pipeline.RunAsync(_initSteps);
            Debug.Log("Initialized");
        }

        private IInitUnit UsageAnalyticsServices(LifetimeScope scope)
        {
            return new RegisterScopeUnit(
                "Usage Analytics Manager",
                0.20f,
                scope,
                InitPolicy.Fail,
                builder =>
                {
                    builder.Register<UsageAnalyticsManager>(Lifetime.Singleton)
                        .AsSelf()
                        .As<IDisposable>();
                },
                resolver =>
                {
                    resolver.Resolve<UsageAnalyticsManager>().Initialize();
                    return default;
                }
            );
        }
    }
}
