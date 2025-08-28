using Cysharp.Threading.Tasks;

namespace com.mapcolonies.yahalom.InitPipeline.InitUnits
{
    public interface IInitUnit
    {
        string Name { get; }
        float Weight { get; }
        InitPolicy  Policy { get; }
        UniTask RunAsync();
    }
}