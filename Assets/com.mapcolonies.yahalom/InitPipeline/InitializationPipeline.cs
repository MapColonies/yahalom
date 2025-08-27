using System.Threading;
using System.Threading.Tasks;
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

        public async Task<UniTask> RunAsync(CancellationToken cancellationToken)
        {
            await UniTask.Delay(1000);
            _preloader.Progress = 0.3f;

            return UniTask.CompletedTask;
        }
    }
}