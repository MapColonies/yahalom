using System;
using com.mapcolonies.core.Services.Analytics.Managers;
using com.mapcolonies.yahalom.DataManagement.AppSettings;
using com.mapcolonies.yahalom.DataManagement.Configuration;
using com.mapcolonies.yahalom.DataManagement.UserSettings;
using com.mapcolonies.yahalom.DataManagement.Workspaces;
using com.mapcolonies.yahalom.InitPipeline;
using com.mapcolonies.yahalom.Preloader;
using com.mapcolonies.yahalom.SceneManagement;
using com.mapcolonies.yahalom.ReduxStore;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using com.mapcolonies.core.Localization;
using com.mapcolonies.core.Services.LoggerService;
using com.mapcolonies.yahalom.AppMode;

namespace com.mapcolonies.yahalom.EntryPoint
{
    public class MainAppLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log("Begin Configure Startup Registrations");

            #region StartUp

            builder.Register<PreloaderViewModel>(Lifetime.Singleton);
            builder.Register<InitializationPipeline>(Lifetime.Transient);
            builder.Register<AppStartUpController>(Lifetime.Singleton).As<IAsyncStartable>();

            #endregion

            //TODO: This is a Demo script, will be deleted in the near future.
            builder.RegisterComponentInHierarchy<DemoController>();

            Debug.Log("End Configure Startup Registrations");

            #region Services

            builder.Register<ActionsMiddleware>(Lifetime.Singleton);
            builder.Register<ReduxStoreManager>(Lifetime.Singleton).AsSelf().As<IReduxStoreManager>();
            builder.Register<AppSettingsManager>(Lifetime.Singleton);
            builder.Register<UserSettingsManager>(Lifetime.Singleton);
            builder.Register<ConfigurationManager>(Lifetime.Singleton);
            builder.Register<WorkspacesManager>(Lifetime.Singleton);
            builder.Register<ISceneController, SceneController>(Lifetime.Singleton);
            builder.Register<TranslationService>(Lifetime.Singleton).As<ITranslationService>().As<IDisposable>();
            builder.Register<AnalyticsManager>(Lifetime.Singleton).AsSelf().As<IAnalyticsManager>().As<IDisposable>();
            builder.Register<AppModeSwitcher>(Lifetime.Singleton);
            builder.RegisterInstance(LoggerInitializer.Logger).As<LoggerService>().As<IDisposable>();

            #endregion
        }
    }
}
