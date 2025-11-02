using com.mapcolonies.core.Utilities;
using com.mapcolonies.yahalom.AppSettings;
using com.mapcolonies.yahalom.ReduxStore;
using Cysharp.Threading.Tasks;
using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.UserSettings
{
    public class UserSettingsManager
    {
        private readonly IReduxStoreManager _reduxStoreManager;

        public UserSettingsManager(IReduxStoreManager reduxStoreManager)
        {
            _reduxStoreManager = reduxStoreManager;

        }

        public async UniTask Load()
        {
            AppSettingsState settings = _reduxStoreManager.Store.GetState<AppSettingsState>(AppSettingsReducer.SliceName);
            UserSettingsState userSettingsState = await JsonLoader.LoadPersistentJsonAsync<UserSettingsState>(settings.UserSettingsPath);
            _reduxStoreManager.Store.Dispatch(UserSettingsActions.LoadUserSettingsAction(userSettingsState));
        }
    }
}
