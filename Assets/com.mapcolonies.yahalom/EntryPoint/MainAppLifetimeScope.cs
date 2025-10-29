using com.mapcolonies.yahalom.AppSettings;
using com.mapcolonies.yahalom.Configuration;
using com.mapcolonies.yahalom.InitPipeline;
using com.mapcolonies.yahalom.Preloader;
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

            Debug.Log("End Configure Startup Registrations");

            #region Services
                builder.Register<ReduxStoreManager>(Lifetime.Singleton);
                builder.Register<AppSettingsManager>(Lifetime.Singleton);
                builder.Register<ConfigurationManager>(Lifetime.Singleton);
            #endregion

        }
    }
}
