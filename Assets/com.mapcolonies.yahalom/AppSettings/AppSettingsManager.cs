using com.mapcolonies.core.Utilities;
using com.mapcolonies.yahalom.ReduxStore;
using Cysharp.Threading.Tasks;
using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.AppSettings
{
    public class AppSettingsManager
    {
        private readonly ReduxStoreManager _reduxStoreManager;

        public AppSettingsManager(ReduxStoreManager reduxStoreManager)
        {
            _reduxStoreManager = reduxStoreManager;
        }

        public async UniTask Load()
        {
            AppSettingsState appState = await JsonLoader.LoadStreamingAssetsJsonAsync<AppSettingsState>("settings.json");
            _reduxStoreManager.Store.Dispatch(new ActionCreator<AppSettingsState>(ReduxStoreManager.SetAppSettingsAction).Invoke(appState));
        }
    }
}
