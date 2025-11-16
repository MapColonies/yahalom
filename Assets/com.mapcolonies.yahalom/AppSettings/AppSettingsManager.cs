using com.mapcolonies.core.Utilities;
using com.mapcolonies.yahalom.ReduxStore;
using Cysharp.Threading.Tasks;

namespace com.mapcolonies.yahalom.AppSettings
{
    public class AppSettingsManager
    {
        private readonly IReduxStoreManager _reduxStoreManager;

        public AppSettingsManager(IReduxStoreManager reduxStoreManager)
        {
            _reduxStoreManager = reduxStoreManager;
        }

        public async UniTask Load()
        {
            AppSettingsState appState = await JsonUtilityEx.LoadStreamingAssetsJsonAsync<AppSettingsState>("settings.json");
            _reduxStoreManager.Store.Dispatch(AppSettingsActions.LoadAppSettingsAction(appState));
        }
    }
}
