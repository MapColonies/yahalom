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
        private readonly AppSettingsState _settings;
        private bool _exists;

        public UserSettingsManager(IReduxStoreManager reduxStoreManager)
        {
            _reduxStoreManager = reduxStoreManager;
            _settings = _reduxStoreManager.Store.GetState<AppSettingsState>(AppSettingsReducer.SliceName);
            _reduxStoreManager.Store.Subscribe(s => s.Get<UserSettingsState>(UserSettingsReducer.SliceName), state =>
            {
                if(!_exists)
                {
                    FileUtility.SavePersistentJsonAsync(_settings.UserSettingsPath, state).Forget();
                }
            });
        }

        public async UniTask Load()
        {
            UserSettingsState userSettingsState;
           _exists = await FileUtility.DoesPersistentJsonExistAsync(_settings.UserSettingsPath);
            if (_exists)
            {
                userSettingsState = await FileUtility.LoadPersistentJsonAsync<UserSettingsState>(_settings.UserSettingsPath);
            }
            else
            {
                userSettingsState = new UserSettingsState();

            }
            _reduxStoreManager.Store.Dispatch(UserSettingsActions.LoadUserSettingsAction(userSettingsState));
        }
    }
}
