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

            bool offline = _reduxStoreManager.GetState(
                UserSettingsReducer.SliceName,
                UserSettingsSelectors.OfflineSelector);

            string offlineConfigurationPath = _reduxStoreManager.GetState(
                AppSettingsReducer.SliceName,
                AppSettingsSelectors.OfflineConfigurationPathSelector);

            string remoteConfigurationUrl = _reduxStoreManager.GetState(
                AppSettingsReducer.SliceName,
                AppSettingsSelectors.RemoteConfigurationUrlSelector);

            if (offline)
            {
                configurationState = await FileUtility.LoadStreamingAssetsJsonAsync<ConfigurationState>(offlineConfigurationPath);
            }
            else
            {
                configurationState = await FileUtility.LoadRemoteJsonAsync<ConfigurationState>(remoteConfigurationUrl);
            }

            _reduxStoreManager.Dispatch(ConfigurationActions.LoadConfigurationAction(configurationState));
        }
    }
}
