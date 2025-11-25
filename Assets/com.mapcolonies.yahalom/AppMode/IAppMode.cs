using Cysharp.Threading.Tasks;

namespace com.mapcolonies.yahalom.AppMode
{
    public interface IAppMode
    {
        UniTask EnterMode();
        UniTask ExitMode();
    }
}
