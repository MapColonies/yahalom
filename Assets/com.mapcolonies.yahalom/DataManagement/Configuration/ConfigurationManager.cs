using com.mapcolonies.core.Utilities;
using com.mapcolonies.yahalom.DataManagement.AppSettings;
using com.mapcolonies.yahalom.DataManagement.UserSettings;
using com.mapcolonies.yahalom.ReduxStore;
using Cysharp.Threading.Tasks;
using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.DataManagement.Configuration
{
    public class ConfigurationManager : BaseDataManager
    {
        public ConfigurationManager(IReduxStoreManager reduxStoreManager) : base(reduxStoreManager){}

        public async UniTask Load()
        {
            ConfigurationState configurationState;

            bool offline = ReduxStoreManager.Store.GetState(
                UserSettingsReducer.SliceName,
                UserSettingsSelectors.OfflineSelector);

            string offlineConfigurationPath = ReduxStoreManager.Store.GetState(
                AppSettingsReducer.SliceName,
                AppSettingsSelectors.OfflineConfigurationPathSelector);

            string remoteConfigurationUrl = ReduxStoreManager.Store.GetState(
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

            ReduxStoreManager.Store.Dispatch(ConfigurationActions.LoadConfigurationAction(configurationState));
        }
    }
}
