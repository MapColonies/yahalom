using com.mapcolonies.core.Utilities;
using com.mapcolonies.yahalom.AppSettings;
using com.mapcolonies.yahalom.ReduxStore;
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
            AppSettingsState settings = _reduxStoreManager.Store.GetState<AppSettingsState>(AppSettingsReducer.SliceName);

            if (settings.OfflineMode)
                configurationState = await JsonLoader.LoadStreamingAssetsJsonAsync<ConfigurationState>(settings.ConfigurationPath);
            else
                configurationState = await JsonLoader.LoadRemoteJsonAsync<ConfigurationState>("settings.RemoteConfigurationUrl");

            _reduxStoreManager.Store.Dispatch(ConfigurationActions.LoadConfigurationAction(configurationState));
        }
    }
}
