using com.mapcolonies.core.Utilities;
using com.mapcolonies.yahalom.AppSettings;
using com.mapcolonies.yahalom.ReduxStore;
using Cysharp.Threading.Tasks;
using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.Configuration
{
    public class ConfigurationManager
    {
        private readonly ReduxStoreManager _reduxStoreManager;

        public ConfigurationManager(ReduxStoreManager reduxStoreManager)
        {
            _reduxStoreManager = reduxStoreManager;
        }

        public async UniTask Load()
        {
            ConfigurationState configurationState;
            AppSettingsState settings = _reduxStoreManager.Store.GetState<AppSettingsState>(ReduxStoreManager.AppSettingsSlice);

            if (settings.OfflineMode)
            {
                configurationState = await JsonLoader.LoadStreamingAssetsJsonAsync<ConfigurationState>(settings.ConfigurationPath);
            }
            else
            {
                configurationState = await JsonLoader.LoadRemoteJsonAsync<ConfigurationState>("some url");
            }

            _reduxStoreManager.Store.Dispatch(new ActionCreator<ConfigurationState>(ReduxStoreManager.SetConfigurationAction).Invoke(configurationState));
        }
    }
}
