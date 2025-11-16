using com.mapcolonies.core.Utilities;
using com.mapcolonies.yahalom.ReduxStore;
using Cysharp.Threading.Tasks;

namespace com.mapcolonies.yahalom.Workspaces
{
    public class WorkspacesManager
    {
        private readonly IReduxStoreManager _reduxStoreManager;

        public WorkspacesManager(IReduxStoreManager reduxStoreManager)
        {
            _reduxStoreManager = reduxStoreManager;
        }

        public async UniTask Load()
        {
            WorkspacesState workspacesState = await JsonUtilityEx.LoadPersistentJsonAsync<WorkspacesState>("workspaces.json");
            _reduxStoreManager.Store.Dispatch(WorkspacesActions.LoadWorkspacesAction(workspacesState));
        }
    }
}
