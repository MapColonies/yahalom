using com.mapcolonies.core.Services.ConfigurationService;
using com.mapcolonies.yahalom.InitPipeline;
using com.mapcolonies.yahalom.Preloader;
using com.mapcolonies.yahalom.Redux;
using Unity.AppUI.Redux;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace com.mapcolonies.yahalom.EntryPoint
{
    public class MainAppLifetimeScope : LifetimeScope
    {
        private AppState _state;

        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log("Begin Configure Startup Registrations");

            builder.Register<PreloaderViewModel>(Lifetime.Scoped);
            builder.Register<InitializationPipeline>(Lifetime.Scoped);

            builder.Register<AppStartUpController>(Lifetime.Singleton).As<IAsyncStartable>();

            Debug.Log("End Configure Startup Registrations");

            _state = new AppState();
            builder.RegisterInstance(new Store<AppState>(RootReducer.Reduce, _state));

            #region Register Srvices
            builder.Register<ConfigurationService>(Lifetime.Singleton);


            #endregion

        }
    }
}
