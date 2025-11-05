using System;
using com.mapcolonies.core.Utilities;
using com.mapcolonies.yahalom.AppSettings;
using com.mapcolonies.yahalom.ReduxStore;
using Cysharp.Threading.Tasks;
using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.UserSettings
{
    public class UserSettingsManager : IDisposable
    {
        private readonly IReduxStoreManager _reduxStoreManager;
        private readonly AppSettingsState _settings;
        private bool _exists;
        private readonly IDisposableSubscription _unsubscribe;
        private readonly string _userSettingsPath;

        public UserSettingsManager(IReduxStoreManager reduxStoreManager)
        {
            _reduxStoreManager = reduxStoreManager;
            _userSettingsPath = _reduxStoreManager.Store.GetState(AppSettingsReducer.SliceName, AppSettingsSelectors.UserSettingsPath);
        }

        public async UniTask Load()
        {
            UserSettingsState userSettingsState;
            _exists = await FileUtility.DoesPersistentJsonExistAsync(_userSettingsPath);
            if (_exists)
            {
                userSettingsState = await FileUtility.LoadPersistentJsonAsync<UserSettingsState>(_userSettingsPath);
            }
            else
            {
                userSettingsState = new UserSettingsState();
            }

            _reduxStoreManager.Store.Dispatch(UserSettingsActions.LoadUserSettingsAction(userSettingsState));
        }

        public void Dispose()
        {
            _unsubscribe?.Dispose();
        }
    }
}
