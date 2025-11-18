using com.mapcolonies.core.Utilities;
using com.mapcolonies.yahalom.ReduxStore;
using Cysharp.Threading.Tasks;

namespace com.mapcolonies.yahalom.DataManagement.AppSettings
{
    public class AppSettingsManager : BaseDataManager
    {
        private const string SettingsFileName = "app_settings.json";
        public AppSettingsManager(IReduxStoreManager reduxStoreManager) : base(reduxStoreManager)
        {
        }

        public async UniTask Load()
        {
            AppSettingsState appState = await JsonUtilityEx.LoadStreamingAssetsJsonAsync<AppSettingsState>(SettingsFileName);
            ReduxStoreManager.Store.Dispatch(AppSettingsActions.LoadAppSettingsAction(appState));
        }
    }
}
