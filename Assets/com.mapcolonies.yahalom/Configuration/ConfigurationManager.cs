using com.mapcolonies.core.Utilities;
using com.mapcolonies.yahalom.Redux;
using Cysharp.Threading.Tasks;
using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.Configuration
{
    public class ConfigurationManager
    {
        private readonly IStore<AppState> _store;

        public ConfigurationManager(IStore<AppState> store)
        {
            _store = store;
        }

        public async UniTask Load()
        {
            ConfigurationState configurationState = await JsonLoader.LoadStreamingAssetsJsonAsync<ConfigurationState>("config.json");
            _store.Dispatch(new SetConfigurationAction(configurationState));
        }
    }
}

