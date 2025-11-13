using Cysharp.Threading.Tasks;
using Unity.AppUI.Redux;
using VContainer;

namespace com.mapcolonies.yahalom.ReduxStore
{
    public interface IReduxStoreManager
    {
        UniTask Create(IObjectResolver resolver);

        public EpicMiddlewareCreator Epics
        {
            get;
        }

        IStore<PartitionedState> Store
        {
            get;
        }
    }
}
