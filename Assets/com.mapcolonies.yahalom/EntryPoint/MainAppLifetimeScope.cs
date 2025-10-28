using com.mapcolonies.yahalom.Configuration;
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
        private AppState _initialState;

        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log("Begin Configure Startup Registrations");

            #region StartUp
                builder.Register<PreloaderViewModel>(Lifetime.Scoped);
                builder.Register<InitializationPipeline>(Lifetime.Scoped);
                builder.Register<AppStartUpController>(Lifetime.Singleton).As<IAsyncStartable>();
            #endregion

            Debug.Log("End Configure Startup Registrations");

            #region Register store
                _initialState = new AppState(new ConfigurationState());
                IStore<AppState> store = new Store<AppState>(RootReducer.Reduce, _initialState);
                builder.RegisterInstance(store);
            #endregion

            #region Register Services
                builder.Register<ReduxStoreManager>(Lifetime.Singleton);
                builder.Register<ConfigurationManager>(Lifetime.Singleton);
            #endregion

        }
    }
}
