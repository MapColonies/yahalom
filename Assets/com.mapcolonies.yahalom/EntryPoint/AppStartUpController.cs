using System.Threading;
using com.mapcolonies.yahalom.InitPipeline;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace com.mapcolonies.yahalom.EntryPoint
{
    public class AppStartUpController : IAsyncStartable
    {
        private readonly InitializationPipeline _pipeline;
        private readonly LifetimeScope _parentLifetimeScope;

        public AppStartUpController(InitializationPipeline initializationPipeline)
        {
            _pipeline = initializationPipeline;
        }

        async UniTask IAsyncStartable.StartAsync(CancellationToken cancellation = new CancellationToken())
        {
            await _pipeline.RunAsync(cancellation);
            
        }
    }
}