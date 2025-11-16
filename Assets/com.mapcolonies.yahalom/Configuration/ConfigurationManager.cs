using com.mapcolonies.core.Utilities;
using com.mapcolonies.yahalom.AppSettings;
using com.mapcolonies.yahalom.ReduxStore;
using com.mapcolonies.yahalom.UserSettings;
using Cysharp.Threading.Tasks;
using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.Configuration
{
    public class ConfigurationManager
    {
        private readonly IReduxStoreManager _reduxStoreManager;

        public ConfigurationManager(IReduxStoreManager reduxStoreManager)
        {
            _reduxStoreManager = reduxStoreManager;
        }

        public async UniTask Load()
        {
            ConfigurationState configurationState;

            bool offline = _reduxStoreManager.Store.GetState(
                UserSettingsReducer.SliceName,
                UserSettingsSelectors.OfflineSelector);

            string offlineConfigurationPath = _reduxStoreManager.Store.GetState(
                AppSettingsReducer.SliceName,
                AppSettingsSelectors.OfflineConfigurationPathSelector);

            string remoteConfigurationUrl = _reduxStoreManager.Store.GetState(
                AppSettingsReducer.SliceName,
                AppSettingsSelectors.RemoteConfigurationUrlSelector);

            if (offline)
            {
                configurationState = await JsonUtilityEx.LoadStreamingAssetsJsonAsync<ConfigurationState>(offlineConfigurationPath);
            }
            else
            {
                configurationState = await JsonUtilityEx.LoadRemoteJsonAsync<ConfigurationState>(remoteConfigurationUrl);
            }

            _reduxStoreManager.Store.Dispatch(ConfigurationActions.LoadConfigurationAction(configurationState));
        }
    }
}
