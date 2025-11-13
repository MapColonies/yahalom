using Cysharp.Threading.Tasks;
using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.ReduxStore
{
    public interface IReduxStoreManager
    {
        UniTask Create();

        IStore<PartitionedState> Store
        {
            get;
        }
    }
}
