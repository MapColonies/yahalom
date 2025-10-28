using com.mapcolonies.yahalom.ReduxStore;
using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.Configuration
{
    public static class ConfigurationReducer
    {
        public static ConfigurationState Reduce(ConfigurationState state, IAction<ConfigurationState> action)
        {
            switch (action.type)
            {
                case ReduxStoreManager.SetConfigurationAction:
                    return action.payload;
                default:
                    return state;
            }
        }
    }
}
