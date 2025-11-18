using System;
using com.mapcolonies.core.Utilities;
using com.mapcolonies.yahalom.DataManagement.AppSettings;
using com.mapcolonies.yahalom.ReduxStore;
using Cysharp.Threading.Tasks;
using R3;
using Unity.AppUI.Redux;
using UnityEngine;

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
                    Debug.Log("saving user settings");
                    await JsonUtilityEx.SavePersistentJsonAsync(_userSettingsPath, state);
                })
                .AddTo(Disposables);
        }

        public async UniTask Load()
        {
            UserSettingsState userSettingsState;
            bool exists = await JsonUtilityEx.DoesPersistentJsonExistAsync(_userSettingsPath);
            if (exists)
            {
                userSettingsState = await JsonUtilityEx.LoadPersistentJsonAsync<UserSettingsState>(_userSettingsPath);
            }
            else
            {
                userSettingsState = ReduxStoreManager.Store.GetState<UserSettingsState>(UserSettingsReducer.SliceName);
                Debug.Log("Saving new user settings state...");
                await JsonUtilityEx.SavePersistentJsonAsync(_userSettingsPath, userSettingsState);
            }

            ReduxStoreManager.Store.Dispatch(UserSettingsActions.LoadUserSettingsAction(userSettingsState));
        }
    }
}
