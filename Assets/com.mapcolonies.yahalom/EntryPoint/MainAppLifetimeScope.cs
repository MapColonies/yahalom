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

namespace com.mapcolonies.yahalom.EntryPoint
{
    public class MainAppLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log("Begin Configure Startup Registrations");

            #region StartUp

            builder.Register<PreloaderViewModel>(Lifetime.Scoped);
            builder.Register<InitializationPipeline>(Lifetime.Scoped);
            builder.Register<AppStartUpController>(Lifetime.Singleton).As<IAsyncStartable>();
            #endregion

            //TODO: This is a Demo script, will be deleted in the near future.
            builder.RegisterComponentInHierarchy<DemoController>();

            Debug.Log("End Configure Startup Registrations");
            #region Services

            builder.Register<ReduxStoreManager>(Lifetime.Singleton).AsSelf().As<IReduxStoreManager>();
            builder.Register<AppSettingsManager>(Lifetime.Singleton);
            builder.Register<UserSettingsManager>(Lifetime.Singleton);
            builder.Register<ConfigurationManager>(Lifetime.Singleton);
            builder.Register<WorkspacesManager>(Lifetime.Singleton);
            builder.Register<ISceneController, SceneController>(Lifetime.Singleton);
            builder.Register<AnalyticsManager>(Lifetime.Singleton).AsSelf().As<IAnalyticsManager>().As<IDisposable>();

            #endregion
        }
    }
}
