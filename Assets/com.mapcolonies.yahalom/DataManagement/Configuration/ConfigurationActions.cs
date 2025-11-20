using Unity.AppUI.Redux;

namespace com.mapcolonies.yahalom.DataManagement.Configuration
{
    public static class ConfigurationActions
    {
        public const string LoadAction = "configuration/load_configuration_action";

        public static void AddActions(SliceReducerSwitchBuilder<ConfigurationState> builder)
        {
            builder.AddCase(LoadConfigurationActionCreator(), ConfigurationReducer.Reduce);
        }

        public static IAction<ConfigurationState> LoadConfigurationAction(ConfigurationState payload)
        {
            return new ActionCreator<ConfigurationState>(LoadAction).Invoke(payload);
        }

        public static IActionCreator<ConfigurationState> LoadConfigurationActionCreator()
        {
            return new ActionCreator<ConfigurationState>(LoadAction);
        }
    }
}
