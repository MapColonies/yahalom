using com.mapcolonies.core.Services.ConfigurationService;
using com.mapcolonies.yahalom.Redux;
using Cysharp.Threading.Tasks;
using Unity.AppUI.Redux;
using UnityEngine;

namespace com.mapcolonies.yahalom.Configuration
{
    public class ConfigurationManager
    {
        public ConfigurationManager(IStore<AppState> store)
        {
            Debug.Log("ConfigurationManager initialized");
        }

        public async UniTask Load()
        {
            // get offline / online config

            // if offline
            IConfigurationController configurationController;

            if (true) //offline
            {
                configurationController = new OfflineConfigurationController();
            }
            else
            {
                configurationController = new RemoteConfigurationController();
            }

            ConfigurationState configurationState = configurationController.Load();

        }
    }
}

