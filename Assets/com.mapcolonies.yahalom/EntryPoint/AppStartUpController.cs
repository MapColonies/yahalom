using System.Threading;
using com.mapcolonies.yahalom.InitPipeline;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace com.mapcolonies.yahalom.EntryPoint
{
    public class AppStartUpController : IAsyncStartable
    {
        private readonly InitializationPipeline _pipeline;

        public AppStartUpController(InitializationPipeline initializationPipeline)
        {
            _pipeline = initializationPipeline;
        }

        UniTask IAsyncStartable.StartAsync(CancellationToken cancellation = new CancellationToken())
        {
            return _pipeline.RunAsync(cancellation);
        }
    }
}