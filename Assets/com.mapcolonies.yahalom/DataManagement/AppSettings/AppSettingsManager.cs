using com.mapcolonies.core.Utilities;
using com.mapcolonies.yahalom.ReduxStore;
using Cysharp.Threading.Tasks;

namespace com.mapcolonies.yahalom.DataManagement.AppSettings
{
    public class AppSettingsManager : BaseDataManager
    {
        public AppSettingsManager(IReduxStoreManager reduxStoreManager) : base(reduxStoreManager) { }

        public async UniTask Load()
        {
            AppSettingsState appState = await JsonUtilityEx.LoadStreamingAssetsJsonAsync<AppSettingsState>("settings.json");
            ReduxStoreManager.Store.Dispatch(AppSettingsActions.LoadAppSettingsAction(appState));
        }
    }
}
