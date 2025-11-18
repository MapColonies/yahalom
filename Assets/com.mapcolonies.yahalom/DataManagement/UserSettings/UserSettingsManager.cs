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

        public UserSettingsManager(IReduxStoreManager reduxStoreManager) : base(reduxStoreManager)
        {
            _userSettingsPath = ReduxStoreManager.Store.GetState(AppSettingsReducer.SliceName, AppSettingsSelectors.UserSettingsPath);

            ReduxStoreManager.Store.Select<UserSettingsState>(UserSettingsReducer.SliceName)
                .DistinctUntilChanged()
                .Debounce(TimeSpan.FromSeconds(30))
                .SubscribeAwait(async (state, token) =>
                {
                    //todo: Log("saving user settings");
                    await JsonUtilityEx.SaveJsonAsync(_userSettingsPath, state);
                })
                .AddTo(Disposables);
        }

        public async UniTask Load()
        {
            UserSettingsState userSettingsState;
            bool exists = await JsonUtilityEx.DoesJsonExistAsync(_userSettingsPath);
            if (exists)
            {
                userSettingsState = await JsonUtilityEx.LoadJsonAsync<UserSettingsState>(_userSettingsPath, FileLocation.PersistentData);
            }
            else
            {
                userSettingsState = ReduxStoreManager.Store.GetState<UserSettingsState>(UserSettingsReducer.SliceName);
                //todo: Log("Saving new user settings state...");
                await JsonUtilityEx.SaveJsonAsync(_userSettingsPath, userSettingsState);
            }

            ReduxStoreManager.Store.Dispatch(UserSettingsActions.LoadUserSettingsAction(userSettingsState));
        }
    }
}
