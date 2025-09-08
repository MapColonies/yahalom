using System.Threading;
using com.mapcolonies.yahalom.InitPipeline;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace com.mapcolonies.yahalom.EntryPoint
{
    public class AppStartUpController : IAsyncStartable
    {
        private readonly LifetimeScope _parentLifetimeScope;
        private readonly InitializationPipeline _pipeline;

        public AppStartUpController(InitializationPipeline initializationPipeline)
        {
            _pipeline = initializationPipeline;
        }

        async UniTask IAsyncStartable.StartAsync(CancellationToken cancellation = new())
        {
            Debug.Log("Start initializing");
            await _pipeline.RunAsync(cancellation);
            Debug.Log("Initialized");
        }
    }
}
