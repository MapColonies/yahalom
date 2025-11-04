using System;
using com.mapcolonies.core.Services.Analytics.Managers;
using com.mapcolonies.yahalom.InitPipeline;
using com.mapcolonies.yahalom.Preloader;
using com.mapcolonies.yahalom.SceneController;
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

            builder.Register<PreloaderViewModel>(Lifetime.Scoped);
            builder.Register<InitializationPipeline>(Lifetime.Scoped);
            builder.Register<ISceneController, SceneController.SceneController>(Lifetime.Singleton);
            builder.Register<AppStartUpController>(Lifetime.Singleton).As<IAsyncStartable>();
            builder.Register<AnalyticsManager>(Lifetime.Singleton).AsSelf().As<IAnalyticsManager>().As<IDisposable>();

            //This is a Demo script, will be deleted in the near future.
            builder.RegisterComponentInHierarchy<DemoController>();

            Debug.Log("End Configure Startup Registrations");
        }
    }
}
