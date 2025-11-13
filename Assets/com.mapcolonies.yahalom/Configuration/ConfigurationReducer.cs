using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.Configuration
{
    public static class ConfigurationReducer
    {
        public const string SliceName = "configuration";

        public static ConfigurationState Reduce(ConfigurationState state, IAction<ConfigurationState> action)
        {
            return action.type switch
            {
                ConfigurationActions.LoadAction => action.payload,
                _ => state
            };
        }
    }
}
