using System;
using com.mapcolonies.core.Utilities;
using com.mapcolonies.yahalom.AppSettings;
using com.mapcolonies.yahalom.ReduxStore;
using Cysharp.Threading.Tasks;
using R3;
using Unity.AppUI.Redux;
using UnityEngine;

namespace com.mapcolonies.yahalom.UserSettings
{
    public class UserSettingsManager : IDisposable
    {
        private readonly IReduxStoreManager _reduxStoreManager;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly string _userSettingsPath;
        private bool _exists;

        public UserSettingsManager(IReduxStoreManager reduxStoreManager)
        {
            _reduxStoreManager = reduxStoreManager;
            _userSettingsPath = _reduxStoreManager.Store.GetState(AppSettingsReducer.SliceName, AppSettingsSelectors.UserSettingsPath);
            _reduxStoreManager.Store.SelectWhere<UserSettingsState>(
                    UserSettingsReducer.SliceName,
                    s => !_exists,
                    state =>
                    {
                        FileUtility.SavePersistentJsonAsync(_userSettingsPath, state).Forget();
                        Debug.Log("save file");
                    })
                .AddTo(_disposables);

            // _reduxStoreManager.Epics.RunEpic(TestEpic, _reduxStoreManager.Store).AddTo(_disposables);
        }

        /*private Observable<UserSettingsStateAction> TestEpic(Observable<UserSettingsStateAction> actions)
        {
            return actions.OfType<UserSettingsStateAction.>()
                .Do(_ => Debug.Log("player jumped"));
        }*/

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
            _disposables.Dispose();
        }
    }
}
