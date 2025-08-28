using Cysharp.Threading.Tasks;

namespace com.mapcolonies.yahalom.InitPipeline
{
    public interface IInitUnit
    {
        string Name { get; }
        float Weight { get; }
        UniTask RunAsync();
    }
}