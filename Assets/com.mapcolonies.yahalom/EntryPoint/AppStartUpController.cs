using System;
using System.Collections.Generic;
using System.Threading;
using com.mapcolonies.core.Services.Analytics.Managers;
using com.mapcolonies.yahalom.DataManagement.AppSettings;
using com.mapcolonies.yahalom.DataManagement.Configuration;
using com.mapcolonies.yahalom.DataManagement.UserSettings;
using com.mapcolonies.yahalom.DataManagement.Workspaces;
using com.mapcolonies.yahalom.InitPipeline;
using com.mapcolonies.yahalom.InitPipeline.InitSteps;
using com.mapcolonies.yahalom.InitPipeline.InitUnits;
using com.mapcolonies.yahalom.ReduxStore;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using com.mapcolonies.core.Localization;
using com.mapcolonies.core.Services.LoggerService;
using com.mapcolonies.core.Localization.Models;
using com.mapcolonies.yahalom.AppMode;
using com.mapcolonies.yahalom.AppMode.Modes;
using Unity.AppUI.Redux;
using SimulationMode = com.mapcolonies.yahalom.AppMode.Modes.SimulationMode;

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
                    new ActionUnit("Workspaces", 0.05f, InitPolicy.Fail,
                        () =>
                        {
                            WorkspacesManager workspacesManager = scope.Container.Resolve<WorkspacesManager>();
                            return workspacesManager.Load();
                        }),
                    new ActionUnit("Analytics Manager", 0.05f, InitPolicy.Fail,
                        () =>
                        {
                            AnalyticsManager analyticsManager = scope.Container.Resolve<AnalyticsManager>();
                            analyticsManager.Initialize();
                            return default;
                        }),
                    new ActionUnit("Translation Service", 0.1f, InitPolicy.Fail,
                        () =>
                        {
                            ReduxStoreManager reduxStoreManager = scope.Container.Resolve<ReduxStoreManager>();
                            TranslationSettings translationSettings = reduxStoreManager.Store.GetState(AppSettingsReducer.SliceName, AppSettingsSelectors.TranslationSettings);
                            ITranslationService translationService = scope.Container.Resolve<ITranslationService>();
                            return translationService.InitializeService(translationSettings);
                        }),
                    UsageAnalyticsServices(scope),
                    new ActionUnit("Configuration", 0.1f, InitPolicy.Fail,
                        () =>
                        {
                            ConfigurationManager config = scope.Container.Resolve<ConfigurationManager>();
                            return config.Load();
                        }),
                    new RegisterScopeUnit("App Mode Switcher Init", 0.01f, _scope, InitPolicy.Fail, builder =>
                        {
                            builder.Register<PlanningMode>(Lifetime.Scoped);
                            builder.Register<SimulationMode>(Lifetime.Scoped);
                        },
                        resolver =>
                        {
                            AppModeSwitcher appModeSwitcher = _scope.Container.Resolve<AppModeSwitcher>();
                            appModeSwitcher.RegisterChildScope(resolver);
                            return default;
                        }),
                    new ActionUnit("Logger Service", 0.1f, InitPolicy.Fail,
                        () =>
                        {
                            ReduxStoreManager reduxStoreManager = scope.Container.Resolve<ReduxStoreManager>();
                            LoggerSettings loggerSettings = reduxStoreManager.Store.GetState(AppSettingsReducer.SliceName, AppSettingsSelectors.LoggerSettings);
                            LoggerService loggerService = scope.Container.Resolve<LoggerService>();
                            return loggerService.UpdateLoggerSettings(loggerSettings);
                        }),
                })
            };
        }

        async UniTask IAsyncStartable.StartAsync(CancellationToken cancellation = new CancellationToken())
        {
            Debug.Log("Start initializing");
            await _pipeline.RunAsync(_initSteps, 0f, 0.8f, false);
            Debug.Log("Initialized");

            AppModeSwitcher appModeSwitcher = _scope.Container.Resolve<AppModeSwitcher>();
            await appModeSwitcher.SetInitialMode<PlanningMode>();
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
