using System;
using com.mapcolonies.core.Utilities;
using com.mapcolonies.yahalom.DataManagement.AppSettings;
using com.mapcolonies.yahalom.ReduxStore;
using Cysharp.Threading.Tasks;
using R3;
using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.DataManagement.UserSettings
{
    public class UserSettingsManager : BaseDataManager
    {
        private readonly string _userSettingsPath;
        private bool _exists;

        public UserSettingsManager(IReduxStoreManager reduxStoreManager) : base(reduxStoreManager)
        {
            _userSettingsPath = ReduxStoreManager.Store.GetState(AppSettingsReducer.SliceName, AppSettingsSelectors.UserSettingsPath);

            ReduxStoreManager.Store.Select<UserSettingsState>(UserSettingsReducer.SliceName)
                .DistinctUntilChanged()
                .Debounce(TimeSpan.FromSeconds(30))
                .Where(_ => !_exists)
                .SubscribeAwait(async (state, token) =>
                {
                    await JsonUtilityEx.SavePersistentJsonAsync(_userSettingsPath, state);
                })
                .AddTo(Disposables);
        }

        public async UniTask Load()
        {
            UserSettingsState userSettingsState;
            _exists = await JsonUtilityEx.DoesPersistentJsonExistAsync(_userSettingsPath);
            if (_exists)
            {
                userSettingsState = await JsonUtilityEx.LoadPersistentJsonAsync<UserSettingsState>(_userSettingsPath);
            }
            else
            {
                userSettingsState = new UserSettingsState();
            }

            ReduxStoreManager.Store.Dispatch(UserSettingsActions.LoadUserSettingsAction(userSettingsState));
        }
    }
}
