using com.mapcolonies.yahalom.ReduxStore;
using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.Configuration
{
    public static class ConfigurationReducer
    {
        public static ConfigurationState Reduce(ConfigurationState state, IAction<ConfigurationState> action)
        {
            return action.type switch
            {
                ReduxStoreManager.SetConfigurationAction => action.payload,
                _ => state
            };
        }
    }
}
