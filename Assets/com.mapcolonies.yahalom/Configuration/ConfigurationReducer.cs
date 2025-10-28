using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.Configuration
{
    public static class ConfigurationReducer
    {
        public static ConfigurationState Reduce(ConfigurationState state, IAction action)
        {
            switch (action)
            {
                case SetConfigurationAction setConfig:
                    return setConfig.ConfigurationState;

                default:
                    return state;
            }
        }
    }
}
