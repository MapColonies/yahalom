using System.Threading;
using com.mapcolonies.yahalom.Preloader;
using Cysharp.Threading.Tasks;

namespace com.mapcolonies.yahalom.InitPipeline
{
    public class InitializationPipeline
    {
        private readonly PreloaderViewModel _preloader;

        public InitializationPipeline(PreloaderViewModel preloader)
        {
            _preloader = preloader;
        }

        public UniTask RunAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}